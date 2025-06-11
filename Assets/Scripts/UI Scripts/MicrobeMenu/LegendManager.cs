using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegendManager : MonoBehaviour
{
    [SerializeField] private GameObject _legendEntry;

    public void DestroyEntries()
    {
        for (int i = 1; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }

    public bool AddEntry(Color col, string legendText)
    {
        GameObject newLegendElement = Object.Instantiate(_legendEntry, transform);
        newLegendElement.name = legendText;
        newLegendElement.GetComponent<LegendEntryScript>().SetColor(col);
        newLegendElement.GetComponent<LegendEntryScript>().SetText(legendText);

        return true;
    }
}
