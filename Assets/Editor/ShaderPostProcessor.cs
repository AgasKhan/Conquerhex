using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;

public class ShaderGraphExporter : EditorWindow
{
    private Object shaderGraph;
    private string shaderGraphPath;
    private string outputShaderPath;

    [MenuItem("Tools/Export Shader Graph")]
    public static void ShowWindow()
    {
        GetWindow<ShaderGraphExporter>("Export Shader Graph");
    }

    private void OnGUI()
    {
        GUILayout.Label("Export Shader Graph to Shader", EditorStyles.boldLabel);

        shaderGraph = EditorGUILayout.ObjectField("Shader Graph Path", shaderGraph, typeof(Object), false);

        shaderGraphPath = AssetDatabase.GetAssetPath(shaderGraph);

        if(shaderGraphPath != null || shaderGraphPath!=string.Empty )
        {
            int index = shaderGraphPath.LastIndexOf(".");
            if (index >= 0)
            {
                outputShaderPath = shaderGraphPath.Substring(0, index) + ".shader";
                Debug.Log(shaderGraphPath + "\n" + outputShaderPath+"\n"+ shaderGraph.GetType().FullName);
            }
            else
                shaderGraph = null;
        }

        if (GUILayout.Button("Export and Modify Shader"))
        {
            ExportAndModifyShader();
        }
    }

    private void ExportAndModifyShader()
    {
        if (shaderGraph==null)
        {
            Debug.LogError("Please specify a shadergraph");
            return;
        }

        // Generate the Shader code using reflection
        string shaderCode = GenerateShaderCode(shaderGraph);

        if (shaderCode == string.Empty)
            return;

        // Write the Shader code to the output path
        File.WriteAllText(outputShaderPath, shaderCode);

        // Modify the Shader to add the Stencil block
        ModifyShader(outputShaderPath);

        // Reimport the Shader
        AssetDatabase.ImportAsset(outputShaderPath, ImportAssetOptions.ForceUpdate);
    }

    private string GenerateShaderCode(Object shaderGraph)
    {
        // Use reflection to get the internal method and properties for generating shader code
        var graphDataField = shaderGraph.GetType().GetField("m_GraphData", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        // Verificar si el campo m_GraphData existe y no es nulo
        if (graphDataField != null)
        {
            var graphData = graphDataField.GetValue(shaderGraph);

            if (graphData != null)
            {
                var generatorMethod = graphData.GetType().GetMethod("GetShaderText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

                // Verificar si el método GetShaderText existe y no es nulo
                if (generatorMethod != null)
                {
                    var shaderCode = (string)generatorMethod.Invoke(graphData, null);
                    return shaderCode;
                }
            }
            else
            {
                Debug.LogError("value of m_GraphData null");
            }
        }
        else
        {
            Debug.LogError("m_GraphData null");
        }

        // Si no se pudo obtener el código del shader, devuelve una cadena vacía o maneja el error según sea necesario
        Debug.LogError("Failed to generate shader code.");
        return string.Empty;
    }

    private void ModifyShader(string shaderPath)
    {
        string[] lines = File.ReadAllLines(shaderPath);

        // Verificar si el shader tiene la propiedad _StencilMask
        bool hasStencilMaskProperty = lines.Any(line => line.Contains("_StencilMask"));
        if (!hasStencilMaskProperty)
        {
            return; // No modificar este shader si no tiene la propiedad _StencilMask
        }

        // Verificar si ya tiene el bloque de Stencil
        bool hasStencilBlock = lines.Any(line => line.Trim().StartsWith("Stencil"));
        if (hasStencilBlock)
        {
            return; // No modificar este shader si ya tiene el bloque de Stencil
        }

        // Si tiene la propiedad y no tiene el bloque de Stencil, modificar el shader
        using (StreamWriter writer = new StreamWriter(shaderPath))
        {
            bool subShaderFound = false;
            foreach (string line in lines)
            {
                writer.WriteLine(line);

                // Añadir las líneas del Stencil Buffer en el lugar apropiado
                if (line.Trim().StartsWith("SubShader"))
                {
                    subShaderFound = true;
                }

                // Añadir el bloque de stencil después de Tags { ... }
                if (subShaderFound && line.Trim().StartsWith("Tags"))
                {
                    writer.WriteLine("        Stencil");
                    writer.WriteLine("        {");
                    writer.WriteLine("            Ref [_StencilMask]");
                    writer.WriteLine("            Comp Equal");
                    writer.WriteLine("        }");
                    subShaderFound = false; // Solo agregar una vez
                }
            }
        }
    }
}