#if UNITY_EDITOR
namespace J.EditorOnly.UsageBrowser
{
	sealed class GitIgnore : GitIgnore<GitIgnore>
	{
		[UnityEditor.InitializeOnLoadMethod]
		static void OnLoad() => OnLoad(@"
/UsageDatabase.asset
/UsageDatabase.asset.meta
");
	}
}
#endif