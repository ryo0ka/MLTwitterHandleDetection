using System;
using UniRx;
using UniRx.Async;
using UnityEngine;
using UnityEngine.XR.MagicLeap;
using Utils;

namespace TwitterHandlerDetection
{
	public class MLCaptureDevice : ICaptureDevice
	{
		Texture2D _previewTexture;

		public void Enable()
		{
			MLCamera.Start().ThrowIfFail();
			MLCamera.Connect().ThrowIfFail();
			_previewTexture = MLCamera.StartPreview();
		}

		public void Disable()
		{
			MLCamera.StopPreview().ThrowIfFail();
			MLCamera.Disconnect().ThrowIfFail();
			MLCamera.Stop();
		}

		public Texture2D GetPreviewTexture() => _previewTexture;

		public async UniTask<byte[]> Capture()
		{
			await UniTask.SwitchToThreadPool();

			if (!MLCamera.IsStarted)
			{
				throw new Exception("MLCamera not running");
			}

			UniTask<byte[]> receiveCapturedImage =
				Observable.FromEvent<byte[]>(
					          h => MLCamera.OnRawImageAvailable += h,
					          h => MLCamera.OnRawImageAvailable -= h)
				          .ToUniTask(useFirstValue: true);

			MLCamera.CaptureRawImageAsync().ThrowIfFail();

			return await receiveCapturedImage;
		}
	}
}