using System;
using static UMost.Editor.EditorDefine;

namespace UMost.Editor
{
    public struct ResEditor
    {
        public string TabName;
        public string TabIcon;
        public Type EditorType;
        public EWindowType EnumType;
        public string[] ToolTabName;
        public string[] ToolFuncName;
    }

    public static class UMostEditorUtility
    {
        public static ResEditor MaterialEditor = new ResEditor()
        {
            TabName = "材质",
            TabIcon = "Material Icon",
            EditorType = typeof(UMostMaterialEditor),
            EnumType = EWindowType.Material,
            ToolTabName = new string[] { "查找指定着色器使用的材质" },
            ToolFuncName = new string[] { "FindMaterialUseShader_OnGUI" }
        };

        public static ResEditor ModelEditor = new ResEditor()
        {
            TabName = "模型",
            TabIcon = "Mesh Icon",
            EditorType = typeof(UMostModellEditor),
            EnumType = EWindowType.Model,
            ToolTabName = new string[] { "查看模型面数" },
            ToolFuncName = new string[] { "ModelFaceCount_OnGUI" }
        };

        public static ResEditor TextureEditor = new ResEditor()
        {
            TabName = "贴图",
            TabIcon = "Texture Icon",
            EditorType = typeof(UMostTextureEditor),
            EnumType = EWindowType.Texture,
            ToolTabName = new string[] { "查找指定格式贴图" },
            ToolFuncName = new string[] { "FindTexturesWithFormat_OnGUI" }
        };

        public static ResEditor ParticleEditor = new ResEditor()
        {
            TabName = "特效",
            TabIcon = "ParticleSystem Icon",
            EditorType = typeof(UMostParticleEditor),
            EnumType = EWindowType.Particle,
            ToolTabName = new string[] { },
            ToolFuncName = new string[] { },
        };

        public static ResEditor ShaderEditor = new ResEditor()
        {
            TabName = "着色器",
            TabIcon = "Shader Icon",
            EditorType = typeof(UMostShaderEditor),
            EnumType = EWindowType.Shader,
            ToolTabName = new string[] { },
            ToolFuncName = new string[] { },
        };
    }
}