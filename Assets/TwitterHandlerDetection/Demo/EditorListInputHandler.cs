using System;
using UniRx;
using UnityEngine;

namespace TwitterHandlerDetection.Demo
{
	public class EditorListInputHandler : IListInputHandler
	{
		readonly IDisposable _disposable;

		public event Action<int> OnIndexDeltaUpdated;
		public event Action OnDecisionIntended;

		public EditorListInputHandler()
		{
			_disposable = Observable.EveryUpdate().Subscribe(_ =>
			{
				Update();
			});
		}

		~EditorListInputHandler()
		{
			_disposable?.Dispose();
		}

		void Update()
		{
			if (Input.GetKeyUp(KeyCode.UpArrow))
			{
				OnIndexDeltaUpdated?.Invoke(-1);
			}
			else if (Input.GetKeyUp(KeyCode.DownArrow))
			{
				OnIndexDeltaUpdated?.Invoke(1);
			}
			else if (Input.GetKeyUp(KeyCode.Space))
			{
				OnDecisionIntended?.Invoke();
			}
		}
	}
}