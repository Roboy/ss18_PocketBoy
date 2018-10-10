using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class HologramDissolveShaderGUI : ShaderGUI {

    Material m_Material;
    MaterialProperty[] m_Properties;
    MaterialEditor m_MaterialEditor;

    // Albedo
    private MaterialProperty Albedo = null;
    private MaterialProperty AlbedoColor = null;
    private MaterialProperty HologramColor = null;
    private MaterialProperty Brightness = null;
    private MaterialProperty Alpha = null;
    private MaterialProperty Direction = null;

    // Rim
    private MaterialProperty RimColor = null;
    private MaterialProperty RimPower = null;

    // Scanlines
    private MaterialProperty ScanSpeed = null;
    private MaterialProperty ScanTiling = null;

    // Glow
    private MaterialProperty GlowSpeed = null;
    private MaterialProperty GlowTiling = null;

    // Glitch
    private MaterialProperty GlitchSpeed = null;
    private MaterialProperty GlitchIntensity = null;

    // Flicker
    private MaterialProperty Flicker = null;
    private MaterialProperty FlickerSpeed = null;

    // Dissolve
    private MaterialProperty DissolveValue = null;
    private MaterialProperty DissolveTransitionSize = null;
    //private MaterialProperty DissolveStart = null;
    //private MaterialProperty DissolveEnd = null;

    private static class Styles
    {
        public static GUIContent AlbedoText = new GUIContent("Albedo");
        public static GUIContent FlickerText = new GUIContent("Flicker Mask");
    }

    enum Category
    {
        General = 0,
        Effects,
        Dissolve,
    }

    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        m_Material = materialEditor.target as Material;
        m_Properties = properties;
        m_MaterialEditor = materialEditor;

        AssignProperties();

        Layout.Initialize(m_Material);

        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(-7);
        EditorGUILayout.BeginVertical();
        EditorGUI.BeginChangeCheck();
        DrawGUI();
        EditorGUILayout.EndVertical();
        GUILayout.Space(1);
        EditorGUILayout.EndHorizontal();

        Undo.RecordObject(m_Material, "Material Edition");
    }

    void AssignProperties()
    {
        Albedo = FindProperty("_MainTex", m_Properties);
        AlbedoColor = FindProperty("_MainColor", m_Properties);
        HologramColor = FindProperty("_HologramColor", m_Properties);
        Brightness = FindProperty("_Brightness", m_Properties);
        Alpha = FindProperty("_Alpha", m_Properties);
        Direction = FindProperty("_Direction", m_Properties);

        RimColor = FindProperty("_RimColor", m_Properties);
        RimPower = FindProperty("_RimPower", m_Properties);

        ScanSpeed = FindProperty("_ScanSpeed", m_Properties);
        ScanTiling = FindProperty("_ScanTiling", m_Properties);

        GlowSpeed = FindProperty("_GlowSpeed", m_Properties);
        GlowTiling = FindProperty("_GlowTiling", m_Properties);

        GlitchSpeed = FindProperty("_GlitchSpeed", m_Properties);
        GlitchIntensity = FindProperty("_GlitchIntensity", m_Properties);

        Flicker = FindProperty("_FlickerTex", m_Properties);
        FlickerSpeed = FindProperty("_FlickerSpeed", m_Properties);

        DissolveValue = FindProperty("_DissolveValue", m_Properties);
        DissolveTransitionSize = FindProperty("_DissolveTransitionSize", m_Properties);
        //DissolveStart = FindProperty("_DissolveStart", m_Properties);
        //DissolveEnd = FindProperty("_DissolveEnd", m_Properties);
    }

    void DrawGUI()
    {
        if (Layout.BeginFold((int)Category.General, "- Surface -"))
            DrawGeneralSettings();
        Layout.EndFold();

        if (Layout.BeginFold((int)Category.Effects, "- Effects -"))
        {
            DrawGeneralEffect();
            DrawRimSettings();
            DrawScanlinesSettings();
            DrawGlowSettings();
            DrawGlitchSettings();
            DrawFlickerSettings();
        }
        Layout.EndFold();

        if (Layout.BeginFold((int)Category.Dissolve, " - Dissolve -"))
        {
            DrawDissolveSettings();
        }
        Layout.EndFold();
    }

    void DrawGeneralSettings()
    {
        GUILayout.Space(-3);
        EditorGUI.indentLevel++;
        var ofs = EditorGUIUtility.labelWidth;
        m_MaterialEditor.SetDefaultGUIWidths();
        EditorGUIUtility.labelWidth = 0;
        m_MaterialEditor.TexturePropertySingleLine(Styles.AlbedoText, Albedo, AlbedoColor, HologramColor);
        EditorGUIUtility.labelWidth = ofs;
        m_MaterialEditor.ShaderProperty(Brightness, "Brightness");
        m_MaterialEditor.ShaderProperty(Alpha, "Alpha");
        EditorGUI.indentLevel--;
    }

    void DrawGeneralEffect()
    {
        GUILayout.Space(-3);
        GUILayout.Label("General", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;
        var ofs = EditorGUIUtility.labelWidth;
        m_MaterialEditor.SetDefaultGUIWidths();
        m_MaterialEditor.ShaderProperty(Direction, "Direction");
        EditorGUIUtility.labelWidth = ofs;
        EditorGUI.indentLevel--;
    }

    void DrawRimSettings()
    {
        GUILayout.Space(-3);
        GUILayout.Label("Rim Light", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;
        var ofs = EditorGUIUtility.labelWidth;
        m_MaterialEditor.SetDefaultGUIWidths();
        m_MaterialEditor.ShaderProperty(RimColor, "Color");
        m_MaterialEditor.ShaderProperty(RimPower, "Power");
        EditorGUIUtility.labelWidth = ofs;
        EditorGUI.indentLevel--;
    }

    void DrawScanlinesSettings()
    {
        GUILayout.Space(-3);
        GUILayout.Label("Scanlines", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;

        bool toggle = Array.IndexOf(m_Material.shaderKeywords, "_SCAN_ON") != -1;
        EditorGUI.BeginChangeCheck();
        toggle = EditorGUILayout.Toggle("Enable", toggle);
        if (EditorGUI.EndChangeCheck())
        {
            if (toggle)
                m_Material.EnableKeyword("_SCAN_ON");
            else
                m_Material.DisableKeyword("_SCAN_ON");
        }

        var ofs = EditorGUIUtility.labelWidth;
        m_MaterialEditor.SetDefaultGUIWidths();
        m_MaterialEditor.ShaderProperty(ScanSpeed, "Speed");
        m_MaterialEditor.ShaderProperty(ScanTiling, "Tiling");
        EditorGUIUtility.labelWidth = ofs;
        EditorGUI.indentLevel--;
    }

    void DrawGlowSettings()
    {
        GUILayout.Space(-3);
        GUILayout.Label("Glow", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;

        bool toggle = Array.IndexOf(m_Material.shaderKeywords, "_GLOW_ON") != -1;
        EditorGUI.BeginChangeCheck();
        toggle = EditorGUILayout.Toggle("Enable", toggle);
        if (EditorGUI.EndChangeCheck())
        {
            if (toggle)
                m_Material.EnableKeyword("_GLOW_ON");
            else
                m_Material.DisableKeyword("_GLOW_ON");
        }

        var ofs = EditorGUIUtility.labelWidth;
        m_MaterialEditor.SetDefaultGUIWidths();
        m_MaterialEditor.ShaderProperty(GlowSpeed, "Speed");
        m_MaterialEditor.ShaderProperty(GlowTiling, "Tiling");
        EditorGUIUtility.labelWidth = ofs;
        EditorGUI.indentLevel--;
    }

    void DrawGlitchSettings()
    {
        GUILayout.Space(-3);
        GUILayout.Label("Glitch", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;

        bool toggle = Array.IndexOf(m_Material.shaderKeywords, "_GLITCH_ON") != -1;
        EditorGUI.BeginChangeCheck();
        toggle = EditorGUILayout.Toggle("Enable", toggle);
        if (EditorGUI.EndChangeCheck())
        {
            if (toggle)
                m_Material.EnableKeyword("_GLITCH_ON");
            else
                m_Material.DisableKeyword("_GLITCH_ON");
        }

        var ofs = EditorGUIUtility.labelWidth;
        m_MaterialEditor.SetDefaultGUIWidths();
        m_MaterialEditor.ShaderProperty(GlitchSpeed, "Speed");
        m_MaterialEditor.ShaderProperty(GlitchIntensity, "Intensity");
        EditorGUIUtility.labelWidth = ofs;
        EditorGUI.indentLevel--;
    }

    void DrawFlickerSettings()
    {
        GUILayout.Space(-3);
        GUILayout.Label("Flicker", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;
        var ofs = EditorGUIUtility.labelWidth;
        m_MaterialEditor.SetDefaultGUIWidths();
        EditorGUIUtility.labelWidth = 0;
        m_MaterialEditor.TexturePropertySingleLine(Styles.FlickerText, Flicker, null);
        EditorGUIUtility.labelWidth = ofs;
        m_MaterialEditor.ShaderProperty(FlickerSpeed, "Speed");
        EditorGUI.indentLevel--;
    }

    void DrawDissolveSettings()
    {
        GUILayout.Space(-3);
        GUILayout.Label("Dissolve", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;
        m_MaterialEditor.ShaderProperty(DissolveValue, "Value");
        m_MaterialEditor.ShaderProperty(DissolveTransitionSize, "Transition size");
        //m_MaterialEditor.ShaderProperty(DissolveStart, "Start");
        //m_MaterialEditor.ShaderProperty(DissolveEnd, "End");
        EditorGUI.indentLevel--;
    }

}
