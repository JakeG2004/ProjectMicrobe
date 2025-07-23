using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PylonMonitorManager : MonoBehaviour
{
    [SerializeField] private GameObject _pylonEntryPrefab;
    [SerializeField] private Transform _entriesParent;
    [SerializeField] private GameObject _noPylonsObj;
    void OnEnable()
    {
        DestroyPylonEntries();
        Dictionary<string, float> pylonDatas = FindPylonDatas();

        if (pylonDatas.Count == 0)
        {
            _noPylonsObj.SetActive(true);
            return;
        }

        _noPylonsObj.SetActive(false);

        foreach (var kvp in pylonDatas)
        {
            GameObject curEntryGO = Instantiate(_pylonEntryPrefab, _entriesParent);
            PylonEntry curEntry = curEntryGO.GetComponent<PylonEntry>();

            curEntry.SetVals(kvp.Key, kvp.Value);
        }
    }

    private Dictionary<string, float> FindPylonDatas()
    {
        Dictionary<string, float> pylonDatas = new();

        PylonStatusEventsChecker[] _pylons = Object.FindObjectsOfType<PylonStatusEventsChecker>();

        // Return empty list if there arent any
        if (_pylons.Length == 0)
        {
            return pylonDatas;
        }

        // Set each into a tuple
        foreach (PylonStatusEventsChecker psec in _pylons)
        {
            string envName = psec.gameObject.GetComponent<MicrobePopSim>().GetEnvSO().envName;
            pylonDatas.Add(envName, psec.GetEnvHealth());
            Debug.Log($"{envName}, {psec.GetEnvHealth()}");
        }

        return pylonDatas;
    }

    private void DestroyPylonEntries()
    {
        foreach (Transform child in _entriesParent)
        {
            if (child.gameObject.name == "BackButton")
            {
                continue;
            }
            
            Destroy(child.gameObject);
        }
    }
}
