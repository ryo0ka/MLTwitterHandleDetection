using UniRx.Async;

namespace TwitterHandlerDetection
{
	public interface ICaptureDevice
	{
		void Enable();
		void Disable();
		UniTask<byte[]> Capture();
	}
}