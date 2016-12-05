

namespace PathFinder
{
	class AStarNode
	{
		public int x;
		public int y;
		public float g;
		public float h;
		public float f;
		public AStarNode parent;
		public AStarNode next;	// for linked list

		public AStarNode(int _x, int _y)
		{
			x = _x;
			y = _y;
			g = h = f = 0;
			parent = null;
			next = null;
		}
	}

}