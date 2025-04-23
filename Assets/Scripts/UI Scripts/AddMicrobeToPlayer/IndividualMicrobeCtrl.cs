using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class IndividualMicrobeCtrl : MonoBehaviour
{
    [SerializeField] private int _microbeIndex = 0;
    private TMP_Text _microbeText;
    private CarriedMicrobes _cm;

    // Start is called before the first frame update
    void Start()
    {
        _microbeText = GetComponent<TMP_Text>();
        _cm = GameObject.FindGameObjectWithTag("Player").GetComponent<CarriedMicrobes>();
    }

    public void UpdateInfo()
    {
        // Assign as needed
        if(!_cm || !_microbeText)
        {
            _cm = GameObject.FindGameObjectWithTag("Player").GetComponent<CarriedMicrobes>();
            _microbeText = GetComponent<TMP_Text>();
        }

        // Handle empty microbes
        if(_cm.GetMicrobeCount() == 0 || (_microbeIndex + 1 > _cm.GetMicrobeCount()))
        {
            _microbeText.text = "No microbe!";
            return;
        }

        // Assign information as necessary
        Microbe curMicrobe = _cm.GetMicrobe(_microbeIndex);
        _microbeText.text = curMicrobe.microbeName + ": " + curMicrobe.population.ToString();
    }

    void OnEnable()
    {
        UpdateInfo();
    }

    public void RemoveMicrobe()
    {
        _cm.RemoveMicrobe(_cm.GetMicrobe(_microbeIndex).microbeName);
        foreach(var im in Object.FindObjectsOfType<IndividualMicrobeCtrl>())
        {
            im.UpdateInfo();
        }
    }
}
