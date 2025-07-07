using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AddMicrobeToPylon : MonoBehaviour
{
    [SerializeField] private TMP_InputField _numToAdd;
    [SerializeField] private TMP_Text _microbeName;
    [SerializeField] private Slider _slider;
    private Button _button;
    [SerializeField] private int _microbeIndex = 0;
    private MicrobeMenu _microbeMenu;
    private CarriedMicrobes _cm;

    void Awake()
    {
        _microbeMenu = GameObject.FindGameObjectWithTag("MicrobeMenu").GetComponent<MicrobeMenu>();
        _cm = GameObject.FindGameObjectWithTag("Player").GetComponent<CarriedMicrobes>();
        _button = GetComponent<Button>();
    }

    public void InsertMicrobes()
    {
        // Get a clone of the carried microbe
        StringFloatPair curMicrobe = _cm.GetMicrobe(_microbeIndex);
        float popToAdd = float.Parse(_numToAdd.text);

        // Check bounds
        if (popToAdd > curMicrobe.amount || curMicrobe.amount == 0)
        {
            Debug.Log("Invalid amount to add");
            return;
        }

        float oldPop = curMicrobe.amount;

        // Add it to the pylon
        _microbeMenu.AddMicrobe(curMicrobe, popToAdd);

        // Set the carried population by the player
        _cm.SetMicrobePopulation(curMicrobe.name, oldPop - popToAdd);

        // Update the player inventory slots
        foreach (var amtp in Object.FindObjectsOfType<AddMicrobeToPylon>())
        {
            amtp.UpdateInfo();
        }

        GetComponent<StringGameEventTrigger>().TriggerEvent(curMicrobe.name);
    }

    public void SetAddVal(float val)
    {
        _numToAdd.text = val.ToString();
    }

    public void SetSliderVal(string amt)
    {
        if (!_slider)
        {
            return;
        }

        float.TryParse(amt, out float val);

        _slider.value = val;
    }

    // Empty text when menu is brought up (go Enabled)
    public void OnEnable()
    {
        UpdateInfo();
    }

    // Updates the info of the pylon UI according to the pylon object
    public void UpdateInfo()
    {
        // Disable if not in index
        if (_cm.GetMicrobeCount() == 0 || (_microbeIndex + 1 > _cm.GetMicrobeCount()))
        {
            _microbeName.text = "No Microbe!";
            _numToAdd.text = "0";
            // _numToAdd.interactable = false;
            // _slider.interactable = false;
            // _button.interactable = false;

            return;
        }

        // Get the microbe
        StringFloatPair curMicrobe = _cm.GetMicrobe(_microbeIndex);

        // Set interactible
        _numToAdd.interactable = true;
        _slider.interactable = true;
        _button.interactable = true;

        // Fill fields
        _microbeName.text = curMicrobe.name;
        _slider.maxValue = curMicrobe.amount;
    }
}
