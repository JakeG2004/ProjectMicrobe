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
    [SerializeField] private GameObject[] _eyebrowStyles;
    [SerializeField] private GameObject[] _skinObjs;
    [SerializeField] private GameObject[] _hats;
    [SerializeField] private GameObject _eyes;

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

    public GameObject[] GetEyebrowStyles()
    {
        return _eyebrowStyles;
    }

    public GameObject[] GetSkinObjects()
    {
        return _skinObjs;
    }

    public GameObject[] GetHats()
    {
        return _hats;
    }
    
    public GameObject GetEyes()
    {
        return _eyes;
    }

    public void DistableAllTops()
    {
        foreach (var top in _topStyles)
        {
            top.SetActive(false);
        }
    }

    public void DisableAllBottoms()
    {
        foreach(var bottom in _bottomStyles)
        {
            if (bottom.name == "Shoes")
            {
                continue;
            }

            bottom.SetActive(false);
        }
    }
}
