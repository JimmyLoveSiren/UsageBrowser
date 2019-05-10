namespace J
{
	using System.Runtime.CompilerServices;

	public static class CallerInfo
	{
		public static string FilePath([CallerFilePath] string _ = default(string)) => _;

		public static int LineNumber([CallerLineNumber] int _ = default(int)) => _;

		public static string MemberName([CallerMemberName] string _ = default(string)) => _;
	}
}