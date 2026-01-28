using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddMicrobeToggler : MonoBehaviour
{
    [SerializeField] private MicrobeSO _microbeSO;
    [SerializeField] private float _maxPopulation = 5;
    [SerializeField] private float _curPopulation = 5;

    public void SetMicrobeSO(MicrobeSO microbe)
    {
        _microbeSO = microbe;
    }

    public void PopulateMenuData()
    {
        AddMicrobeToPlayerMenu.Instance.ToggleMenu();
        AddMicrobeToCM.Instance.SetMicrobe(_microbeSO, _curPopulation, this);
    }

    public void SetPopulation(float pop)
    {
        _curPopulation = pop;
    }

    public void FillPopulation()
    {
        _curPopulation = _maxPopulation;
    }
}
