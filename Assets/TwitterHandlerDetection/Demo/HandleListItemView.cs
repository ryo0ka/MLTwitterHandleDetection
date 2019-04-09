using UnityEngine;
using UnityEngine.UI;

namespace TwitterHandlerDetection.Demo
{
	public class HandleListItemView : MonoBehaviour
	{
		[SerializeField]
		Button _button;

		[SerializeField]
		Text _handleText;

		public string Handle => _handleText.text;

		public void SetHandle(string handle)
		{
			_handleText.text = handle;
		}

		public void SetSelected(bool selected)
		{
			_button.interactable = true;//selected;
		}
	}
}