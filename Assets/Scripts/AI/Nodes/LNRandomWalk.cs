using UnityEngine;

namespace AI.LeafNodes
{
    public class LNRandomWalk : BTNode
    {
        protected Vector3 nextDestination { get; set; }
        public float speed = 10;
        
        public LNRandomWalk(BehaviourTree t) : base(t)
        {
            nextDestination = Vector3.zero;
            FindNextDestination();
        }

        private bool FindNextDestination()
        {
            var found = false;

            found = Tree.Blackboard.TryGetValue("WorldBounds", out var o);
            if (found)
            {
                var  bounds = (Rect) o;
                var x      = Random.value * bounds.width;
                var y      = Random.value * bounds.height;
                nextDestination = new Vector3(x, y, 0);
            }
            
            return found;
        }

        public override Result Execute()
        {
            // if we have arrived at the new point
            if (Tree.gameObject.transform.position == nextDestination)
            {
                return !FindNextDestination() ? Result.Failure : Result.Success;
            }

            Tree.gameObject.transform.position = Vector3.MoveTowards(Tree.gameObject.transform.position,
                nextDestination, Time.deltaTime * speed);
            return Result.Running;
        }
    }
}