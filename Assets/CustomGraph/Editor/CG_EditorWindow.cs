
using UnityEngine;
using UnityEditor;
using CustomGraph;
using UnityEditor.Experimental.GraphView;
using System;

namespace CustomGraph.Editor
{
    public class CG_EditorWindow : EditorWindow
    {
        [SerializeField]
        CG_AssetGraph _currentGraph;

        [SerializeField]
        SerializedObject _serializedWindow;

        CG_GraphView _currentGraphView;

        public CG_AssetGraph CurrentGraph => _currentGraph;



        public static void OpenWindow(CG_AssetGraph asset)
        {
            CG_EditorWindow[] windows = Resources.FindObjectsOfTypeAll<CG_EditorWindow>();

            foreach (var w in windows)
            {
                if (w.CurrentGraph == asset)
                {
                    w.Focus();
                    return;
                }
            }

            CG_EditorWindow window = CreateWindow<CG_EditorWindow>(typeof(CG_EditorWindow), typeof(SceneView));
            window.titleContent = new GUIContent($"{asset.name}",
            EditorGUIUtility.ObjectContent(null, typeof(CG_AssetGraph)).image);
            window.LoadWindow(asset);
        }

        void OnGUI()
        {
            if (_currentGraph == null) return;

            if (EditorUtility.IsDirty(_currentGraph))
                this.hasUnsavedChanges = true;
            else this.hasUnsavedChanges = false;
        }

        public void LoadWindow(CG_AssetGraph asset)
        {
            _currentGraph = asset;
            Draw();
        }

        void Draw()
        {
            _serializedWindow = new(_currentGraph);
            _currentGraphView = new(_serializedWindow, this);
            _currentGraphView.graphViewChanged += OnGraphChanged;
            rootVisualElement.Add(_currentGraphView);
        }

        private GraphViewChange OnGraphChanged(GraphViewChange graphViewChange)
        {
            EditorUtility.SetDirty(_currentGraph);
            return graphViewChange;
        }


        void OnEnable()
        {
            if (_currentGraph != null) Draw();
        }
    }
}
