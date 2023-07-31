using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

namespace UMost.Editor
{
    public partial class UMostShaderEditor : UMostEditorBase
    {
        private string targetFolderPath = "Assets/";
        private List<Shader> shadersInFolder = new List<Shader>();
        private Vector2 scrollPosition;

        private int redThreshold = 5;

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

            // 在面板上显示并修改红色阈值
            redThreshold = EditorGUILayout.IntField("Max Count", redThreshold);

            if (GUILayout.Button("Show Shaders"))
            {
                ShowShadersInFolder(targetFolderPath);
            }

            if (shadersInFolder.Count > 0)
            {
                if (GUILayout.Button("Export CSV"))
                {
                    ExportCSV();
                }

                GUILayout.Label("Shaders in Folder:", EditorStyles.boldLabel);

                // Begin the scroll view (Unity 2019 or later)
#if UNITY_2019_1_OR_NEWER
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.ExpandHeight(true));
#else
                scrollPosition = GUILayout.BeginScrollView(scrollPosition);
#endif

                foreach (Shader shader in shadersInFolder)
                {
                    // Get the number of properties in the shader
                    int propertyCount = ShaderUtil.GetPropertyCount(shader);
                    if (propertyCount >= redThreshold)
                    {
                        GUILayout.BeginHorizontal();
                        EditorGUILayout.ObjectField(shader, typeof(Shader), false);

                        // Display the property count next to the shader name
                        GUILayout.Label($"Properties: {propertyCount}");

                        GUILayout.EndHorizontal();
                    }
                }

                // End the scroll view
                GUILayout.EndScrollView();
            }
            else if (shadersInFolder.Count == 0)
            {
                GUILayout.Label("No shaders found in the specified folder", EditorStyles.boldLabel);
            }
        }

        private void ExportCSV()
        {
            string csvPath = EditorUtility.SaveFilePanel("Export CSV", "", "ShaderIndex.csv", "csv");
            if (string.IsNullOrEmpty(csvPath))
            {
                // 用户取消了导出操作
                return;
            }

            // 创建一个字符串列表用于存储 CSV 数据
            List<string> csvData = new List<string>();
            csvData.Add("Shader Name,Property Count");

            foreach (Shader shader in shadersInFolder)
            {
                // 获取 Shader 名字和属性个数
                string shaderName = shader.name;
                int propertyCount = ShaderUtil.GetPropertyCount(shader);

                // 将数据添加到 CSV 列表中
                csvData.Add($"{shaderName},{propertyCount}");
            }

            // 将 CSV 数据写入文件
            File.WriteAllLines(csvPath, csvData.ToArray());

            // 在 Unity 编辑器中刷新文件，确保导出的 CSV 在 Project 面板中可见
            AssetDatabase.Refresh();

            Debug.Log($"CSV exported to: {csvPath}");
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
