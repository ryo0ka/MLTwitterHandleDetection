using System;
using UniRx.Async;
using UnityEngine;

namespace TwitterHandlerDetection
{
	public class EditorCaptureDevice : ICaptureDevice
	{
		const string ImageAssetPath = "sample";

		byte[] _image;

		public void Enable()
		{
			var tex = Resources.Load<Texture2D>(ImageAssetPath);

			if (tex == null || !tex)
			{
				throw new Exception($"Image asset not found. Path: {ImageAssetPath}");
			}

			_image = tex.EncodeToJPG();
		}

		public void Disable()
		{
		}

		public async UniTask<byte[]> Capture()
		{
			await UniTask.CompletedTask;

			return _image;
		}
	}
}