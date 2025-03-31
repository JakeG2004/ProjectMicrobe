using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CosmeticContainer : MonoBehaviour
{
    public static CosmeticContainer Instance { get; private set; }

    [SerializeField] private GameObject[] _hairStyles;
    [SerializeField] private GameObject[] _topStyles;
    [SerializeField] private GameObject[] _bottomStyles;
    [SerializeField] private GameObject[] _glassesStyles;

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

    public GameObject[] GetTopStyles()
    {
        return _topStyles;
    }

    public GameObject[] GetBottomStyles()
    {
        return _bottomStyles;
    }

    public GameObject[] GetGlassesStyles()
    {
        return _glassesStyles;
    }
}
