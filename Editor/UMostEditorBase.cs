using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace UMost.Editor
{
    public abstract class UMostEditorBase : EditorWindow
    {
        protected int toolTab = 0;
        protected GUIContent[] toolTabs;
        protected string[] toolFuncNames;

        public void OnInit(string[] names, string[] funcNames)
        {
            toolFuncNames = funcNames;
            toolTabs = new GUIContent[names.Length];
            for (int i = 0; i < names.Length; i++)
            {
                toolTabs[i] = new GUIContent(names[i]);
            }
        }
            
        public void DrawTab()
        {
            if (toolFuncNames.Length > 0)
            {
                DrawHead();

                MethodInfo method = this.GetType().GetMethod(toolFuncNames[toolTab]);
                if (method != null)
                {
                    method.Invoke(this, null);
                }

                DrawEnd();
            }
            else
            {
                GUILayout.Label("No tools available at this time");
            }
        }

        protected void DrawHead()
        {
            EditorGUILayout.BeginVertical();
            toolTab = GUILayout.Toolbar(toolTab, toolTabs);
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical("RegionBg");
            GUILayout.Space(-15);
        }

        protected void DrawEnd()
        {
            EditorGUILayout.EndVertical();
        }
    }
}