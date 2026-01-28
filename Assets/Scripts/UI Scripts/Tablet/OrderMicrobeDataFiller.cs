using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class OrderMicrobeDataFiller : MonoBehaviour
{
    [SerializeField] private MicrobeSO _microbe;
    [SerializeField] private MicrobeGameEventSO _addMicrobeChannel;
    [SerializeField] private MicrobeGameEventSO _removeMicrobeChannel;

    // Start is called before the first frame update
    void Start()
    {
        foreach (Transform child in transform)
        {
            if (child.name == "MicrobeTitle")
            {
                child.GetChild(0).GetComponent<TMP_Text>().text = _microbe.microbeName;
            }

            if (child.name == "AddMicrobe")
            {
                child.GetComponent<Button>().onClick.AddListener(AddMicrobe);
            }

            if(child.name == "RemoveMicrobe")
            {
                child.GetComponent<Button>().onClick.AddListener(RemoveMicrobe);
            }
        }
    }

    private void AddMicrobe()
    {
        _addMicrobeChannel.Raise(_microbe);
    }
    
    private void RemoveMicrobe()
    {
        _removeMicrobeChannel.Raise(_microbe);
    }
}
