using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class CreateColorTupleSOFromObject : EditorWindow
{
    private string _folderPath = "Assets/SOs/Colors";
    private string _baseFileName = "Color";
    private string _field1 = "_TintR";
    private string _field2 = "_TintG";
    private string _field3 = "_TintB";
    private string _requiredSubstring = "";
    private int _matIdx = 0;

    [MenuItem("Tools/Character/CreateColorTupleSOs (Inspector)")]
    public static void ShowWindow()
    {
        GetWindow<CreateColorTupleSOFromObject>("Color Tuple Generator");
    }

    void OnGUI()
    {
        GUILayout.Label("Create ColorTupleSOs from Selected Objects", EditorStyles.boldLabel);

        _folderPath = EditorGUILayout.TextField("Folder Path", _folderPath);
        _baseFileName = EditorGUILayout.TextField("Base Filenane", _baseFileName);
        _requiredSubstring = EditorGUILayout.TextField("RequiredSubstring", _requiredSubstring);

        _field1 = EditorGUILayout.TextField("Field 1", _field1);
        _field2 = EditorGUILayout.TextField("Field 2", _field2);
        _field3 = EditorGUILayout.TextField("Field 3", _field3);
        _matIdx = EditorGUILayout.IntField("Material Index", _matIdx);

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
            Renderer renderer = go.GetComponent<Renderer>();
            Material[] materials = renderer.materials;
            Material mat = materials[_matIdx];

            if (!mat.name.Contains(_requiredSubstring))
            {
                continue;
            }

            Color r = mat.GetColor(_field1);
            Color g = mat.GetColor(_field2);
            Color b = mat.GetColor(_field3);


            ColorTupleSO ctSO = ScriptableObject.CreateInstance<ColorTupleSO>();

            ctSO.r = r;
            ctSO.g = g;
            ctSO.b = b;

            string assetPath = Path.Combine(_folderPath, $"{_baseFileName}_{idx}.asset");
            AssetDatabase.CreateAsset(ctSO, assetPath);

            idx++;
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"Created {idx} ColorTupleSOs in '{_folderPath}'");
    }
}
