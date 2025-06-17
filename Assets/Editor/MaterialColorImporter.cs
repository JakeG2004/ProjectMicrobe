using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class MaterialColorImporter : EditorWindow
{
    private List<TopController> _topControllers = new();

    [MenuItem("Tools/Import Material Colors JSON")]
    public static void ShowWindow()
    {
        GetWindow<MaterialColorImporter>("Material Color Importer");
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Import Colors and Apply"))
        {
            ApplyToSelectedTopControllers();
        }
    }

private void ImportAndApply()
{
    string path = EditorUtility.OpenFilePanel("Open Material Colors JSON", "", "json");
    if (string.IsNullOrEmpty(path)) return;

    string json = File.ReadAllText(path);
    Wrapper data = JsonUtility.FromJson<Wrapper>(json);

    int groupIndex = 0;

    for (int i = 0; i < _topControllers.Count; i++)
    {
        var topCtrl = _topControllers[i];
        if (topCtrl == null)
        {
            Debug.LogWarning("No TopController found in the scene!");
            continue;
        }

        // Every 3 TopControllers, increase groupIndex
        if (i > 0 && i % 3 == 0)
        {
            groupIndex++;
        }

        if (groupIndex >= data.groups.Length)
        {
            Debug.LogWarning("Not enough groups in JSON for all TopControllers!");
            break;
        }

        GroupData group = data.groups[groupIndex];

        foreach (var obj in group.objects)
        {
            if (obj.materials.Count == 0) continue;

            var mat = obj.materials[0];
            var r = mat.TintR != null ? mat.TintR.ToColor() : Color.white;
            var g = mat.TintG != null ? mat.TintG.ToColor() : Color.white;
            var b = mat.TintB != null ? mat.TintB.ToColor() : Color.white;

            if (obj.name.Contains("Shirt"))
            {
                topCtrl.GetShirtColors().r = r;
                topCtrl.GetShirtColors().g = g;
                topCtrl.GetShirtColors().b = b;

                topCtrl.SetHairAccessoryColor(r);
            }
            else if (obj.name.Contains("Hoodie") || obj.name.Contains("Jacket"))
            {
                topCtrl.GetJacketColors().r = r;
                topCtrl.GetJacketColors().g = g;
                topCtrl.GetJacketColors().b = b;
            }
            else if (obj.name.Contains("LabCoat") || obj.name.Contains("Coat"))
            {
                topCtrl.GetCoatColors().r = r;
                topCtrl.GetCoatColors().g = g;
                topCtrl.GetCoatColors().b = b;
            }
        }

        //topCtrl.AssignColors();
    }

    Debug.Log("Material colors applied!");
}

    
    public void ApplyToSelectedTopControllers()
    {
        _topControllers = new();
#if UNITY_EDITOR
        // Get all selected game objects in the editor
        var selectedGameObjects = Selection.gameObjects;

        foreach (var go in selectedGameObjects)
        {
            // Try to get TopController component
            var topController = go.GetComponent<TopController>();
            if (topController != null)
            {
                _topControllers.Add(topController);
            }
        }

        ImportAndApply();
#else
        Debug.LogWarning("ApplyToSelectedTopControllers can only run in the Unity Editor.");
#endif
    }
}

// Helper extension
public static class ColorDataExtensions
{
    public static Color ToColor(this ColorData c)
    {
        return new Color(c.r, c.g, c.b, c.a);
    }
}
