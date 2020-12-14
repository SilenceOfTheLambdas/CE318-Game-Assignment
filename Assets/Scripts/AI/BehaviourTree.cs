using System;
using System.Collections;
using System.Collections.Generic;
using AI.LeafNodes;
using UnityEngine;

namespace AI
{
    public class BehaviourTree : MonoBehaviour
    {
        private bool      _startedBehaviour;
        private Coroutine behaviour;
        /// <summary>
        /// A "memory-space"
        /// </summary>
        public Dictionary<string, object> Blackboard { get; private set; }

        private BTNode Root { get; set; }

        private void Start()
        {
            Blackboard = new Dictionary<string, object> {{"WorldBounds", new Rect(0, 0, 5, 5)}};

            _startedBehaviour = false;
            
            // set the root behaviour
            Root = new BTRepeater(this, new BTSequencer(this, new BTNode[] { new LNRandomWalk(this) }));
        }

        private void Update()
        {
            if (_startedBehaviour) return;
            
            behaviour = StartCoroutine(RunBehaviour());
            _startedBehaviour = true;
        }

        private IEnumerator RunBehaviour()
        {
            var result = Root.Execute();
            while (result == BTNode.Result.Running)
            {
                Debug.Log("Root result " + result);
                yield return null;
                result = Root.Execute();
            }
            
            Debug.Log($"Behaviour has finished with {result}");
        }
    }
}