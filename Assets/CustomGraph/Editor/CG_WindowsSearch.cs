using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace CustomGraph.Editor
{
    public class CG_WindowsSearch : ScriptableObject, ISearchWindowProvider
    {
        public CG_GraphView Graph;
        public VisualElement Item;
        public static List<SearchContextElem> Elements;

        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            List<SearchTreeEntry> tree = new();
            tree.Add(new SearchTreeGroupEntry(new GUIContent("Nodes"), 0));

            Elements = new();

            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (Assembly a in assemblies)
            {
                foreach (Type t in a.GetTypes())
                {
                    if (t.CustomAttributes.ToList() == null) continue;

                    var attribute = t.GetCustomAttribute(typeof(InfoAttribute));

                    if (attribute == null) continue;

                    InfoAttribute att = (InfoAttribute)attribute;
                    var node = Activator.CreateInstance(t);

                    if (string.IsNullOrEmpty(att.MenuItem)) continue;

                    Elements.Add(new SearchContextElem(node, att.MenuItem));
                }
            }

            Elements.Sort((x, y) =>
            {
                string[] splits1 = x.Title.Split('/');
                string[] splits2 = y.Title.Split('/');

                for (int i = 0; i < splits1.Length; i++)
                {
                    if (i >= splits2.Length) return 1;

                    int val = splits1[i].CompareTo(splits2[i]);

                    if (val != 0)
                    {
                        if (splits1.Length != splits2.Length && (i == splits1.Length - 1 || i == splits2.Length - 1))
                            return splits1.Length < splits2.Length ? 1 : -1;
                        return val;
                    }
                }

                return 0;
            });

            List<string> groups = new();

            foreach (SearchContextElem elem in Elements)
            {
                string[] title = elem.Title.Split('/');
                string groupName = "";

                for (int i = 0; i < title.Length - 1; i++)
                {
                    groupName += title[i];

                    if (!groups.Contains(groupName))
                    {
                        tree.Add(new SearchTreeGroupEntry(new GUIContent(title[i]), i + 1));
                        groups.Add(groupName);
                    }

                    groupName += "/";
                }

                SearchTreeEntry entry = new(new GUIContent(title.Last()));
                entry.level = title.Length;
                entry.userData = new SearchContextElem(elem.Item, elem.Title);
                tree.Add(entry);
            }

            return tree;
        }

        public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
        {
            var windowMousePos = Graph.ChangeCoordinatesTo(Graph, context.screenMousePosition - Graph.Window.position.position);
            var graphMousePos = Graph.contentViewContainer.WorldToLocal(windowMousePos);

            SearchContextElem elem = (SearchContextElem)SearchTreeEntry.userData;
            CG_Node node = (CG_Node)elem.Item;
            node.SetRectPosition(new Rect(graphMousePos, new Vector2()));
            Graph.Add(node);

            return true;
        }
    }

    public struct SearchContextElem
    {
        public string Title { get; private set; }
        public object Item { get; private set; }

        public SearchContextElem(object item, string title)
        {
            Title = title;
            Item = item;
        }

    }
}
