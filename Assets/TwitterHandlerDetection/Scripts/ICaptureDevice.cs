using UniRx.Async;
using UnityEngine;

namespace TwitterHandlerDetection
{
	public interface ICaptureDevice
	{
		void Enable();
		void Disable();
		Texture GetPreviewTexture();
		UniTask<byte[]> Capture();
	}
}