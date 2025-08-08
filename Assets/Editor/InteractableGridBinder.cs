using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

// Must inherit from EditorWindow
public class InteractableGridBinder : EditorWindow
{
    private GridLayoutGroup grid;
    private Selectable[] selectables;
    private int columns;
    private int rows;
    private Selectable upSelectable;
    private Selectable downSelectable;

    [MenuItem("Tools/UI/Bind Interactables/Grid")]
    public static void ShowWindow()
    {
        GetWindow<InteractableGridBinder>("Grid Nav Binder");
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Grid Navigation Binder", EditorStyles.boldLabel);
        grid = (GridLayoutGroup)EditorGUILayout.ObjectField("Grid", grid, typeof(GridLayoutGroup), true);
        upSelectable = (Selectable)EditorGUILayout.ObjectField("Up Selectable", upSelectable, typeof(Selectable), true);
        downSelectable = (Selectable)EditorGUILayout.ObjectField("Down Selectable", downSelectable, typeof(Selectable), true);

        if (GUILayout.Button("Get Grid Interactables"))
        {
            PopulateChildren();
        }

        if (selectables != null && selectables.Length > 0)
        {
            EditorGUILayout.LabelField($"Found {selectables.Length} Selectables");

            if (GUILayout.Button("Bind Selectables"))
            {
                BindSelectables();
            }
        }
    }

    private void PopulateChildren()
    {
        if (grid == null)
        {
            Debug.LogWarning("Please assign a GridLayoutGroup.");
            return;
        }

        List<Selectable> found = new List<Selectable>();

        foreach (Transform child in grid.transform)
        {
            Selectable sel = child.GetComponent<Selectable>();
            if (sel != null)
            {
                found.Add(sel);
            }
        }

        selectables = found.ToArray();

        if (selectables.Length == 0)
        {
            Debug.LogWarning("No Selectables found in grid.");
        }
        else
        {
            Debug.Log($"Found {selectables.Length} selectable(s).");
        }
    }

    private void BindSelectables()
    {
        if (selectables == null || selectables.Length == 0)
        {
            Debug.LogWarning("No selectables to bind.");
            return;
        }

        int total = selectables.Length;

        // Calculate columns based on grid width and spacing
        float gridWidth = ((RectTransform)grid.transform).rect.width;
        columns = Mathf.Max(1, Mathf.FloorToInt((gridWidth + grid.spacing.x) / (grid.cellSize.x + grid.spacing.x)));
        rows = Mathf.CeilToInt((float)total / columns);

        Undo.RecordObjects(selectables, "Bind UI Navigation");

        for (int i = 0; i < total; i++)
        {
            Navigation nav = new Navigation();
            nav.mode = Navigation.Mode.Explicit;

            int row = i / columns;
            int col = i % columns;

            if (row == 0)
            {
                nav.selectOnUp = upSelectable;
            }

            // Up
            if (row > 0)
            {
                int upIndex = i - columns;
                if (upIndex < total)
                    nav.selectOnUp = selectables[upIndex];
            }

            if (row == rows - 1)
            {
                nav.selectOnDown = downSelectable;
            }

            // Down
            if (row < rows - 1)
            {
                int downIndex = i + columns;
                if (downIndex < total)
                    nav.selectOnDown = selectables[downIndex];
            }

            // Left
            if (col > 0)
            {
                int leftIndex = i - 1;
                if (leftIndex >= 0)
                    nav.selectOnLeft = selectables[leftIndex];
            }

            // Right
            if (col < columns - 1)
            {
                int rightIndex = i + 1;
                if (rightIndex < total)
                    nav.selectOnRight = selectables[rightIndex];
            }

            selectables[i].navigation = nav;
        }

        Debug.Log("Navigation binding complete.");
    }
}
