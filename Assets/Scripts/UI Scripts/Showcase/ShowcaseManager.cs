using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowcaseManager : MonoBehaviour
{
    public ShowcaseManager Instance { get; private set; }
    private CosmeticUnlocker _cosmeticUnlocker;

    [System.Serializable]
    private struct ShowcaseObject
    {
        [SerializeField] public string name;
        [SerializeField] public ShowcaseRotator showcaseObj;
    }

    [SerializeField] private List<ShowcaseObject> _objs = new();

    void Awake()
    {
        if (Instance != this && Instance != null)
        {
            Destroy(this.gameObject);
        }

        else
        {
            Instance = this;
        }

        _cosmeticUnlocker = gameObject.GetComponent<CosmeticUnlocker>();
    }

    // Starts showcase on a specific object based on name
    public void StartShowcase(string objName)
    {
        foreach (ShowcaseObject obj in _objs)
        {
            // Skip over objects we don't need
            if (obj.name != objName)
            {
                continue;
            }

            // Enable object, start its rotation
            obj.showcaseObj.gameObject.SetActive(true);
            obj.showcaseObj.StartRotation();
            _cosmeticUnlocker.UnlockCosmetic(objName);

            return;
        }
    }

    // Stops showcase on all showcase objects
    public void StopShowcase()
    {
        foreach (ShowcaseObject obj in _objs)
        {
            obj.showcaseObj.StopRotation();
            obj.showcaseObj.gameObject.SetActive(false);
        }
    }
}
