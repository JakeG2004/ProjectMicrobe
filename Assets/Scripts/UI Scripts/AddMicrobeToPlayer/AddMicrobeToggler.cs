using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddMicrobeToggler : MonoBehaviour
{
    [SerializeField] private MicrobeSO _microbeSO;
    [SerializeField] private float _maxPopulation = 100;
    [SerializeField] private float _curPopulation = 50;

    public void ToggleMenu()
    {
        AddMicrobeToPlayerMenu.Instance.ToggleState();
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
