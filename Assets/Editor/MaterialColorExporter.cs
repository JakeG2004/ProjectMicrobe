using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class MaterialColorExporter : EditorWindow
{
    [MenuItem("Tools/Export Material Colors as JSON")]
    public static void ShowWindow()
    {
        GetWindow<MaterialColorExporter>("Material Color Exporter");
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Export Colors of Selected Objects as JSON"))
        {
            ExportMaterialColors();
        }
    }

    private void ExportMaterialColors()
    {
        GameObject[] selectedObjects = Selection.gameObjects;
        if (selectedObjects.Length == 0)
        {
            Debug.LogWarning("No GameObjects selected!");
            return;
        }

        List<GroupData> groups = new List<GroupData>();

        for (int i = 0; i < selectedObjects.Length; i += 3)
        {
            GroupData group = new GroupData();
            group.group = i / 3;
            group.objects = new List<ObjectData>();

            for (int j = 0; j < 3 && (i + j) < selectedObjects.Length; j++)
            {
                GameObject obj = selectedObjects[i + j];
                ObjectData objData = new ObjectData();
                objData.name = obj.name;
                objData.materials = new List<MaterialData>();

                Renderer renderer = obj.GetComponent<Renderer>();
                if (renderer != null)
                {
                    Material[] materials = renderer.sharedMaterials;

                    foreach (Material mat in materials)
                    {
                        if (mat == null) continue;
                        if (j != 0 && mat.name.Contains("Shirt")) continue;

                        MaterialData matData = new MaterialData();
                        matData.name = mat.name;

                        if (mat.HasProperty("_TintR"))
                            matData.TintR = ColorToColorData(mat.GetColor("_TintR"));
                        if (mat.HasProperty("_TintG"))
                            matData.TintG = ColorToColorData(mat.GetColor("_TintG"));
                        if (mat.HasProperty("_TintB"))
                            matData.TintB = ColorToColorData(mat.GetColor("_TintB"));

                        objData.materials.Add(matData);
                    }
                }

                group.objects.Add(objData);
            }

            groups.Add(group);
        }

        Wrapper wrapper = new Wrapper();
        wrapper.groups = groups.ToArray();

        string json = JsonUtility.ToJson(wrapper, true);

        string path = EditorUtility.SaveFilePanel("Save Material Colors JSON", "", "MaterialColors.json", "json");
        if (!string.IsNullOrEmpty(path))
        {
            File.WriteAllText(path, json);
            Debug.Log($"Material colors exported to: {path}");
        }
    }

    private ColorData ColorToColorData(Color c)
    {
        return new ColorData { r = c.r, g = c.g, b = c.b, a = c.a };
    }
}


    // Data structures
    [System.Serializable]
    public class Wrapper
    {
        public GroupData[] groups;
    }

    [System.Serializable]
    public class GroupData
    {
        public int group;
        public List<ObjectData> objects;
    }

    [System.Serializable]
    public class ObjectData
    {
        public string name;
        public List<MaterialData> materials;
    }

    [System.Serializable]
    public class MaterialData
    {
        public string name;
        public ColorData TintR;
        public ColorData TintG;
        public ColorData TintB;
    }

    [System.Serializable]
    public class ColorData
    {
        public float r;
        public float g;
        public float b;
        public float a;
    }
