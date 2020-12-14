namespace AI
{
    public class BTNode
    {
        public enum Result { Running, Failure, Success }
        
        public BehaviourTree Tree { get; set; }

        public BTNode(BehaviourTree t)
        {
            Tree = t;
        }

        /// <summary>
        /// Executes a given task/node
        /// </summary>
        /// <returns>The result of this task; Failure, Running, or Success</returns>
        public virtual Result Execute()
        {
            return Result.Failure;
        }
    }
}
