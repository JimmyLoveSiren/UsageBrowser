namespace J.EditorOnly.UsageBrowser
{
	using UnityEditor;

	sealed class GitIgnore : GitIgnore<GitIgnore>
	{
		[InitializeOnLoadMethod]
		static void OnLoad() => OnLoad(@"
/UsageDatabase.asset
/UsageDatabase.asset.meta
");
	}
}