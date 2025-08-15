#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityMeshSimplifier; // Make sure your namespace matches

public class OptimizeMeshesTool : EditorWindow
{
    private GameObject _selectedRoot;
    private float _quality = 0.5f;

    [MenuItem("Tools/Meshes/Optimize Selected Mesh Hierarchy", false, 100)]
    private static void ShowWindow()
    {
        var window = GetWindow<OptimizeMeshesTool>("Mesh Optimizer");
        window.minSize = new Vector2(300, 100);
        window._selectedRoot = Selection.activeGameObject;
        window.Show();
    }

    private void OnGUI()
    {
        GUILayout.Label("Optimize All Meshes in Hierarchy", EditorStyles.boldLabel);

        _selectedRoot = (GameObject)EditorGUILayout.ObjectField("Root Object", _selectedRoot, typeof(GameObject), true);
        _quality = EditorGUILayout.Slider("Quality", _quality, 0f, 1f);

        EditorGUILayout.Space();

        if (GUILayout.Button("Optimize"))
        {
            if (_selectedRoot != null)
            {
                OptimizeSelected(_selectedRoot, _quality);
            }
            else
            {
                EditorUtility.DisplayDialog("No Root Selected", "Please assign a root GameObject.", "OK");
            }
        }

        if (GUILayout.Button("Save Meshes"))
        {
            if (_selectedRoot != null)
            {
                SaveSelected(_selectedRoot);
            }
            else
            {
                EditorUtility.DisplayDialog("No Root Selected", "Please assign a root GameObject.", "OK");
            }
        }
    }

    private static void OptimizeSelected(GameObject root, float quality)
    {
        Undo.RegisterFullObjectHierarchyUndo(root, "Optimize Meshes");

        Dictionary<Mesh, Mesh> optimizedCache = new Dictionary<Mesh, Mesh>();
        MeshFilter[] meshFilters = root.GetComponentsInChildren<MeshFilter>(true);

        foreach (MeshFilter mf in meshFilters)
        {
            if (mf.sharedMesh == null) continue;

            Mesh originalMesh = mf.sharedMesh;
            if (!optimizedCache.TryGetValue(originalMesh, out Mesh optimizedMesh))
            {
                var meshSimplifier = new MeshSimplifier();
                meshSimplifier.Initialize(originalMesh);

                // Preserve important edges for texture safety
                meshSimplifier.PreserveBorderEdges = true;
                meshSimplifier.PreserveUVSeamEdges = true;
                meshSimplifier.PreserveUVFoldoverEdges = true;

                meshSimplifier.SimplifyMesh(quality);
                optimizedMesh = meshSimplifier.ToMesh();
                optimizedCache[originalMesh] = optimizedMesh;
            }

            mf.sharedMesh = optimizedMesh;
        }

        EditorUtility.DisplayDialog("Mesh Optimization Complete", $"Optimized {optimizedCache.Count} unique meshes.", "OK");
    }

    private static void SaveSelected(GameObject root)
    {
        HashSet<Mesh> savedMeshes = new HashSet<Mesh>();
        MeshFilter[] meshFilters = root.GetComponentsInChildren<MeshFilter>(true);

        foreach (MeshFilter mf in meshFilters)
        {
            if (mf.sharedMesh == null) continue;
            if (savedMeshes.Contains(mf.sharedMesh)) continue;

            string meshName = "Optimized__" + mf.gameObject.name;
            MeshSaverEditor.SaveMesh(mf.sharedMesh, meshName, false, true);
            savedMeshes.Add(mf.sharedMesh);
        }

        EditorUtility.DisplayDialog("Mesh Saving Complete", $"Saved {savedMeshes.Count} unique meshes.", "OK");
    }
}
#endif
