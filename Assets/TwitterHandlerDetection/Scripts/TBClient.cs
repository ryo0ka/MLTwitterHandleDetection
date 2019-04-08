using UniRx.Async;

namespace TwitterHandlerDetection
{
	public class TBClient
	{
		readonly ITBCredentialStorage _credentials;
		
		public TBClient(ITBCredentialStorage credentials)
		{
			_credentials = credentials;
		}

		public UniTask Tweet(string message)
		{
			return default(UniTask);
		}
	}
}