using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TwitterHandlerDetection.Demo
{
	public class HandleListView : MonoBehaviour
	{
		[SerializeField]
		GameObject _itemTemplate;

		[SerializeField]
		Transform _root;

		List<GameObject> _items;

		public event Action<string> OnHandleSelected;

		void Awake()
		{
			_items = new List<GameObject>();
			_itemTemplate.SetActive(false);
		}

		public void UpdateList(IEnumerable<string> handles)
		{
			_items.ForEach(Destroy);
			_items.Clear();

			foreach (string handle in handles)
			{
				var item = Instantiate(_itemTemplate, _root);

				var label = item.GetComponentInChildren<Text>();
				var button = item.GetComponentInChildren<Button>();

				label.text = handle;
				button.onClick.AddListener(() =>
				{
					OnHandleSelected?.Invoke(handle);
				});

				_items.Add(item);
			}
		}
	}
}