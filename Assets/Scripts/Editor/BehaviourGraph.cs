using UnityEngine.Experimental.UIElements;
using UnityEditor.Experimental.UIElements;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class BehaviourGraph : EditorWindow
    {
        private BehaviourGraphView _graphView;
        
        [MenuItem("Graph/Behaviour Graph")]
        public static void OpenBehaviourGraphWindow()
        {
            var window = GetWindow<BehaviourGraph>();
            window.titleContent = new GUIContent("Behaviour Graph");
        }

        private void OnEnable()
        {
            ConstructGraphView();
            GenerateToolbar();
        }

        private void ConstructGraphView()
        {
            _graphView = new BehaviourGraphView
            {
                name = "Dialogue Graph"
            };
            _graphView.StretchToParentSize();
            this.GetRootVisualContainer().Add(_graphView);
        }

        private void GenerateToolbar()
        {
            var toolbar = new Toolbar();

            var nodeCreateButton = new Button(() => { _graphView.CreateNode("Behaviour Node"); })
            {
                text = "Create Behaviour Node", name = "Create Behaviour Node"
            };
            toolbar.Add(nodeCreateButton);
            
            this.GetRootVisualContainer().Add(toolbar);
        }

        private void OnDisable()
        {
            this.GetRootVisualContainer().Remove(_graphView);
        }
    }
}