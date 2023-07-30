using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace UMost.Editor
{
    public partial class UMostTextureEditor
    {
        private string targetFolderPath = "Assets/";
        private Vector2 scrollPosition;
        private bool showPlatformFormats = false;
        private BuildTarget[] validBuildTargets;

        public void FindTexturesWithFormat_OnGUI()
        {
            GUILayout.Label("Find Textures with Format", EditorStyles.boldLabel);

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

            if (GUILayout.Button("Find Textures with Format"))
            {
                FindTexturesWithTargetFormat();
                showPlatformFormats = true;
            }

            if (showPlatformFormats)
            {
                GUILayout.Label("Textures Formats for Each Platform:", EditorStyles.boldLabel);

                // Begin the scroll view (Unity 2019 or later)
#if UNITY_2019_1_OR_NEWER
                scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.ExpandHeight(true));
#else
                scrollPosition = GUILayout.BeginScrollView(scrollPosition, EditorStyles.helpBox);
#endif

                Texture2D[] textures = FindTexturesInFolder(targetFolderPath);
                foreach (Texture2D texture in textures)
                {
                    GUILayout.Label(texture.name, EditorStyles.boldLabel);

                    TextureImporter importer = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(texture)) as TextureImporter;
                    if (importer != null)
                    {
                        EditorGUILayout.BeginHorizontal();

                        foreach (BuildTarget buildTarget in validBuildTargets)
                        {
                            EditorGUILayout.BeginHorizontal("RL Background");

                            BuildTargetGroup buildTargetGroup = BuildPipeline.GetBuildTargetGroup(buildTarget);
                            string platformName = buildTarget.ToString();
                            GUILayout.Label(platformName);
                            TextureImporterPlatformSettings platformSetting = importer.GetPlatformTextureSettings(platformName);
                            if (platformSetting != null)
                            {
                                EditorGUILayout.EnumPopup(platformSetting.format);
                            }
                            else
                            {
                                EditorGUILayout.LabelField("Not Set");
                            }

                            EditorGUILayout.EndHorizontal();
                        }

                        EditorGUILayout.EndHorizontal();
                    }

                    EditorGUILayout.Space();
                }

                // End the scroll view
                GUILayout.EndScrollView();
            }
        }

        private Texture2D[] FindTexturesInFolder(string folderPath)
        {
            string[] guids = AssetDatabase.FindAssets("t:Texture2D", new string[] { folderPath });
            Texture2D[] textures = new Texture2D[guids.Length];
            for (int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                textures[i] = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            }
            return textures;
        }

        private void FindTexturesWithTargetFormat()
        {
            validBuildTargets = GetValidBuildTargets();
        }

        private BuildTarget[] GetValidBuildTargets()
        {
            var validTargets = new List<BuildTarget>();
            foreach (BuildTarget target in System.Enum.GetValues(typeof(BuildTarget)))
            {
                if (BuildPipeline.IsBuildTargetSupported(BuildPipeline.GetBuildTargetGroup(target), target))
                {
                    validTargets.Add(target);
                }
            }
            return validTargets.ToArray();
        }

        public void FindTexturesWithFormat_Clear()
        {
            showPlatformFormats = false;
        }
    }
}
