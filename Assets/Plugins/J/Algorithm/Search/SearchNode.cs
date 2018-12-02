namespace J.Algorithm
{
	public static partial class Search { }

	public class SearchNode<T>
	{
		public readonly int Index;
		public readonly T Value;
		public readonly SearchNode<T> Parent;
		public readonly int Depth;

		public SearchNode(int index, T value, SearchNode<T> parent = null)
		{
			Index = index;
			Value = value;
			if (parent != null)
			{
				Parent = parent;
				Depth = parent.Depth + 1;
			}
		}
	}
}
