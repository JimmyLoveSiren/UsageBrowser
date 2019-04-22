#if UNITY_EDITOR
namespace J.UsageBrowser
{
	class GitIgnore : GitIgnore<GitIgnore>
	{
		[UnityEditor.InitializeOnLoadMethod]
		static void Init() => OnLoad();

		public override string Content =>
			"/" + nameof(UsageDatabase) + ".asset\n" +
			"/" + nameof(UsageDatabase) + ".asset.meta\n";
	}
}
#endif