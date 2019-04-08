using System;

namespace TwitterHandlerDetection.Demo
{
	public interface IListInputHandler
	{
		event Action<int> OnIndexDeltaUpdated;
		event Action OnDecisionIntended;
	}
}