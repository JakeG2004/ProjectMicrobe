using UnityEngine;
using UnityEditor;

public class ReplaceWithSpecifiedObject : EditorWindow
{
    private GameObject replacementPrefab;

    [MenuItem("Tools/GOManager/Replace Selected With...")]
    private static void ShowWindow()
    {
        GetWindow<ReplaceWithSpecifiedObject>("Replace With...");
    }

    private void OnGUI()
    {
        GUILayout.Label("Replace Selected GameObjects", EditorStyles.boldLabel);

        replacementPrefab = (GameObject)EditorGUILayout.ObjectField(
            "Replacement Prefab", replacementPrefab, typeof(GameObject), false);

        if (GUILayout.Button("Replace Selected"))
        {
            ReplaceSelected();
        }
    }

    private void ReplaceSelected()
    {
        if (replacementPrefab == null)
        {
            EditorUtility.DisplayDialog("Error", "Please assign a replacement prefab.", "OK");
            return;
        }

        GameObject[] selectedObjects = Selection.gameObjects;

        if (selectedObjects.Length == 0)
        {
            EditorUtility.DisplayDialog("Error", "No GameObjects selected.", "OK");
            return;
        }

        Undo.RegisterCompleteObjectUndo(selectedObjects, "Replace GameObjects");

        foreach (GameObject go in selectedObjects)
        {
            // Instantiate replacement
            GameObject newObject = (GameObject)PrefabUtility.InstantiatePrefab(replacementPrefab);

            // Maintain transform
            newObject.transform.position = go.transform.position;
            newObject.transform.rotation = go.transform.rotation;
            newObject.transform.localScale = go.transform.localScale;

            // Maintain hierarchy
            newObject.transform.parent = go.transform.parent;

            // Register undo
            Undo.RegisterCreatedObjectUndo(newObject, "Replace GameObject");

            // Delete old object
            Undo.DestroyObjectImmediate(go);
        }
    }
}
