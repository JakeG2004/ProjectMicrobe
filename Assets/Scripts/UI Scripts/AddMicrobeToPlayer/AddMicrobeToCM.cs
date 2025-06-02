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
    private float _population = 0;
    private AddMicrobeToggler _srcToggle;

    // Start is called before the first frame update
    void Awake()
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
        if (!float.TryParse(_microbeAmt.text, out float amtToAdd))
        {
            Debug.LogWarning("Failed to parse string");
            return;
        }

        if (amtToAdd > _population)
        {
            if (NotificationPanelManager.Instance.IsAnimating() == true)
            {
                return;
            }

            NotificationPanelManager.Instance.ShowPanelForSeconds("Attempting to take too many microbes!");
            return;
        }

        CarriedMicrobes _cm = GameObject.FindGameObjectWithTag("Player").GetComponent<CarriedMicrobes>();
        Microbe newMicrobe = Microbe.CreateMicrobeFromSO(_microbeSO);
        newMicrobe.population = amtToAdd;
        if (_cm.IsFull())
        {
            if (NotificationPanelManager.Instance.IsAnimating() == true)
            {
                return;
            }

            NotificationPanelManager.Instance.ShowPanelForSeconds("Too many microbes in backpack!");
            return;
        }
        _cm.AddMicrobe(newMicrobe);

        _population -= amtToAdd;
        UpdateInfo();

        // Update the player inventory slots
        foreach (var im in Object.FindObjectsOfType<IndividualMicrobeCtrl>())
        {
            im.UpdateInfo();
        }

        // Broadcast what microbe was picked up
        GetComponent<StringGameEventTrigger>().TriggerEvent(_microbeSO.microbeName);
    }

    public void UpdateInfo()
    {
        TMP_Text _microbeName = GetComponent<TMP_Text>();
        _microbeName.text = _microbeSO.microbeName + ": " + _population.ToString();
        _srcToggle?.SetPopulation(_population);
        //Debug.Log($"Set the population to {_population}");
    }

    void OnEnable()
    {
        //UpdateInfo();
    }

    public void SetMicrobe(MicrobeSO microbeSO, float population, AddMicrobeToggler src)
    {
        _microbeSO = microbeSO;
        _population = population;
        _srcToggle = src;

        _microbeAmt.text = "";
        UpdateInfo();
    }
}
