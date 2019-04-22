namespace J.EditorOnly.UsageBrowser
{
	using System;
	using System.Collections.Generic;

	[Serializable]
	sealed class Entry
	{
		public string Id;
		public List<string> DependIds;

		public Entry()
		{
		}

		public Entry(string id, List<string> dependIds)
		{
			Id = id;
			DependIds = dependIds;
		}
	}
}
