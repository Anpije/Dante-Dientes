using UnityEngine;
using UnityEditor;
using System.IO;

public class CreateProtectedTextTexture : EditorWindow
{
    [MenuItem("Tools/Create Protected Text Texture")]
    static void CreateTexture()
    {
        if (!Directory.Exists("Assets/Textures"))
        {
            Directory.CreateDirectory("Assets/Textures");
        }

        // Creamos la textura
        int width = 512;
        int height = 128;
        Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);

        // Con un fondo transparente
        Color[] colors = new Color[width * height];
        for (int i = 0; i < colors.Length; i++)
            colors[i] = Color.clear;

        // De color azul para el texto
        Color textColor = new Color(0.3f, 0.8f, 1f, 1f);

        // Dibujando un texto simple
        DrawTextSimple("PROTEGIDO", 40, 50, 6, textColor, colors, width);

        texture.SetPixels(colors);
        texture.Apply();

        // Guardar como PNG
        byte[] bytes = texture.EncodeToPNG();
        string path = Application.dataPath + "/Textures/ProtectedText.png";
        File.WriteAllBytes(path, bytes);

        AssetDatabase.Refresh();

        string assetPath = "Assets/Textures/ProtectedText.png";
        TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
        if (importer != null)
        {
            importer.textureType = TextureImporterType.Default;
            importer.alphaIsTransparency = true;
            importer.wrapMode = TextureWrapMode.Clamp;
            importer.filterMode = FilterMode.Bilinear;
            importer.SaveAndReimport();
        }

        Debug.Log("Textura 'PROTEGIDO' creada en: " + assetPath);
        EditorUtility.DisplayDialog("Éxito", "Textura creada en Assets/Textures/ProtectedText.png", "OK");

        // Seleccionaremos la textura en el Project
        Texture2D createdTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);
        if (createdTexture != null)
        {
            Selection.activeObject = createdTexture;
            EditorGUIUtility.PingObject(createdTexture);
        }
    }

    static void DrawTextSimple(string text, int x, int y, int pixelSize, Color color, Color[] colors, int width)
    {
        // Dibujaremos rectángulos simulando texto
        for (int i = 0; i < text.Length; i++)
        {
            int xPos = x + i * 45;
            DrawBlockLetter(text[i], xPos, y, pixelSize, color, colors, width);
        }
    }

    static void DrawBlockLetter(char c, int x, int y, int s, Color color, Color[] colors, int width)
    {
        int h = 128;

        // Patrones simples para cada letra (estilo bloque)
        switch (char.ToUpper(c))
        {
            case 'P':
                FillRect(x, y, s, s * 5, color, colors, width, h); // Vertical
                FillRect(x, y + s * 4, s * 4, s, color, colors, width, h); // Superior
                FillRect(x + s * 3, y + s * 2, s, s * 2, color, colors, width, h); // Derecha medio
                FillRect(x, y + s * 2, s * 4, s, color, colors, width, h); // Medio
                break;
            case 'R':
                FillRect(x, y, s, s * 5, color, colors, width, h);
                FillRect(x, y + s * 4, s * 4, s, color, colors, width, h);
                FillRect(x + s * 3, y + s * 2, s, s * 2, color, colors, width, h);
                FillRect(x, y + s * 2, s * 4, s, color, colors, width, h);
                FillRect(x + s * 2, y, s * 2, s * 2, color, colors, width, h); // Diagonal
                break;
            case 'O':
                FillRect(x, y + s, s, s * 3, color, colors, width, h);
                FillRect(x + s * 3, y + s, s, s * 3, color, colors, width, h);
                FillRect(x + s, y + s * 4, s * 2, s, color, colors, width, h);
                FillRect(x + s, y, s * 2, s, color, colors, width, h);
                break;
            case 'T':
                FillRect(x, y + s * 4, s * 5, s, color, colors, width, h);
                FillRect(x + s * 2, y, s, s * 5, color, colors, width, h);
                break;
            case 'E':
                FillRect(x, y, s, s * 5, color, colors, width, h);
                FillRect(x, y + s * 4, s * 5, s, color, colors, width, h);
                FillRect(x, y + s * 2, s * 3, s, color, colors, width, h);
                FillRect(x, y, s * 5, s, color, colors, width, h);
                break;
            case 'G':
                FillRect(x + s, y + s * 4, s * 3, s, color, colors, width, h);
                FillRect(x, y + s, s, s * 3, color, colors, width, h);
                FillRect(x + s * 3, y + s * 2, s, s * 2, color, colors, width, h);
                FillRect(x + s, y, s * 3, s, color, colors, width, h);
                FillRect(x + s * 2, y + s * 2, s, s, color, colors, width, h);
                break;
            case 'I':
                FillRect(x + s * 2, y, s, s * 5, color, colors, width, h);
                FillRect(x, y, s * 5, s, color, colors, width, h);
                FillRect(x, y + s * 4, s * 5, s, color, colors, width, h);
                break;
            case 'D':
                FillRect(x, y, s, s * 5, color, colors, width, h);
                FillRect(x + s, y, s * 3, s, color, colors, width, h);
                FillRect(x + s, y + s * 4, s * 3, s, color, colors, width, h);
                FillRect(x + s * 3, y + s, s, s * 3, color, colors, width, h);
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