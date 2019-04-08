using System;
using System.Collections.Generic;
using UnityEngine;

namespace TwitterHandlerDetection.Demo
{
	public class HandleListView : MonoBehaviour
	{
		[SerializeField]
		HandleListItemView _itemTemplate;

		[SerializeField]
		Transform _root;

		int _selectedIndex;
		List<HandleListItemView> _items;
		IListInputHandler _listHandler;

		public event Action<string> OnHandleSelected;

		void Awake()
		{
			_selectedIndex = 0;
			_items = new List<HandleListItemView>();

			_listHandler = Application.isEditor
				? (IListInputHandler) new EditorListInputHandler()
				: (IListInputHandler) new MLListInputHandler();
		}

		void Start()
		{
			_itemTemplate.gameObject.SetActive(false);

			_listHandler.OnIndexDeltaUpdated += delta =>
			{
				_selectedIndex += delta;
				UpdateSelectionView();
			};

			_listHandler.OnDecisionIntended += () =>
			{
				if (_selectedIndex >= _items.Count) return;
				
				string selectedHandle = _items[_selectedIndex].Handle;
				OnHandleSelected?.Invoke(selectedHandle);
			};
		}

		public void SetHandles(IEnumerable<string> handles)
		{
			_items.ForEach(i => Destroy(i.gameObject));
			_items.Clear();

			foreach (string handle in handles)
			{
				var item = Instantiate(_itemTemplate, _root);
				item.gameObject.SetActive(true);
				item.SetHandle(handle);
				_items.Add(item);
			}

			UpdateSelectionView();
		}

		void UpdateSelectionView()
		{
			_selectedIndex = Mathf.Clamp(_selectedIndex, 0, _items.Count - 1);

			for (var i = 0; i < _items.Count; i++)
			{
				_items[i].SetSelected(i == _selectedIndex);
			}
		}
	}
}