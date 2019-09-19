namespace J.Algorithm
{
	using System.Collections.Generic;
	using System.Linq;

	partial class Search
	{
		public static IEnumerable<SearchNode<T>> BreadthFirst<T>(IEnumerable<T> start,
			SearchNodeExpander<T> expander, int maxDepth) => maxDepth >= 0
			? BreadthFirst(start, expander, node => node.Depth < maxDepth)
			: BreadthFirst(start, expander);

		public static IEnumerable<SearchNode<T>> BreadthFirst<T>(IEnumerable<T> start,
			SearchNodeExpander<T> expander, SearchFilter<T> expandFilter) => expandFilter != null
			? BreadthFirst(start, node => expandFilter(node) ? expander(node) : Enumerable.Empty<T>())
			: BreadthFirst(start, expander);

		public static IEnumerable<SearchNode<T>> BreadthFirst<T>(IEnumerable<T> start, SearchNodeExpander<T> expander)
		{
			var visit = new HashSet<T>();
			var queue = new Queue<SearchNode<T>>();
			int count = 0;
			foreach (var value in start)
				if (visit.Add(value))
					queue.Enqueue(new SearchNode<T>(count++, value));
			while (queue.Count > 0)
			{
				var node = queue.Dequeue();
				yield return node;
				foreach (var value in expander(node))
					if (visit.Add(value))
						queue.Enqueue(new SearchNode<T>(count++, value, node));
			}
		}
	}
}