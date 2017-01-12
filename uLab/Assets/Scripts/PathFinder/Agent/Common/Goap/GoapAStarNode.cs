

namespace Lite.Goap
{
	public class GoapAStarNode : AStar.AStarNode
	{
		public WorldState currentState;
		
		public WorldState goalState;

		public GoapAction fromAction;

		public GoapAStarNode(int stateCount)
		{
			currentState = new WorldState(stateCount);
			fromAction = null;
		}

		public override void Reset()
		{
			base.Reset();
			currentState.Reset();
			fromAction = null;
		}

	}

}