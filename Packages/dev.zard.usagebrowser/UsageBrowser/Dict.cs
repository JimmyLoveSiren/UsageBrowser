namespace J.EditorOnly.UsageBrowser
{
	using System.Collections.Generic;

	sealed class Dict : Dictionary<string, HashSet<string>>
	{
		public HashSet<string> GetOrDefault(string key)
		{
			HashSet<string> value;
			TryGetValue(key, out value);
			return value;
		}

		public HashSet<string> GetOrAdd(string key)
		{
			HashSet<string> value;
			if (!TryGetValue(key, out value))
				Add(key, value = new HashSet<string>());
			return value;
		}
	}
}