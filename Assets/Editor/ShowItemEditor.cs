using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Reflection;

/*

[CustomEditor(typeof(Object), true)]
public class ShowItemEditor : Editor
{
    private bool button;

    private Vector2 scrollPos;

    private Dictionary<string, Editor> editors = new Dictionary<string, Editor>();

    private void OnValidate()
    {   
        // reset the editor instance
        editors.Clear();
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        foreach (var item in target.GetType()
            .GetMembers()
            .Where((itemLinq) =>
            {
                if (!(itemLinq is PropertyInfo || itemLinq is FieldInfo))
                    return false;

                //Los 2 poseen propiedades y metodos parecidos

                if (typeof(ScriptableObject).IsAssignableFrom(GetUnderlyingType(itemLinq)))
                    return true;

                var aux = GetUnderlyingType(itemLinq)?.GetInterfaces().Where((inter) => inter.FullName.Contains(typeof(IEnumerable<>).FullName)).FirstOrDefault();
                
                if (aux!=null)
                {
                    foreach (var arrayType in aux.GetGenericArguments())
                    {
                        if (typeof(ScriptableObject).IsAssignableFrom(arrayType))
                            return true;
                    }
                }
                
                return false;
            }))
        {

            if (GetUnderlyingType(item).GetInterfaces().Where((inter) => inter.FullName.Contains(typeof(IEnumerable<>).FullName)).FirstOrDefault()==null && !editors.ContainsKey(item.Name))
            {
                editors.Add(item.Name, Editor.CreateEditor(GetValue(item, target) as Object));
            }
            else if(!editors.ContainsKey(item.Name))//en caso que sea una coleccion la recorro
            {
                int i = 0;

                var collection = GetValue(item, target) as IEnumerable;
                if(collection!=null)
                    foreach (var itemInArray in collection)
                    {
                        if (!editors.ContainsKey(item.Name + ": " + i))
                        {
                            editors.Add(item.Name + ": " + i, Editor.CreateEditor(itemInArray as Object));
                        }
                        i++;
                    }
            }
        }

        //Imprimo los resultados si es que hay

        if (editors.Count == 0)
            return;

        GUIStyle style = new GUIStyle(GUI.skin.label);

        style.richText = true;

        GUILayout.Space(30);

        if (GUILayout.Button("Scriptables"))
        {
            button = !button;
            editors.Clear();
        }

        if (!button)
        {
            return;
        }

        scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.Height(300));

        foreach (var item in editors)
        {
            //if (item.PropertyType.IsAssignableFrom(typeof(ScriptableObject)))
            GUILayout.Label(item.Key.RichText("size",20.ToString()), style);
            
            item.Value?.DrawDefaultInspector();

            GUILayout.Space(10);

            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        }

        GUILayout.EndScrollView();
    }

    //Obtengo el tipo de la variable dependiendo del tipo de miembro
    public object GetValue(MemberInfo member, object obj)
    {
        switch (member.MemberType)
        {
            case MemberTypes.Field:
                return ((FieldInfo)member).GetValue(obj);
            case MemberTypes.Property:
                return ((PropertyInfo)member).GetValue(obj);
            default:
                throw new System.ArgumentException
                (
                 "Input MemberInfo must be if type FieldInfo, or PropertyInfo"
                );
        }
    }

    //Obtengo el tipo de la variable dependiendo del tipo de miembro
    public System.Type GetUnderlyingType(MemberInfo member)
    {
        switch (member.MemberType)
        {
            case MemberTypes.Event:
                return ((EventInfo)member).EventHandlerType;
            case MemberTypes.Field:
                return ((FieldInfo)member).FieldType;
            case MemberTypes.Method:
                return ((MethodInfo)member).ReturnType;
            case MemberTypes.Property:
                return ((PropertyInfo)member).PropertyType;
            default:
                throw new System.ArgumentException
                (
                 "Input MemberInfo must be if type EventInfo, FieldInfo, MethodInfo, or PropertyInfo"
                );
        }
    }
}
*/