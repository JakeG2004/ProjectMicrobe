using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class CreateTopColorTupleSOFromObject : EditorWindow
{
    private string _folderPath = "Assets/SOs/Colors/Tops";
    private string _baseFileName = "Top";
    private string _requiredSubstring = "";

    [MenuItem("Tools/Character/CreateTopColorTupleSO (Inspector)")]
    public static void ShowWindow()
    {
        GetWindow<CreateTopColorTupleSOFromObject>("Color Tuple Generator");
    }

    void OnGUI()
    {
        GUILayout.Label("Create ColorTupleSOs from Selected Objects", EditorStyles.boldLabel);

        _folderPath = EditorGUILayout.TextField("Folder Path", _folderPath);
        _baseFileName = EditorGUILayout.TextField("Base Filenane", _baseFileName);
        _requiredSubstring = EditorGUILayout.TextField("RequiredSubstring", _requiredSubstring);

        if (GUILayout.Button("Create ColorTuple ScriptableObjects"))
        {
            CreateColorTupleSOs();
        }
    }

    public void CreateColorTupleSOs()
    {
        if (string.IsNullOrEmpty(_folderPath) || string.IsNullOrEmpty(_baseFileName))
        {
            Debug.LogWarning("Folder patha dn base filename must be set");
            return;
        }

        if (!Directory.Exists(_folderPath))
        {
            Debug.LogWarning("Invalid path!");
            return;
        }

        int idx = 0;

        GameObject[] selectedObjects = Selection.gameObjects;
        if (selectedObjects.Length == 0)
        {
            Debug.LogWarning("No objects");
            return;
        }

        Undo.RecordObjects(selectedObjects, "CreateColorTupleSOs");

        foreach (GameObject go in selectedObjects)
        {
            TopController _tc = go.GetComponent<TopController>();

            ColorTuple shirt = _tc.GetShirtColors();
            ColorTuple jacket = _tc.GetJacketColors();
            ColorTuple coat = _tc.GetCoatColors();

            ColorTupleSO shirtCT = ScriptableObject.CreateInstance<ColorTupleSO>();
            ColorTupleSO jacketCT = ScriptableObject.CreateInstance<ColorTupleSO>();
            ColorTupleSO coatCT = ScriptableObject.CreateInstance<ColorTupleSO>();

            shirtCT.r = shirt.r;
            shirtCT.g = shirt.g;
            shirtCT.b = shirt.b;

            jacketCT.r = jacket.r;
            jacketCT.g = jacket.g;
            jacketCT.b = jacket.b;

            coatCT.r = coat.r;
            coatCT.g = coat.g;
            coatCT.b = coat.b;

            string assetPath = Path.Combine(_folderPath, $"{_baseFileName}_{go.name}");
            AssetDatabase.CreateAsset(shirtCT, assetPath + "_shirt.asset");
            AssetDatabase.CreateAsset(jacketCT, assetPath + "_jacket.asset");
            AssetDatabase.CreateAsset(coatCT, assetPath + "_coat.asset");
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"Created {idx} ColorTupleSOs in '{_folderPath}'");
    }
}
