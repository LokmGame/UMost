using System;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using static UMost.Editor.EditorDefine;

namespace UMost.Editor
{
    public class UMostWindow : EditorWindow
    {
        private int currentTab = 0;
        private GUIContent[] tabContents;

        private Dictionary<EWindowType, ResEditor> _resEditor = new Dictionary<EWindowType, ResEditor>();
        private Dictionary<EWindowType, object> _editorWindows = new Dictionary<EWindowType, object>();

        [MenuItem("UMost/Open Window")]
        public static void ShowWindow()
        {
            UMostWindow window = GetWindow<UMostWindow>("UMost资源工具合集", true);
            window.minSize = new Vector2(1000, 700);
        }

        private void OnEnable()
        {
            Type utilityType = typeof(UMostEditorUtility);
            FieldInfo[] fields = utilityType.GetFields(BindingFlags.Public | BindingFlags.Static);
            foreach (FieldInfo field in fields)
            {
                if (field.FieldType == typeof(ResEditor))
                {
                    ResEditor resEditor = (ResEditor)field.GetValue(null);
                    _resEditor.Add(resEditor.EnumType, resEditor);
                }
            }

            // Create tab contents with icons
            tabContents = new GUIContent[_resEditor.Count];
            foreach (ResEditor editor in _resEditor.Values)
            {
                object instance = CreateInstance(editor.EditorType);
                _editorWindows.Add(editor.EnumType, instance);
                MethodInfo method = editor.EditorType.GetMethod("OnInit");
                if (method != null)
                {
                    method.Invoke(instance, new object[] { editor.ToolTabName, editor.ToolFuncName });
                }

                Texture2D icon = EditorGUIUtility.IconContent(editor.TabIcon).image as Texture2D;
                // Reduce the icon size by creating a new GUIContent with smaller image
                GUIContent tabContent = new GUIContent(editor.TabName, ScaleTexture(icon, 16, 16));
                tabContents[(int)editor.EnumType] = tabContent;
            }
        }

        // Utility method to scale the texture
        private Texture2D ScaleTexture(Texture2D texture, int targetWidth, int targetHeight)
        {
            RenderTexture rt = new RenderTexture(targetWidth, targetHeight, 24);
            RenderTexture.active = rt;
            Graphics.Blit(texture, rt);
            Texture2D result = new Texture2D(targetWidth, targetHeight);
            result.ReadPixels(new Rect(0, 0, targetWidth, targetHeight), 0, 0);
            result.Apply();
            return result;
        }

        private void OnGUI()
        {
            GUILayout.Space(10);

            EditorGUILayout.BeginHorizontal();

            // Left column for tabs with a slightly darker background
            EditorGUILayout.BeginVertical("RegionBg", GUILayout.Width(position.width * 0.2f));
            GUILayout.Space(-10);
            DrawTabs();
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical();
            DrawToolContent();
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();
        }

        private void DrawTabs()
        {
            for (int i = 0; i < tabContents.Length; i++)
            {
                if (GUILayout.Toggle(currentTab == i, tabContents[i], "Button"))
                {
                    currentTab = i;
                }
                GUILayout.Space(5);
            }
        }

        private void DrawToolContent()
        {
            EWindowType eCurType = (EWindowType)currentTab;
            object curEditor = _editorWindows[eCurType];
            ResEditor curResEditor = _resEditor[eCurType];

            MethodInfo method = curResEditor.EditorType.GetMethod("DrawTab");
            if (method != null)
            {
                method.Invoke(curEditor, null);
            }
        }
    }
}