using UnityEngine;
using UnityEditor;
using System.IO;

public class CreateDamageTextTexture : EditorWindow
{
    [MenuItem("Tools/Create Damage Text Texture")]
    static void CreateTexture()
    {
        if (!Directory.Exists("Assets/Textures"))
        {
            Directory.CreateDirectory("Assets/Textures");
        }

        int width = 512;
        int height = 128;
        Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);

        Color[] colors = new Color[width * height];
        for (int i = 0; i < colors.Length; i++)
            colors[i] = Color.clear;

        // Color rojo para el daño con borde más oscuro
        Color textColor = new Color(1f, 0.15f, 0.15f, 1f);
        Color borderColor = new Color(0.6f, 0f, 0f, 1f);

        // Dibujar texto con borde para mejor legibilidad
        string text = "DAÑADO";
        int startX = 30;
        int startY = 40;
        int letterSpacing = 55; // Más espacio entre letras
        int thickness = 5; // Grosor del texto

        // Primero dibujar el borde (más grueso)
        for (int i = 0; i < text.Length; i++)
        {
            int xPos = startX + i * letterSpacing;
            DrawLetterBlock(text[i], xPos, startY, thickness + 2, borderColor, colors, width, height);
        }

        // Luego dibujar el texto encima
        for (int i = 0; i < text.Length; i++)
        {
            int xPos = startX + i * letterSpacing;
            DrawLetterBlock(text[i], xPos, startY, thickness, textColor, colors, width, height);
        }

        texture.SetPixels(colors);
        texture.Apply();

        byte[] bytes = texture.EncodeToPNG();
        string path = Application.dataPath + "/Textures/DamageText.png";
        File.WriteAllBytes(path, bytes);

        AssetDatabase.Refresh();

        string assetPath = "Assets/Textures/DamageText.png";
        TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
        if (importer != null)
        {
            importer.textureType = TextureImporterType.Default;
            importer.alphaIsTransparency = true;
            importer.wrapMode = TextureWrapMode.Clamp;
            importer.filterMode = FilterMode.Bilinear;
            importer.SaveAndReimport();
        }

        Debug.Log("Textura 'DAÑADO' creada en: " + assetPath);

        Texture2D createdTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);
        if (createdTexture != null)
        {
            Selection.activeObject = createdTexture;
            EditorGUIUtility.PingObject(createdTexture);
        }
    }

    static void DrawLetterBlock(char c, int x, int y, int s, Color color, Color[] colors, int width, int height)
    {
        switch (char.ToUpper(c))
        {
            case 'D':
                // Vertical izquierda
                FillRect(x, y, s, s * 6, color, colors, width, height);
                // Superior
                FillRect(x + s, y + s * 5, s * 3, s, color, colors, width, height);
                // Inferior
                FillRect(x + s, y, s * 3, s, color, colors, width, height);
                // Vertical derecha
                FillRect(x + s * 4, y + s, s, s * 4, color, colors, width, height);
                break;

            case 'A':
                // Vertical izquierda
                FillRect(x, y, s, s * 6, color, colors, width, height);
                // Vertical derecha
                FillRect(x + s * 4, y, s, s * 6, color, colors, width, height);
                // Barra superior
                FillRect(x + s, y + s * 5, s * 3, s, color, colors, width, height);
                // Barra media
                FillRect(x + s, y + s * 3, s * 3, s, color, colors, width, height);
                // Pico superior (triangular simplificado)
                FillRect(x + s, y + s * 4, s, s, color, colors, width, height);
                FillRect(x + s * 3, y + s * 4, s, s, color, colors, width, height);
                break;

            case 'Ñ':
                // Vertical izquierda
                FillRect(x, y, s, s * 6, color, colors, width, height);
                // Vertical derecha
                FillRect(x + s * 4, y, s, s * 6, color, colors, width, height);
                // Barra superior
                FillRect(x + s, y + s * 5, s * 3, s, color, colors, width, height);
                // Virgulilla (simplificada)
                FillRect(x + s, y + s * 6, s * 3, s / 2, color, colors, width, height);
                FillRect(x + s * 2, y + s * 6 + s / 2, s, s / 2, color, colors, width, height);
                break;

            case 'O':
                // Vertical izquierda
                FillRect(x, y + s, s, s * 4, color, colors, width, height);
                // Vertical derecha
                FillRect(x + s * 4, y + s, s, s * 4, color, colors, width, height);
                // Superior
                FillRect(x + s, y + s * 5, s * 3, s, color, colors, width, height);
                // Inferior
                FillRect(x + s, y, s * 3, s, color, colors, width, height);
                break;
        }
    }

    static void FillRect(int x, int y, int w, int h, Color color, Color[] colors, int texWidth, int texHeight)
    {
        for (int i = x; i < x + w && i < texWidth; i++)
        {
            for (int j = y; j < y + h && j < texHeight; j++)
            {
                if (i >= 0 && j >= 0 && i < texWidth && j < texHeight)
                    colors[j * texWidth + i] = color;
            }
        }
    }
}