// OrderMicrobesManager.cs
// A script which manages the order microbes menu in the tablet
// Author:  Jake Gendreau
// Date:    7/22/25

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class OrderMicrobesManager : MonoBehaviour
{
    [SerializeField] private TMP_Text _microbeText;
    private List<MicrobeSO> _curMicrobeList = new();

    // Resets the main text to say the default things on enable
    void OnEnable()
    {
        _curMicrobeList.Clear();
        UpdateText();
    }

    // Adds a microbe to the list
    public void AddMicrobeToList(MicrobeSO newMicrobe)
    {
        if (_curMicrobeList.Count >= 3 || _curMicrobeList.Contains(newMicrobe))
        {
            return;
        }

        _curMicrobeList.Add(newMicrobe);

        UpdateText();
    }

    // Removes a microbe from the list
    public void RemoveMicrobeFromList(MicrobeSO newMicrobe)
    {
        _curMicrobeList.Remove(newMicrobe);

        UpdateText();
    }

    public void PlaceOrder()
    {
        if (_curMicrobeList.Count == 0)
        {
            return;    
        }

        DroneManager.Instance.ShipMicrobesToPlayer(_curMicrobeList);
    }

    // Updates the text entries regarding the current microbe list
    private void UpdateText()
    {
        string microbeString = "";

        int numEntries = _curMicrobeList.Count;
        for (int i = 0; i < 3; i++)
        {
            string microbeName = "No Microbe.";
            if (i + 1 <= numEntries)
            {
                microbeName = _curMicrobeList[i].microbeName;
            }

            microbeString += $"{i + 1}: {microbeName}\n\n";
        }

        _microbeText.text = microbeString;
    }
}
