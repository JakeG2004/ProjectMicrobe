using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleGetPylonMenu : MonoBehaviour
{
    public void ToggleMenu()
    {
        GetPylonMenuManager.Instance.ToggleMenu();
    }
}
