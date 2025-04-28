using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddMicrobeToggler : MonoBehaviour
{
    [SerializeField] private MicrobeSO _microbeSO;
    [SerializeField] private float _maxPopulation = 100;
    [SerializeField] private float _curPopulation = 50;
    [SerializeField] private float _updatePeriod = 15.0f;

    private float _elapsedTime = 0.0f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        _elapsedTime += Time.deltaTime;
        if(_elapsedTime >= _updatePeriod)
        {
            _elapsedTime = 0.0f;

            _curPopulation = _maxPopulation;
        }
    }

    public void ToggleMenu()
    {
        AddMicrobeToCM.Instance.SetMicrobe(_microbeSO, _curPopulation, this);
        AddMicrobeToPlayerMenu.Instance.ToggleState();
    }

    public void SetPopulation(float pop)
    {
        _curPopulation = pop;
    }
}
