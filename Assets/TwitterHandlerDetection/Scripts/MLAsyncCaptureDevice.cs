using System;
using System.IO;
using TinyJpegSharp;
using UniRx.Async;
using UnityEngine;
using UnityEngine.XR.MagicLeap;
using Utils;

namespace TwitterHandlerDetection
{
	public class MLAsyncCaptureDevice : ICaptureDevice
	{
		ComputeBuffer _computeBuffer;
		ComputeShader _computeShader;
		Texture2D _previewTexture;
		int _width, _height;
		int _kernel;
		float[] _colors; // Intermediate texture (3 floats per pixel)
		byte[] _colorsPost; // Intermediate texture but half the width

		public void Enable()
		{
			MLCamera.Start().ThrowIfFail();
			MLCamera.Connect().ThrowIfFail();
			_previewTexture = MLCamera.StartPreview();

			_width = MLCamera.PreviewTextureWidth;
			_height = MLCamera.PreviewTextureHeight;

			_computeBuffer = new ComputeBuffer(_width * _height * 3, sizeof(float));
			_computeShader = Resources.Load<ComputeShader>("AsyncRead");

			_kernel = _computeShader.FindKernel("CSMain");
			_computeShader.SetInt("_Width", _width);
			_computeShader.SetInt("_Height", _height);
			_computeShader.SetTexture(_kernel, "_SrcTex", _previewTexture);
			_computeShader.SetBuffer(_kernel, "_DstBuffer", _computeBuffer);

			_colors = new float[_width * _height * 3]; // 3 = RGB
			_colorsPost = new byte[_width * _height * 3 / 2]; // 2 = half width
		}

		public void Disable()
		{
			MLCamera.StopPreview().ThrowIfFail();
			MLCamera.Disconnect().ThrowIfFail();
			MLCamera.Stop();
		}

		public Texture GetPreviewTexture() => _previewTexture;

		public async UniTask<byte[]> Capture()
		{
			_computeShader.Dispatch(_kernel, _width, _height, 1);

			// Wait for computation (generally 2~3 frames)
			await UniTask.DelayFrame(5);

			_computeBuffer.GetData(_colors);

			await UniTask.SwitchToThreadPool();

			DateTime encStart = DateTime.Now;
			PostProcess(_colors, _colorsPost, _width, _height);
			byte[] jpg = ToJPG(_colorsPost, _width / 2, _height);
			TimeSpan encTime = DateTime.Now - encStart;

			await UniTask.SwitchToMainThread();

			Debug.Log($"MLAsyncCaptureDevice.Capture() {encTime.TotalSeconds:0.0}");

			return jpg;
		}

		static void PostProcess(float[] src, byte[] dst, int width, int height)
		{
			int j = 0;
			for (int i = 0; i < src.Length; i++)
			{
				int p = (int) ((float) i / 3);
				int x = p % width;
				if (x < width / 2) continue; // dispose first half of width

				dst[j++] = (byte) src[i];
			}
		}

		static byte[] ToJPG(byte[] tex, int width, int height)
		{
			JpegEncoder e = new JpegEncoder();
			using (MemoryStream s = new MemoryStream())
			{
				e.Encode(tex, width, height, 3, 1, s);
				return s.ToArray();
			}
		}
	}
}