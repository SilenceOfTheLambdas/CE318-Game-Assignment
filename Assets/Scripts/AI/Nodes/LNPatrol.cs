using UnityEngine.AI;

namespace AI.LeafNodes
{
    public class LNPatrol : BTNode
    {
        private NavMeshAgent _navMeshAgent;
        
        public LNPatrol(BehaviourTree t) : base(t)
        {
            _navMeshAgent = Tree.gameObject.GetComponent<NavMeshAgent>();
        }
    }
}