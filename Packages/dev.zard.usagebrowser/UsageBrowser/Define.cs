namespace J.EditorOnly.UsageBrowser
{
	using System.IO;

	static class Define
	{
		public static readonly string DatabasePath =
			Path.ChangeExtension(EditorUtilityJ.GetScriptPathOfScriptableObject<UsageDatabase>(), "asset");
	}
}