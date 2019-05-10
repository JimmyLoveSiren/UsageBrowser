namespace J.Algorithm
{
	using System;
	using System.Collections.Generic;

	partial class Search
	{
		public static IEnumerable<SearchNode<T>> BreadthFirst<T>(IEnumerable<T> start,
			Func<T, IEnumerable<T>> expander, int maxDepth = -1)
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
				if (maxDepth < 0 || node.Depth < maxDepth)
					foreach (var value in expander(node.Value))
						if (visit.Add(value))
							queue.Enqueue(new SearchNode<T>(count++, value, node));
			}
		}
	}
}