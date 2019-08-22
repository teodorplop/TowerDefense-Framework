using UnityEngine;
using UnityEditor;
using System.IO;

public static class CreateAssetsUtility {
    private static string GetAssetPath(string type, string extension) {
        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (string.IsNullOrEmpty(path)) {
            path = "Assets";
        }
        return AssetDatabase.GenerateUniqueAssetPath(path + "/New " + type + extension);
    }
    private static void CreateAsset(Object asset, string extension) {
        AssetDatabase.CreateAsset(asset, GetAssetPath(asset.GetType().ToString(), extension));

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
    }

    public static void CreateScriptableObject<T>() where T : ScriptableObject {
        CreateAsset(ScriptableObject.CreateInstance<T>(), ".asset");
    }

    public static void CreateTextAsset<T>(string path) where T : new() {
        if (!path.EndsWith(".txt"))
            path += ".txt";

        T instance = new T();
        string text = JsonUtility.ToJson(instance, true);
        File.WriteAllText(path, text);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    public static void CreateTextAssetAtSelection<T>() where T : new() {
        CreateTextAsset<T>(GetAssetPath("TextAsset", ".txt"));
    }

    [MenuItem("Assets/Create/Text Asset")]
    private static void CreateTextAsset() {
        FileStream file = File.Create(GetAssetPath("TextAsset", ".txt"));
        file.Close();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
