using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DeliveryMenuManager : MonoBehaviour
{
    [SerializeField] private Button[] _microbeEntries;
    private const float ADD_AMT = 5;
    private IndividualMicrobeCtrl[] _allMicrobeControls;
    private List<MicrobeSO> _microbes = new();
    private StringGameEventTrigger _gotMicrobeTrigger;

    void Awake()
    {
        _gotMicrobeTrigger = GetComponent<StringGameEventTrigger>();
    }

    // Subscribe to the button click events
    void OnEnable()
    {
        _allMicrobeControls = Object.FindObjectsOfType<IndividualMicrobeCtrl>();

        MicrobeDelivery delivery = DroneManager.Instance.GetCurrentDelivery();

        _microbes = delivery.curMicrobeList;

        SetDeliveryPanel();
    }

    private void SetDeliveryPanel()
    {
        // Iterate through each microbe in the delivery
        int microbeCount = _microbes.Count;
        for (int i = 0; i < 3; i++)
        {
            TMP_Text microbeText = _microbeEntries[i].GetComponentInChildren<TMP_Text>();

            foreach (Transform child in _microbeEntries[i].transform.parent)
            {
                if (child.gameObject.name == "MicrobeTitle")
                {
                    microbeText = child.GetChild(0).GetComponent<TMP_Text>();
                    break;
                }
            }

            // Assign microbe to menu page
            if (i < microbeCount)
            {
                MicrobeSO curMicrobe = _microbes[i];
                _microbeEntries[i].onClick.AddListener(() => AddMicrobeToPlayer(curMicrobe));
                _microbeEntries[i].onClick.AddListener(() => _gotMicrobeTrigger.TriggerEvent(curMicrobe.microbeName));
                microbeText.text = curMicrobe.microbeName;
            }

            // Assign empty
            else
            {
                microbeText.text = "No Microbe.";
            }
        }
    }

    // Unsubscribe from the button click events
    void OnDisable()
    {
        UnsubscribeFromButtons();
    }

    private void UnsubscribeFromButtons()
    {
        for (int i = 0; i < 3; i++)
        {
            _microbeEntries[i].onClick.RemoveAllListeners();
        }
    }

    // Creates a stringfloatpair from the microbeso, then sends it off to get added
    private void AddMicrobeToPlayer(MicrobeSO microbe)
    {
        StringFloatPair newMicrobe = new();
        newMicrobe.name = microbe.microbeName;
        newMicrobe.amount = ADD_AMT;

        CarriedMicrobes cm = GameObject.FindGameObjectWithTag("Player").GetComponent<CarriedMicrobes>();

        cm.AddMicrobe(newMicrobe);

        if (cm.backpackFull)
        {
            return;
        }

        // Update the backpack panel to show the new amount of _microbes
        foreach (IndividualMicrobeCtrl imc in _allMicrobeControls)
        {
            imc.UpdateInfo();
        }

        // Remove the microbe from the delivery panel
        RemoveMicrobeFromDeliveryPanel(microbe);
    }

    private void RemoveMicrobeFromDeliveryPanel(MicrobeSO microbe)
    {
        UnsubscribeFromButtons();
        _microbes.Remove(microbe);

        SetDeliveryPanel();
    }
}
