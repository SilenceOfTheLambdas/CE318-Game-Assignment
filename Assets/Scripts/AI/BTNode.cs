using UnityEngine;

namespace AI
{
    public class BTNode : ScriptableObject
    {
        public enum Result { Running, Failure, Success }

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
