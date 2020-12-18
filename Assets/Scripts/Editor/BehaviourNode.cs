using Node = UnityEditor.Experimental.UIElements.GraphView.Node;

namespace Editor
{
    public class BehaviourNode : Node
    {
        public string GUID;

        public string BehaviourText;
        public bool   EntryPoint = false;
    }
}