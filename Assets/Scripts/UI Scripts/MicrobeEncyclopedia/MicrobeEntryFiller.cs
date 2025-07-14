// MicrobeEntryFiller.cs
// Fills a microbe entry based on a microbe SO
// Author:  Jake Gendreau
// Date:    7/14/25

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MicrobeEntryFiller : MonoBehaviour
{
    [SerializeField] private MicrobeSO _microbe;

    [Space(10)]
    [SerializeField] private TMP_Text _titleText;
    [SerializeField] private TMP_Text _summaryText;
    [SerializeField] private TMP_Text _bioText;

    [Space(10)]
    [SerializeField] private Image _microbeBody;
    [SerializeField] private Image _microbeFace;

    void Start()
    {
        _titleText.text = _microbe.microbeName;

        _bioText.text = _microbe.description;
        
        SetSummaryString();
        SetMicrobeImages();
    }

    void SetSummaryString()
    {
        string summary = "";

        summary += PopulateConsumptionText();
        summary += PopulateProductionText();
        summary += PopulateToxinsText();

        _summaryText.text = summary;
    }

    void SetMicrobeImages()
    {
        _microbeBody.sprite = _microbe.microbeBody;
        _microbeBody.color = _microbe.color;
        _microbeFace.sprite = _microbe.microbeFace;
    }

    private string PopulateConsumptionText()
    {
        string consumption = "Consumes ";

        // Handle empty
        if (_microbe.requiredResources.Count == 0)
        {
            consumption += "nothing.\n";
            return consumption;
        }

        int idx = 0;
        foreach (var res in _microbe.requiredResources)
        {
            consumption += $"{res.amount} {res.resourceName}";

            // Proper grammar for last entry
            if (idx == _microbe.requiredResources.Count - 1)
            {
                consumption += ".";
                break;
            }

            // Proper grammar for second to last entry
            if (idx == _microbe.requiredResources.Count - 2)
            {
                consumption += ", and ";
                idx++;
                continue;
            }

            // Proper grammar for every other entry
            consumption += ", ";
            idx++;
        }

        consumption += "\n";
        return consumption;
    }

    private string PopulateProductionText()
    {
        string production = "Produces ";

        // Handle empty
        if (_microbe.producedResources.Count == 0)
        {
            production += "nothing.\n";
            return production;
        }

        int idx = 0;
        foreach (var res in _microbe.producedResources)
        {
            production += $"{res.amount} {res.resourceName}";

            // Proper grammar for last entry
            if (idx == _microbe.producedResources.Count - 1)
            {
                production += ".";
                break;
            }

            // Proper grammar for second to last entry
            if (idx == _microbe.producedResources.Count - 2)
            {
                production += ", and ";
                idx++;
                continue;
            }

            // Proper grammar for every other entry
            production += ", ";
            idx++;
        }

        production += "\n";
        return production;
    }
    
    private string PopulateToxinsText()
    {
        string toxins = "Toxic to ";

        // Handle empty
        if (_microbe.toxins.Count == 0)
        {
            toxins += "nothing.\n";
            return toxins;
        }

        int idx = 0;
        foreach (ToxinAmount toxin in _microbe.toxins)
        {
            toxins += $"{toxin.toxinName}";

            // Proper grammar for last entry
            if (idx == _microbe.toxins.Count - 1)
            {
                toxins += ".";
                break;
            }

            // Proper grammar for second to last entry
            if (idx == _microbe.toxins.Count - 2)
            {
                toxins += ", and ";
                idx++;
                continue;
            }

            // Proper grammar for every other entry
            toxins += ", ";
            idx++;
        }

        toxins += "\n";
        return toxins;
    }
}
