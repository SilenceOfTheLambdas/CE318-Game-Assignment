using UnityEditor.Experimental.UIElements.GraphView;
using UnityEngine.Experimental.UIElements;
using UnityEngine;
using System;
using System.Collections.Generic;

namespace Editor
{
    public class BehaviourGraphView : GraphView
    {
        private readonly Vector2 _defaultNodeSize = new Vector2(150, 200);
        public BehaviourGraphView()
        {
            AddStyleSheetPath("BehaviourGraph");
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
            
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            
            var gridBg = new GridBackground();
            Insert(0, gridBg);
            
            AddElement(GenerateEntryPointNode());
        }

        private Port GeneratePort(BehaviourNode node, Direction portDirection,
            Port.Capacity capacity = Port.Capacity.Single)
        {
            return node.InstantiatePort(Orientation.Horizontal, portDirection, capacity, typeof(float));
        }

        private BehaviourNode GenerateEntryPointNode()
        {
            var node = new BehaviourNode
            {
                title = "Start",
                GUID = Guid.NewGuid().ToString(),
                BehaviourText = "ROOT_NODE",
                EntryPoint = true
            };

            var generatedPort = GeneratePort(node, Direction.Output);
            generatedPort.portName = "Next";
            node.outputContainer.Add(generatedPort);
            
            node.RefreshExpandedState();
            node.RefreshPorts();
            
            node.SetPosition(new Rect(100, 200, 100, 100));
            return node;
        }

        public void CreateNode(string nodeName)
        {
            AddElement(CreateBehaviourNode(nodeName));
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            var compatiblePorts = new List<Port>();

            ports.ForEach(port =>
            {
                if (startPort != port && startPort.node != port.node)
                    compatiblePorts.Add(port);
            });

            return compatiblePorts;
        }

        private BehaviourNode CreateBehaviourNode(string nodeName)
        {
            var behaviourNode = new BehaviourNode
            {
                title = nodeName,
                BehaviourText = nodeName,
                GUID = Guid.NewGuid().ToString()
            };

            var inputPort = GeneratePort(behaviourNode, Direction.Input, Port.Capacity.Multi);
            inputPort.portName = "Input";
            behaviourNode.inputContainer.Add(inputPort);

            var button = new Button(() => { AddChoicePort(behaviourNode); }) {text = "New Choice"};
            behaviourNode.titleContainer.Add(button);
            behaviourNode.RefreshExpandedState();
            behaviourNode.RefreshPorts();
            behaviourNode.SetPosition(new Rect(Vector2.zero, _defaultNodeSize));

            return behaviourNode;
        }

        private void AddChoicePort(BehaviourNode behaviourNode)
        {
            var generatedPort = GeneratePort(behaviourNode, Direction.Output);

            var outputPortCount = behaviourNode.outputContainer.Query("connector").ToList().Count;
            generatedPort.name = $"Choice {outputPortCount + 1}";
            
            behaviourNode.outputContainer.Add(generatedPort);
            behaviourNode.RefreshPorts();
            behaviourNode.RefreshExpandedState();
        }
    }
}