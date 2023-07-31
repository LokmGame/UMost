using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace UMost.Editor
{
    public partial class UMostShaderEditor : UMostEditorBase
    {
        private string targetFolderPath = "Assets/";
        private List<Shader> shadersInFolder = new List<Shader>();
        private Vector2 scrollPosition;

        public void ShowShadersInFolder_OnGUI()
        {
            GUILayout.Label("Show Shaders in Folder", EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();
            targetFolderPath = EditorGUILayout.TextField("Target Folder Path", targetFolderPath);
            if (GUILayout.Button("Select Folder", GUILayout.Width(100)))
            {
                string folderPath = EditorUtility.OpenFolderPanel("Select Target Folder", targetFolderPath, "");
                if (!string.IsNullOrEmpty(folderPath))
                {
                    targetFolderPath = "Assets" + folderPath.Substring(Application.dataPath.Length);
                }
            }
            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("Show Shaders"))
            {
                ShowShadersInFolder(targetFolderPath);
            }

            if (shadersInFolder.Count > 0)
            {
                GUILayout.Label("Shaders in Folder:", EditorStyles.boldLabel);

                // Begin the scroll view (Unity 2019 or later)
#if UNITY_2019_1_OR_NEWER
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.ExpandHeight(true));
#else
                scrollPosition = GUILayout.BeginScrollView(scrollPosition);
#endif

                foreach (Shader shader in shadersInFolder)
                {
                    GUILayout.BeginHorizontal();
                    EditorGUILayout.ObjectField(shader, typeof(Shader), false);

                    // Get the number of properties in the shader
                    int propertyCount = ShaderUtil.GetPropertyCount(shader);
                    // Display the property count next to the shader name
                    GUILayout.Label($"Properties: {propertyCount}");

                    GUILayout.EndHorizontal();
                }

                // End the scroll view
                GUILayout.EndScrollView();
            }
            else if (shadersInFolder.Count == 0)
            {
                GUILayout.Label("No shaders found in the specified folder", EditorStyles.boldLabel);
            }
        }

        private void ShowShadersInFolder(string folderPath)
        {
            string[] guids = AssetDatabase.FindAssets("t:Shader", new string[] { folderPath });
            shadersInFolder.Clear();

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                Shader shader = AssetDatabase.LoadAssetAtPath<Shader>(path);
                if (shader != null)
                {
                    shadersInFolder.Add(shader);
                }
            }
        }
    }
}
