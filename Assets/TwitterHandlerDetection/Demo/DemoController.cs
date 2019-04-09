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
		Text _annotateText;

		[SerializeField]
		RawImage _captureImage;

		[SerializeField]
		RawImage _jpgImage;

		[SerializeField]
		HandleListView _handleList;

		[SerializeField]
		GameObject _captureIndicator;

		[SerializeField]
		GameObject _annotateIndicator;

		ICaptureDevice _captureDevice;
		GVTextClient _visionTextClient;
		TwitterHandleInterpreter _textInterpreter;
		Texture2D _jpgTexture;

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
				: (ICaptureDevice) new MLAsyncCaptureDevice();

			_visionTextClient = new GVTextClient(_credentials);
			_textInterpreter = new TwitterHandleInterpreter();

			_handleList.OnHandleSelected += handle =>
			{
				OnHandleSelected(handle);
			};

			_jpgTexture = new Texture2D(1, 1);
			_jpgImage.texture = _jpgTexture;

			_captureDevice.Enable();
			_captureImage.texture = _captureDevice.GetPreviewTexture();

			while (this)
			{
				_captureIndicator.SetActive(true);

				DateTime captureStart = DateTime.Now;
				byte[] image = await _captureDevice.Capture();
				TimeSpan captureTime = DateTime.Now - captureStart;

				_jpgTexture.LoadImage(image);
				_jpgTexture.Apply();

				_captureIndicator.SetActive(false);
				_annotateIndicator.SetActive(true);

				DateTime annotateStart = DateTime.Now;
				var annotation = await _visionTextClient.Annotate(image);
				TimeSpan annotateTime = DateTime.Now - annotateStart;

				Debug.Log($"Capture: {captureTime.TotalSeconds}, " +
				          $"Annotate: {annotateTime.TotalSeconds}");

				_annotateIndicator.SetActive(false);

				if (annotation == null)
				{
					Debug.LogWarning("Received null text annotation");
					_annotateText.text = "<null>";
					continue;
				}

				string text = annotation.Description;
				_annotateText.text = text.Replace("\n", " ");

				_textInterpreter.Clear();
				if (_textInterpreter.Interpret(text))
				{
					Debug.Log("Found new handle(s)");
				}

				_handleList.SetHandles(_textInterpreter.Handles);
			}
		}

		void OnDisable()
		{
			_captureDevice.Disable();
			_textInterpreter.Clear();
		}

		void OnHandleSelected(string handle)
		{
			Debug.Log(handle);
		}
	}
}