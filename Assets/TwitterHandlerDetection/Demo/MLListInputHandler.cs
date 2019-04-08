using System;
using UnityEngine.XR.MagicLeap;

namespace TwitterHandlerDetection.Demo
{
	public class MLListInputHandler : IListInputHandler
	{
		public event Action<int> OnIndexDeltaUpdated;
		public event Action OnDecisionIntended;

		public MLListInputHandler()
		{
			MLInput.OnControllerTouchpadGestureEnd += (b, gesture) =>
			{
				if (gesture.Type == MLInputControllerTouchpadGestureType.Swipe)
				{
					int delta = SwipeDirectionToIndexDelta(gesture.Direction);
					OnIndexDeltaUpdated?.Invoke(delta);
				}
			};

			MLInput.OnTriggerUp += (b, f) =>
			{
				OnDecisionIntended?.Invoke();
			};
		}

		int SwipeDirectionToIndexDelta(MLInputControllerTouchpadGestureDirection direction)
		{
			switch (direction)
			{
				case MLInputControllerTouchpadGestureDirection.Up: return 1;
				case MLInputControllerTouchpadGestureDirection.Down: return -1;
				default: return 0;
			}
		}
	}
}