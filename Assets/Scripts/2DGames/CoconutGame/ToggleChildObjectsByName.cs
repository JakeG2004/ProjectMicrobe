using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleChildObjectsByName : MonoBehaviour
{
    public void SetNamedChildTrue(string childName)
    {
        foreach (Transform child in transform)
        {
            if (child.gameObject.name == childName)
            {
                child.gameObject.SetActive(true);
                continue;
            }

            child.gameObject.SetActive(false);
        }
    }

    public void SetAllChildrenFalse()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
    }
}
