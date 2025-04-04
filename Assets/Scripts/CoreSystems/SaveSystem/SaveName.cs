using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveName : MonoBehaviour
{
    public void UpdateSaveName(string name) 
    {
        SaveSystem.Instance.SetName(name);
    }
}
