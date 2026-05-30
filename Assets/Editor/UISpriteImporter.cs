using UnityEditor;

/// <summary>
/// Asegura que las texturas en Resources/UI/ se importen como Sprite (2D and UI).
/// Se ejecuta automáticamente al importar assets.
/// </summary>
public class UISpriteImporter : AssetPostprocessor
{
    private void OnPreprocessTexture()
    {
        if (!assetPath.Contains("Resources/UI/"))
        {
            return;
        }

        TextureImporter importer = (TextureImporter)assetImporter;
        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Single;
        importer.mipmapEnabled = false;
        importer.filterMode = UnityEngine.FilterMode.Bilinear;
        importer.textureCompression = TextureImporterCompression.Compressed;

        // Enable border for sliced sprites (buttons, panels)
        if (assetPath.Contains("Buttons/") || assetPath.Contains("Panels/") || assetPath.Contains("Shop/"))
        {
            importer.spriteBorder = new UnityEngine.Vector4(8, 8, 8, 8);
        }
    }
}
