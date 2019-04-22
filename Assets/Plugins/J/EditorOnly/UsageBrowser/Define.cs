namespace J.EditorOnly.UsageBrowser
{
	using System.IO;

	static class Define
	{
		public static readonly string DatabasePath =
			Path.ChangeExtension(EditorUtilityJ.GetScriptPath<UsageDatabase>(), "asset");
	}

	public enum UsageType
	{
		Dependency = 0x00,
		Reference = 0x10,
	}
}