// CosmeticUnlocker.cs
// A script for managing cosmetic unlocking
// Author:  Jake Gendreau
// Date:    6/17/25

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CosmeticUnlocker : MonoBehaviour
{
    public void UnlockCosmetic(string cosmeticName)
    {
        SaveSystem.Instance.UnlockCosmetic(cosmeticName);
    }
}
