using System;
using UniRx;
using UniRx.Async;
using UnityEngine;
using UnityEngine.XR.MagicLeap;

namespace Utils
{
	public static class MagicLeapUtils
	{
		public static async UniTask WaitAllGranted(this PrivilegeRequester p, bool usingZi)
		{
			if (Application.isEditor && !usingZi)
			{
				return;
			}

			var result = await Observable
			                   .FromEvent<MLResult>(
				                   h => p.OnPrivilegesDone += h,
				                   h => p.OnPrivilegesDone -= h)
			                   .First();

			if (!result.IsOk)
			{
				throw new Exception("Privileges failed");
			}
		}

		public static void ThrowIfFail(this MLResult result)
		{
			if (result.Code == MLResultCode.PrivilegeDenied)
			{
				throw new Exception("Privilege denied.");
			}

			if (!result.IsOk)
			{
				throw new Exception(result.Code.ToString());
			}
		}
	}
}