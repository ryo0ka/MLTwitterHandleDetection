using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace TwitterHandlerDetection
{
	public class TwitterHandleInterpreter
	{
		readonly Regex _regex;
		readonly HashSet<string> _handles;

		public TwitterHandleInterpreter()
		{
			_regex = new Regex("@([0-9a-zA-Z_]{2,})");
			_handles = new HashSet<string>();
		}

		public IEnumerable<string> Handles => _handles;

		public void Clear()
		{
			_handles.Clear();
		}

		public bool Interpret(string text)
		{
			bool found = false;
			foreach (Match match in _regex.Matches(text))
			{
				string handle = match.Value.ToLower();
				found |= _handles.Add(handle);
			}

			return found;
		}
	}
}