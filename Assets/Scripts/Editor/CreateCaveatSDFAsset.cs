using UnityEditor;
using UnityEngine;
using UnityEngine.TextCore.LowLevel;
using TMPro;

#if UNITY_EDITOR
public static class CreateCaveatSDFAsset
{
    private const string TTF_PATH   = "Assets/Fonts/Caveat-Regular.ttf";
    private const string ASSET_PATH = "Assets/Fonts/Caveat-Regular SDF.asset";

    [MenuItem("Tools/Create Caveat SDF Font Asset")]
    public static void Create()
    {
        Font sourceTTF = AssetDatabase.LoadAssetAtPath<Font>(TTF_PATH);
        if (sourceTTF == null)
        {
            Debug.LogError($"[CreateCaveatSDF] Could not load font at {TTF_PATH}.");
            return;
        }

        TMP_FontAsset fontAsset = TMP_FontAsset.CreateFontAsset(
            sourceTTF,
            samplingPointSize       : 90,
            atlasPadding            : 9,
            renderMode              : GlyphRenderMode.SDFAA,
            atlasWidth              : 1024,
            atlasHeight             : 1024,
            atlasPopulationMode     : AtlasPopulationMode.Dynamic,
            enableMultiAtlasSupport : false
        );

        if (fontAsset == null)
        {
            Debug.LogError("[CreateCaveatSDF] TMP_FontAsset.CreateFontAsset returned null.");
            return;
        }

        string charSequence = "";
        for (int c = 32; c <= 126; c++)
            charSequence += (char)c;

        string missingChars;
        fontAsset.TryAddCharacters(charSequence, out missingChars, false);
        if (!string.IsNullOrEmpty(missingChars))
            Debug.LogWarning($"[CreateCaveatSDF] Missing glyphs: {missingChars}");

        fontAsset.name = "Caveat-Regular SDF";

        var existing = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(ASSET_PATH);
        if (existing != null)
            AssetDatabase.DeleteAsset(ASSET_PATH);

        AssetDatabase.CreateAsset(fontAsset, ASSET_PATH);

        if (fontAsset.atlasTextures != null)
        {
            foreach (var tex in fontAsset.atlasTextures)
            {
                if (tex != null && !AssetDatabase.IsSubAsset(tex))
                {
                    tex.name = "Caveat-Regular SDF Atlas";
                    AssetDatabase.AddObjectToAsset(tex, fontAsset);
                }
            }
        }

        if (fontAsset.material != null && !AssetDatabase.IsSubAsset(fontAsset.material))
        {
            fontAsset.material.name = "Caveat-Regular SDF Material";
            AssetDatabase.AddObjectToAsset(fontAsset.material, fontAsset);
        }

        EditorUtility.SetDirty(fontAsset);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"[CreateCaveatSDF] Saved to {ASSET_PATH} — glyphs={fontAsset.glyphTable.Count}");
    }
}
#endif
