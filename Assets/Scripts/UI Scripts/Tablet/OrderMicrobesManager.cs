// OrderMicrobesManager.cs
// A script which manages the order microbes menu in the tablet
// Author:  Jake Gendreau
// Date:    7/22/25

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OrderMicrobesManager : MonoBehaviour
{
    [SerializeField] private TMP_Text _microbeText;
    [SerializeField] private TMP_Text _orderButtonText;
    [SerializeField] private Toggle _pylonToggle;
    private MicrobeDelivery _curDelivery = new();

    // Resets the main text to say the default things on enable
    void OnEnable()
    {
        _curDelivery.curMicrobeList.Clear();
        _orderButtonText.text = "Place Order";
        _pylonToggle.isOn = false;
        UpdateText();
    }

    // Adds a microbe to the list
    public void AddMicrobeToList(MicrobeSO newMicrobe)
    {
        if (_curDelivery.curMicrobeList.Count >= 3 || _curDelivery.curMicrobeList.Contains(newMicrobe))
        {
            return;
        }

        _curDelivery.curMicrobeList.Add(newMicrobe);

        UpdateText();
    }

    // Removes a microbe from the list
    public void RemoveMicrobeFromList(MicrobeSO newMicrobe)
    {
        _curDelivery.curMicrobeList.Remove(newMicrobe);

        UpdateText();
    }

    public void PlaceOrder()
    {
        if (_curDelivery.curMicrobeList.Count == 0 && !_pylonToggle.isOn)
        {
            return;
        }

        _orderButtonText.text = "Order Placed!";

        _curDelivery.hasPylon = _pylonToggle.isOn;

        GetComponent<VoidGameEventTrigger>().TriggerEvent();
        DroneManager.Instance.ShipMicrobesToPlayer(_curDelivery);
    }

    // Updates the text entries regarding the current microbe list
    private void UpdateText()
    {
        string microbeString = "";

        int numEntries = _curDelivery.curMicrobeList.Count;
        for (int i = 0; i < 3; i++)
        {
            string microbeName = "No Microbe.";
            if (i + 1 <= numEntries)
            {
                microbeName = _curDelivery.curMicrobeList[i].microbeName;
            }

            microbeString += $"{i + 1}: {microbeName}\n\n";
        }

        _microbeText.text = microbeString;
    }
}

//
public class MicrobeDelivery
{
    public List<MicrobeSO> curMicrobeList = new();
    public bool hasPylon = false;
}
