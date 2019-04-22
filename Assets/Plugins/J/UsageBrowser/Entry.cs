#if UNITY_EDITOR
namespace J.UsageBrowser
{
	using System;
	using System.Collections.Generic;

	partial class UsageDatabase
	{
		[Serializable]
		class Entry
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
}
#endif