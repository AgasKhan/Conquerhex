using CustomGraph;
using UnityEditor;
using UnityEngine;

namespace CustomGraph.Editor
{
    [UnityEditor.CustomEditor(typeof(CG_AssetGraph))]
    public class CG_AssetEditable : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            if(GUILayout.Button("Abrir Grafo"))
            {
                CG_EditorWindow.OpenWindow((CG_AssetGraph)target);
            }
        }

        public static bool OnOpenAsset(int id)
        {
            Object asset = EditorUtility.InstanceIDToObject(id);

            if(asset.GetType() == typeof(CG_AssetGraph))
            {
                CG_EditorWindow.OpenWindow((CG_AssetGraph)asset);
                return true;
            }

            return false;
        }
    }
}
