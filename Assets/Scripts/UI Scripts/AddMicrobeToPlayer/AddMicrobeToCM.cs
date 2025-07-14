using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AddMicrobeToCM : MonoBehaviour
{
    public static AddMicrobeToCM Instance { get; private set; }

    [SerializeField] private MicrobeSO _microbeSO;

    [Space(10)]
    [SerializeField] private Image _microbeBody;
    [SerializeField] private Image _microbeFace;

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
        CarriedMicrobes _cm = GameObject.FindGameObjectWithTag("Player").GetComponent<CarriedMicrobes>();

        StringFloatPair newMicrobe = new();
        newMicrobe.name = _microbeSO.microbeName;
        newMicrobe.amount = _population;

        // Check for full with no duplicate
        if (_cm.IsFull() && !_cm.HasMicrobe(newMicrobe.name))
        {
            if (NotificationPanelManager.Instance.IsAnimating() == true)
            {
                NotificationPanelManager.Instance.UpdatePanelText("Too many microbes in backpack!");
                return;
            }

            NotificationPanelManager.Instance.ShowPanelForSeconds("Too many microbes in backpack!");
            return;
        }
        _cm.AddMicrobe(newMicrobe);

        _population = 0;
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

        _microbeBody.sprite = _microbeSO.microbeBody;
        _microbeBody.color = _microbeSO.color;

        _microbeFace.sprite = _microbeSO.microbeFace;
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

        UpdateInfo();
    }
}
