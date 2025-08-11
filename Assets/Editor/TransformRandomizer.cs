using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TransformRandomizer : EditorWindow
{

    private Vector3 _rotationAmounts;

    private Vector3 _scaleAmounts;

    [MenuItem("Tools/RandomizeObjects")]
    public static void ShowWindow()
    {
        EditorWindow window = GetWindow<TransformRandomizer>();
    }

    private void OnGUI()
    {
        // Transform selection into transform list
        GameObject[] selectedObjects = Selection.gameObjects;
        List<Transform> transforms = new List<Transform>();

        foreach (var go in selectedObjects)
        {
            if (go != null)
                transforms.Add(go.transform);
        }

        _rotationAmounts = EditorGUILayout.Vector3Field("Rotation", _rotationAmounts);
        _scaleAmounts = EditorGUILayout.Vector3Field("Scale", _scaleAmounts);

        if (GUILayout.Button("Randomize Transforms"))
        {
            foreach (Transform transform in transforms)
            {
                // Handle the rotation randomization
                Vector3 rot = transform.localEulerAngles;

                rot.x = Random.Range(-_rotationAmounts.x, _rotationAmounts.x);
                rot.y = Random.Range(-_rotationAmounts.y, _rotationAmounts.y);
                rot.z = Random.Range(-_rotationAmounts.z, _rotationAmounts.z);

                transform.localEulerAngles = rot;

                // Handle the scale randomization
                Vector3 scale = transform.localScale;

                scale.x = 1 + Random.Range(-_scaleAmounts.x, _scaleAmounts.x);
                scale.y = 1 + Random.Range(-_scaleAmounts.y, _scaleAmounts.y);
                scale.z = 1 + Random.Range(-_scaleAmounts.z, _scaleAmounts.z);

                transform.localScale = scale;
            }
        }

        GUILayout.Label("Selected Transforms");
        foreach (Transform transform in transforms)
        {
            GUILayout.Label(transform.name);
        }
    }
}
