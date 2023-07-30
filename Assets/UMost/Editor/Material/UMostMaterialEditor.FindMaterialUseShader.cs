using UnityEngine;
using UnityEditor;

namespace UMost.Editor
{
    public partial class UMostMaterialEditor
    {
        private Shader targetShader;
        private Material[] materialsUsingShader;
        private Vector2 scrollPosition;

        public void FindMaterialUseShader_OnGUI()
        {
            GUILayout.Label("Find Material Use Shader", EditorStyles.boldLabel);

            targetShader = (Shader)EditorGUILayout.ObjectField("Target Shader", targetShader, typeof(Shader), false);

            if (GUILayout.Button("查找"))
            {
                FindMaterialsUsingShader();
            }

            // Check if there are materials to display
            if (materialsUsingShader != null && materialsUsingShader.Length > 0)
            {
                GUILayout.Label("所有使用该着色器的材质:", EditorStyles.boldLabel);

                // Begin the scroll view (Unity 2019 or later)
#if UNITY_2019_1_OR_NEWER
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.ExpandHeight(true));
#else
                scrollPosition = GUILayout.BeginScrollView(scrollPosition);
#endif

                foreach (Material material in materialsUsingShader)
                {
                    EditorGUILayout.ObjectField(material, typeof(Material), false);
                }

                // End the scroll view
                GUILayout.EndScrollView();
            }
            else if (materialsUsingShader != null && materialsUsingShader.Length == 0)
            {
                GUILayout.Label("没有材质使用该着色器", EditorStyles.boldLabel);
            }
        }

        private void FindMaterialsUsingShader()
        {
            if (targetShader == null)
            {
                Debug.LogError("Target Shader is not assigned!");
                return;
            }

            string shaderName = targetShader.name;
            string[] guids = AssetDatabase.FindAssets("t:Material"); // Find all Material assets

            materialsUsingShader = new Material[0];

            int totalMaterialCount = guids.Length;
            int processedMaterialCount = 0;

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                Material material = AssetDatabase.LoadAssetAtPath<Material>(path);

                if (material != null && material.shader.name == shaderName)
                {
                    ArrayUtility.Add(ref materialsUsingShader, material);
                }

                // Update progress bar
                processedMaterialCount++;
                float progress = (float)processedMaterialCount / totalMaterialCount;
                EditorUtility.DisplayProgressBar("Finding Materials", "Processing...", progress);
            }

            // Clear progress bar when finished
            EditorUtility.ClearProgressBar();
        }

        public void FindMaterialUseShader_Clear()
        {
            targetShader = null;
            materialsUsingShader = new Material[0];
        }
    }
}