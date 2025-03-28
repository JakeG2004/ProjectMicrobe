using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CosmeticContainer : MonoBehaviour
{
    public static CosmeticContainer Instance { get; private set; }

    [SerializeField] private GameObject[] _hairStyles;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public GameObject[] GetHairStyles()
    {
        return _hairStyles;
    }
}
