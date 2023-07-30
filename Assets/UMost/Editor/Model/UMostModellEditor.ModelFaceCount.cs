using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace UMost.Editor
{
    public partial class UMostModellEditor
    {
        private string targetFolderPath = "Assets/";
        private Vector2 scrollPosition;
        private bool show = false;
        private float progress = 0f; // Current progress for the progress bar

        public void ModelFaceCount_OnGUI()
        {
            GUILayout.Label("Model Face Count Tool", EditorStyles.boldLabel);

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

            if (GUILayout.Button("Get Model Face Count"))
            {
                GetModelFaceCount();
                show = true;
            }

            if (show)
            {
                GUILayout.Label("Model Face Count:", EditorStyles.boldLabel);

                // Begin the scroll view (Unity 2019 or later)
#if UNITY_2019_1_OR_NEWER
                scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.ExpandHeight(true));
#else
                scrollPosition = GUILayout.BeginScrollView(scrollPosition, EditorStyles.helpBox);
#endif

                DisplayModelFaceCounts();

                // End the scroll view
                GUILayout.EndScrollView();
            }
        }

        private void GetModelFaceCount()
        {
            Dictionary<string, int> modelFaceCounts = new Dictionary<string, int>();
            string[] guids = AssetDatabase.FindAssets("t:Model", new string[] { targetFolderPath });

            // Reset progress before starting the search
            progress = 0f;

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                ModelImporter modelImporter = AssetImporter.GetAtPath(path) as ModelImporter;
                if (modelImporter != null)
                {
                    ModelImporterClipAnimation[] clipAnimations = modelImporter.defaultClipAnimations;
                    int totalFaceCount = 0;
                    foreach (ModelImporterClipAnimation clipAnimation in clipAnimations)
                    {
                        totalFaceCount += GetFaceCount(path, clipAnimation.name);
                    }
                    modelFaceCounts.Add(path, totalFaceCount);
                }

                // Update progress for the progress bar
                progress = (float)(ArrayUtility.IndexOf(guids, guid) + 1) / guids.Length;
                EditorUtility.DisplayProgressBar("Finding Model Face Count", "Processing...", progress);
            }

            // Hide the progress bar when the search is complete
            EditorUtility.ClearProgressBar();
        }

        private int GetFaceCount(string modelPath, string animationName)
        {
            GameObject modelPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(modelPath);
            if (modelPrefab != null)
            {
                int totalFaceCount = 0;
                MeshFilter[] meshFilters = modelPrefab.GetComponentsInChildren<MeshFilter>();
                foreach (MeshFilter meshFilter in meshFilters)
                {
                    if (meshFilter.sharedMesh != null)
                    {
                        int faceCount = meshFilter.sharedMesh.triangles.Length / 3;
                        totalFaceCount += faceCount;
                    }
                }
                return totalFaceCount;
            }
            return 0;
        }

        private void DisplayModelFaceCounts()
        {
            string[] guids = AssetDatabase.FindAssets("t:Model", new string[] { targetFolderPath });
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                int faceCount = 0;
                if (path.EndsWith(".fbx") || path.EndsWith(".FBX"))
                {
                    faceCount = GetFaceCount(path, "");
                }

                GUILayout.Label($"{path}: {faceCount} faces");
            }
        }

        public void ModelFaceCount_Clear()
        {

        }
    }
}
