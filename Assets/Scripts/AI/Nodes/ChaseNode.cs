using UnityEngine;
using UnityEngine.AI;

namespace AI.LeafNodes
{
    public class ChaseNode : BTNode
    {
        private Transform    target;
        private NavMeshAgent agent;

        public ChaseNode(BehaviourTree t, Transform target, NavMeshAgent agent) : base(t)
        {
            this.target = target;
            this.agent = agent;
        }

        public override Result Execute()
        {
            var distance = Vector3.Distance(target.position, agent.transform.position);
            {
                if (distance > 0.2f)
                {
                    agent.isStopped = false;
                    agent.SetDestination(target.position);
                    return Result.Running;
                }
                else
                {
                    agent.isStopped = true;
                    return Result.Success;
                }
            }
        }
    }
}