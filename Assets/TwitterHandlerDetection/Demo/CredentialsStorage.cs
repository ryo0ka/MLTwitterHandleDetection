using UnityEngine;

namespace TwitterHandlerDetection.Demo
{
	public class CredentialsStorage : ScriptableObject, IGVCredentialStorage, ITBCredentialStorage
	{
		[SerializeField]
		string _apiKey;

		public string ApiKey => _apiKey;
	}
}