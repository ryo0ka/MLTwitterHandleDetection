using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UniRx.Async;
using UnityEngine.Networking;
using Utils;

namespace TwitterHandlerDetection
{
	public class GVTextClient
	{
		const string UrlTemplate = "https://vision.googleapis.com/v1/images:annotate?key=";

		readonly IGVCredentialStorage _credentials;

		public GVTextClient(IGVCredentialStorage credentials)
		{
			_credentials = credentials;
		}

		public async UniTask<GVEntityAnnotation> Annotate(byte[] image)
		{
			await UniTask.SwitchToThreadPool();

			string imageBase64 = Convert.ToBase64String(image);

			var requestData = new GVRequestBody
			{
				Requests = new[]
				{
					new GVAnnotateImageRequest
					{
						Image = new GVImage
						{
							Content = imageBase64,
						},

						Features = new[]
						{
							new GVFeature
							{
								Type = GVType.TEXT_DETECTION,
							},
						},
					},
				},
			};

			string url = $"{UrlTemplate}{_credentials.ApiKey}";
			var requestDataJson = JsonConvert.SerializeObject(requestData);

			await UniTask.SwitchToMainThread();

			using (var req = UnityWebRequest.Put(url, requestDataJson))
			{
				// Counter bug: UnityWebRequest.POST() will URL-encode Base64 data
				req.method = "POST";

				await req.SendWebRequest();

				string responseText = req.downloadHandler.text;

				if (!string.IsNullOrEmpty(req.error))
				{
					throw new Exception($"Failed request ({req.responseCode}): '{req.error}', {responseText}");
				}

				await UniTask.SwitchToThreadPool();

				var body = JsonConvert.DeserializeObject<GVAnnotateImageResponseBody>(responseText);
				GVAnnotateImageResponse response = null;
				if (!body.Responses?.TryGetFirstValue(out response) ?? false)
				{
					throw new Exception("Received null or empty responses");
				}

				GVEntityAnnotation annotation = null;
				if (!response?.TextAnnotations?.TryGetFirstValue(out annotation) ?? false)
				{
					throw new Exception("Received null or empty annotations");
				}

				await UniTask.SwitchToMainThread();

				return annotation;
			}
		}
	}

	[Serializable]
	public class GVRequestBody
	{
		[JsonProperty("requests")]
		public IEnumerable<GVAnnotateImageRequest> Requests { get; set; }
	}

	[Serializable]
	public class GVAnnotateImageRequest
	{
		[JsonProperty("image")]
		public GVImage Image { get; set; }

		[JsonProperty("features")]
		public IEnumerable<GVFeature> Features { get; set; }
	}

	[Serializable]
	public class GVImage
	{
		[JsonProperty("content")]
		public string Content { get; set; }
	}

	[Serializable]
	public class GVFeature
	{
		[JsonProperty("type")]
		public GVType Type { get; set; }
	}

	public enum GVType
	{
		// ReSharper disable UnusedMember.Global
		TYPE_UNSPECIFIED,
		FACE_DETECTION,
		LANDMARK_DETECTION,
		LOGO_DETECTION,
		LABEL_DETECTION,
		TEXT_DETECTION,
		DOCUMENT_TEXT_DETECTION,
		SAFE_SEARCH_DETECTION,
		IMAGE_PROPERTIES,
		CROP_HINTS,
		WEB_DETECTION,
		OBJECT_LOCALIZATION,
		// ReSharper restore UnusedMember.Global
	}

	[Serializable]
	public class GVAnnotateImageResponseBody
	{
		[JsonProperty("responses")]
		public IEnumerable<GVAnnotateImageResponse> Responses { get; private set; }
	}

	[Serializable]
	public class GVAnnotateImageResponse
	{
		[JsonProperty("textAnnotations")]
		public IEnumerable<GVEntityAnnotation> TextAnnotations { get; private set; }
	}

	[Serializable]
	public class GVEntityAnnotation
	{
		[JsonProperty("description")]
		public string Description { get; private set; }

		[JsonProperty("score")]
		public float Score { get; private set; }
	}
}