using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AddMicrobeToCM : MonoBehaviour
{
    public static AddMicrobeToCM Instance { get; private set; }

    [SerializeField] private MicrobeSO _microbeSO;
    [SerializeField] private TMP_InputField _microbeAmt;

    // Start is called before the first frame update
    void Start()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }

        else
        {
            Instance = this;
        }
    }

    public void AddMicrobeToPlayer()
    {
        if(!float.TryParse(_microbeAmt.text, out float amtToAdd))
        {
            Debug.LogWarning("Failed to parse string");
            return;
        }
        
        CarriedMicrobes _cm = GameObject.FindGameObjectWithTag("Player").GetComponent<CarriedMicrobes>();
        Microbe newMicrobe = Microbe.CreateMicrobeFromSO(_microbeSO);
        newMicrobe.population = amtToAdd;
        _cm.AddMicrobe(newMicrobe);
        
        // Update the player inventory slots
        foreach(var im in Object.FindObjectsOfType<IndividualMicrobeCtrl>())
        {
            im.UpdateInfo();
        }
    }

    public void UpdateInfo()
    {
        TMP_Text _microbeName = GetComponent<TMP_Text>();
        _microbeName.text = _microbeSO.microbeName;
    }

    void OnEnable()
    {
        UpdateInfo();
    }
}
