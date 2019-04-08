using System;
using UniRx;
using UniRx.Async;
using UnityEngine.XR.MagicLeap;
using Utils;

namespace TwitterHandlerDetection
{
	public class MLCaptureDevice : ICaptureDevice
	{
		public void Enable()
		{
			MLCamera.Start().ThrowIfFail();
			MLCamera.Connect().ThrowIfFail();
		}

		public void Disable()
		{
			MLCamera.Stop();
		}

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