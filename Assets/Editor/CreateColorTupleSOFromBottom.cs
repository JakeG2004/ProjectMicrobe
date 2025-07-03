using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class CreateBottomColorTupleSOFromObject : EditorWindow
{
    private string _folderPath = "Assets/SOs/Colors/Bottoms";
    private string _baseFileName = "Bottom";
    private string _requiredSubstring = "";

    [MenuItem("Tools/Character/CreateBottomColorTupleSO (Inspector)")]
    public static void ShowWindow()
    {
        GetWindow<CreateBottomColorTupleSOFromObject>("Color Tuple Generator");
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
            BottomController _bc = go.GetComponent<BottomController>();

            ColorTuple shorts = _bc.GetShortsColor();
            ColorTuple pants = _bc.GetPantsColor();
            ColorTuple shoe = _bc.GetShoeColor();

            ColorTupleSO shortsCT = ScriptableObject.CreateInstance<ColorTupleSO>();
            ColorTupleSO pantsCT = ScriptableObject.CreateInstance<ColorTupleSO>();
            ColorTupleSO shoeCT = ScriptableObject.CreateInstance<ColorTupleSO>();

            shortsCT.r = shorts.r;
            shortsCT.g = shorts.g;
            shortsCT.b = shorts.b;

            pantsCT.r = pants.r;
            pantsCT.g = pants.g;
            pantsCT.b = pants.b;

            shoeCT.r = shoe.r;
            shoeCT.g = shoe.g;
            shoeCT.b = shoe.b;

            string assetPath = Path.Combine(_folderPath, $"{_baseFileName}_{go.name}");
            AssetDatabase.CreateAsset(shortsCT, assetPath + "_shorts.asset");
            AssetDatabase.CreateAsset(pantsCT, assetPath + "_pants.asset");
            AssetDatabase.CreateAsset(shoeCT, assetPath + "_shoe.asset");
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"Created {idx} ColorTupleSOs in '{_folderPath}'");
    }
}
