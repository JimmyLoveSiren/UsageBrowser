namespace J.Algorithm
{
	using System;
	using System.Collections.Generic;

	public static partial class Search
	{
		public static SearchNodeExpander<T> ToNodeExpander<T>(this Func<T, IEnumerable<T>> func) =>
			node => func(node.Value);

		public static SearchNodeExpander<T> ToNodeExpander<T>(this SearchExpander<T> expander) =>
			node => expander(node.Value);
	}

	public delegate IEnumerable<T> SearchExpander<T>(T node);

	public delegate IEnumerable<T> SearchNodeExpander<T>(SearchNode<T> node);

	public delegate bool SearchFilter<T>(SearchNode<T> node);

	public sealed class SearchNode<T>
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