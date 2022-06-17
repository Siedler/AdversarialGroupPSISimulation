using System.Collections.Generic;

namespace Scrips.Agent.Memory {
	public class FoodCluster {
		private AgentMemoryWorldCell _center;
		private HashSet<AgentMemoryWorldCell> _clusterMembers;

		public FoodCluster(AgentMemoryWorldCell center, HashSet<AgentMemoryWorldCell> clusterMembers) {
			_center = center;
			_clusterMembers = clusterMembers;
		}
		
		public void SetCenter(AgentMemoryWorldCell newCenter) {
			_center = newCenter;
		}

		public void SetClusterMembers(HashSet<AgentMemoryWorldCell> newClusterMembers) {
			_clusterMembers = newClusterMembers;
		}

		public AgentMemoryWorldCell GetCenter() {
			return _center;
		}

		public HashSet<AgentMemoryWorldCell> GetClusterMembers() {
			return _clusterMembers;
		}
	}
}