using UniRx.Async;
using UnityEngine;

namespace TwitterHandlerDetection
{
	public interface ICaptureDevice
	{
		void Enable();
		void Disable();
		UniTask<byte[]> Capture();
		Texture2D GetPreviewTexture();
	}
}