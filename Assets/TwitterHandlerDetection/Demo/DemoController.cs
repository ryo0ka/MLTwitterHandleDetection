using System;
using UniRx.Async;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.MagicLeap;
using Utils;

namespace TwitterHandlerDetection.Demo
{
	public class DemoController : MonoBehaviour
	{
		[SerializeField]
		PrivilegeRequester _privilegeRequester;

		[SerializeField]
		CredentialsStorage _credentials;

		[SerializeField]
		Text _handleText;

		[SerializeField]
		RawImage _captureImage;

		ICaptureDevice _captureDevice;
		GVTextClient _visionTextClient;
		TwitterHandleInterpreter _textInterpreter;
		Texture2D _captureTexture;

		void Start()
		{
			DoStart().Forget(Debug.LogException);
		}

		async UniTask DoStart()
		{
			await _privilegeRequester.WaitAllGranted(usingZi: false);

			Debug.Log("Finished Magic Leap privileges");

			_captureDevice = Application.isEditor
				? (ICaptureDevice) new EditorCaptureDevice()
				: (ICaptureDevice) new MLCaptureDevice();

			_visionTextClient = new GVTextClient(_credentials);
			_textInterpreter = new TwitterHandleInterpreter();

			_captureTexture = new Texture2D(2, 2);
			_captureImage.texture = _captureTexture;

			_captureDevice.Enable();

			while (this)
			{
				DateTime captureStart = DateTime.Now;
				
				byte[] image = await _captureDevice.Capture();

				await UniTask.SwitchToMainThread();

				_captureTexture.LoadImage(image);
				_captureTexture.Apply();

				TimeSpan captureTime = DateTime.Now - captureStart;
				DateTime annotateStart = DateTime.Now;

				var annotation = await _visionTextClient.Annotate(image);
				if (annotation == null)
				{
					Debug.LogWarning("Received null text annotation");
					continue;
				}

				string text = annotation.Description;
				
				bool foundAnew = _textInterpreter.Interpret(text);

				_handleText.text = string.Join("\n", _textInterpreter.Handles);

				TimeSpan annotateTime = DateTime.Now - annotateStart;

				Debug.Log("Finished a cycle. " +
				          $"Found anew: {foundAnew}, " +
				          $"Capture time: {captureTime.TotalSeconds}, " +
				          $"Annotate time: {annotateTime.TotalSeconds}.");
			}
		}

		void OnDisable()
		{
			_captureDevice.Disable();
			_textInterpreter.Clear();
		}
	}
}