using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System;

public static class ScriptAssetProcessor
{
    private static Dictionary<Type, (int ID, int line)> scriptTypeToInstanceID = new();

    public static bool HasInstanceID(Type type)
    {
        return scriptTypeToInstanceID.ContainsKey(type);
    }

    public static (int ID, int line) GetInstanceID(Type type)
    {
        return scriptTypeToInstanceID[type];
    }

    [InitializeOnLoadMethod]
    private static void Initialize()
    {
        UpdateScriptTypeToInstanceID();
    }

    private static void UpdateScriptTypeToInstanceID()
    {
        scriptTypeToInstanceID.Clear();

        string scriptsFolder = "Assets/Script"; // Ruta a la carpeta de scripts (ajústala según tu estructura)
        string[] scriptFiles = Directory.GetFiles(scriptsFolder, "*.cs", SearchOption.AllDirectories);

        foreach (string scriptFile in scriptFiles)
        {
            MonoScript script = AssetDatabase.LoadAssetAtPath<MonoScript>(scriptFile);
            if (script != null)
            {
                string scriptText = script.text;
                string namespaceName = null;
                string[] lines = scriptText.Split('\n');

                for (int i = 0; i < lines.Length; i++)
                {
                    string line = lines[i];
                    string trimmedLine = line.Trim();

                    if (trimmedLine.StartsWith("namespace "))
                    {
                        namespaceName = trimmedLine.Substring("namespace ".Length).Trim();
                    }

                    int classIndex = trimmedLine.IndexOf("class ");
                    int structIndex = trimmedLine.IndexOf("struct ");

                    if (classIndex >= 0 || structIndex >= 0)
                    {
                        int index = classIndex >= 0 ? classIndex : structIndex;
                        string typeDeclaration = trimmedLine.Substring(index).Trim();

                        string[] typeParts = typeDeclaration.Split(' ');
                        if (typeParts.Length > 1)
                        {
                            string typeName = typeParts[1];
                            string fullTypeName = namespaceName != null ? $"{namespaceName}.{typeName}" : typeName;

                            string assemblyQualifiedName = $"{fullTypeName}, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null";

                            Type scriptType = Type.GetType(assemblyQualifiedName, false, true);
                            if (scriptType != null)
                            {
                                int instanceID = script.GetInstanceID();
                                if (!scriptTypeToInstanceID.ContainsKey(scriptType))
                                {
                                    scriptTypeToInstanceID.Add(scriptType, (instanceID, i+1));
                                }
                                else
                                {
                                    scriptTypeToInstanceID[scriptType] = (instanceID, i+1);
                                }
                            }
                        }
                    }
                }
            }
        }

        Debug.Log($"Cantidad de claves: {scriptTypeToInstanceID.Count}");
    }

    private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        foreach (string assetPath in importedAssets)
        {
            if (IsScriptFile(assetPath))
            {
                UpdateScriptTypeToInstanceID();
                break;
            }
        }

        foreach (string assetPath in deletedAssets)
        {
            if (IsScriptFile(assetPath))
            {
                UpdateScriptTypeToInstanceID();
                break;
            }
        }

        foreach (string assetPath in movedAssets)
        {
            if (IsScriptFile(assetPath))
            {
                UpdateScriptTypeToInstanceID();
                break;
            }
        }

        foreach (string assetPath in movedFromAssetPaths)
        {
            if (IsScriptFile(assetPath))
            {
                UpdateScriptTypeToInstanceID();
                break;
            }
        }
    }

    private static bool IsScriptFile(string path)
    {
        return path.EndsWith(".cs", StringComparison.OrdinalIgnoreCase);
    }
}