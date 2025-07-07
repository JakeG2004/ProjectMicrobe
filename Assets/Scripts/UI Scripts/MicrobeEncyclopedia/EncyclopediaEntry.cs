// EncyclopediaEntry.cs
// A script for setting the information in encyclopedia entries
// Author:  Jake Gendreau
// Date:    7/7/25

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EncyclopediaEntry : MonoBehaviour
{
    [SerializeField] private MicrobeSO _microbe;
    [SerializeField] private TMP_Text _titleText;
    [SerializeField] private TMP_Text _bodyText;

    void Start()
    {
        SetInfo();
    }

    private void SetInfo()
    {
        _titleText.text = _microbe.microbeName;
        string bodyContent = "";

        bodyContent += GetProducedResources();
        bodyContent += GetRequiredResources();
        bodyContent += GetToxins();

        _bodyText.text = bodyContent;
    }

    private string GetToxins()
    {
        string toxinString = "Toxic to ";

        List<string> toxins = new();

        if (_microbe.toxins.Count == 0)
        {
            toxinString += "nothing.\n\n";
            return toxinString;            
        }

        foreach (ToxinAmount toxin in _microbe.toxins)
        {
            toxins.Add(toxin.toxinName);
        }

        int count = toxins.Count;

        for (int i = 0; i < count; i++)
        {
            toxinString += toxins[i];

            if (i == count - 1)
            {
                toxinString += ".";
                break;
            }

            toxinString += ", ";

            if (i == count - 2)
            {
                toxinString += "and ";
            }
        }

        return toxinString;
    }

    private string GetRequiredResources()
    {
        string conRes = "Consumes ";

        int count = _microbe.requiredResources.Count;

        if (count == 0)
        {
            conRes += "nothing.\n\n";
            return conRes;
        }
        
        for (int i = 0; i < count; i++)
        {
            ResourceAmount res = _microbe.requiredResources[i];

            conRes += $"{res.amount} {res.resourceName}";

            if (i == count - 1)
            {
                conRes += ".\n\n";
                break;
            }

            // Always add the comma after an element
            conRes += ", ";

            // Add the 'and' on the last element
            if (i == count - 2)
            {
                conRes += "and ";
            }
        }

        return conRes;
    }

    private string GetProducedResources()
    {
        string prodRes = "Produces ";

        int count = _microbe.producedResources.Count;

        if (count == 0)
        {
            prodRes += "nothing.\n\n";
            return prodRes;
        }

        for (int i = 0; i < count; i++)
        {
            ResourceAmount res = _microbe.producedResources[i];

            prodRes += $"{res.amount} {res.resourceName}";

            if (i == count - 1)
            {
                prodRes += ".\n\n";
                break;
            }

            // Always add the comma after an element
            prodRes += ", ";

            // Add the 'and' on the last element
            if (i == count - 2)
            {
                prodRes += "and ";
            }
        }

        return prodRes;
    }
}
