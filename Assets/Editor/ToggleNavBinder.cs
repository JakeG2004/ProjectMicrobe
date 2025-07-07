// ToggleNavBinder.cs
// A script to bind selectable ui elements automatically
// Author:  Jake Gendreau
// Date:    7/2/25 (my birthday!)

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEditor;

public class ToggleNavBinder
{
    public enum BindDirection
    {
        HORIZONTAL,
        VERTICAL
    };

    public static BindDirection bindDirection = BindDirection.HORIZONTAL;

    [MenuItem("Tools/UI/Bind Toggles/Horizontal")]
    public static void LinkTogglesHorizontally()
    {
        LinkToggles(BindDirection.HORIZONTAL);
    }

    [MenuItem("Tools/UI/Bind Toggles/Vertical")]
    public static void LinkTogglesVertically()
    {
        LinkToggles(BindDirection.VERTICAL);
    }

    public static void LinkToggles(BindDirection bindDirection)
    {
        GameObject[] selectedObjects = Selection.gameObjects;

        if (selectedObjects.Length < 2)
        {
            Debug.LogWarning("Select at least 2 toggles to link");
            return;
        }

        // Filter for selectables
        Selectable[] selectables = System.Array.ConvertAll(selectedObjects, go => go.GetComponent<Selectable>());
        selectables = System.Array.FindAll(selectables, t => t != null);

        if (selectables.Length < 2)
        {
            Debug.LogWarning("No valid selectables found");
        }

        Undo.RecordObjects(selectables, "Link toggle navigation");

        for (int i = 0; i < selectables.Length; i++)
        {
            Navigation nav = selectables[i].navigation;
            nav.mode = Navigation.Mode.Explicit;

            if (bindDirection == BindDirection.HORIZONTAL)
            {
                nav.selectOnLeft = (i > 0) ? selectables[i - 1] : selectables[selectables.Length - 1];
                nav.selectOnRight = (i < selectables.Length - 1) ? selectables[i + 1] : selectables[0];

                selectables[i].navigation = nav;

                Debug.Log("Horizontal link complete");
            }

            if (bindDirection == BindDirection.VERTICAL)
            {
                nav.selectOnUp = (i > 0) ? selectables[i - 1] : selectables[selectables.Length - 1];
                nav.selectOnDown = (i < selectables.Length - 1) ? selectables[i + 1] : selectables[0];

                selectables[i].navigation = nav;

                Debug.Log("Vertical link complete");
            }
        }
    }
}
