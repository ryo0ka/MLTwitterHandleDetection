using System;
using UniRx.Async;
using UnityEngine;

namespace TwitterHandlerDetection
{
	public class EditorCaptureDevice : ICaptureDevice
	{
		const string ImageAssetPath = "sample";

		Texture2D _texture;
		byte[] _image;

		public void Enable()
		{
			_texture = Resources.Load<Texture2D>(ImageAssetPath);

			if (_texture == null || !_texture)
			{
				throw new Exception($"Image asset not found. Path: {ImageAssetPath}");
			}

			_image = _texture.EncodeToJPG();
		}

		public void Disable()
		{
		}

		public Texture2D GetPreviewTexture() => _texture;

		public async UniTask<byte[]> Capture()
		{
			await UniTask.CompletedTask;

			return _image;
		}
	}
}