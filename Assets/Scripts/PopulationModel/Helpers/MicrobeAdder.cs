using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MicrobeAdder : MonoBehaviour
{
    public KeyCode addMicrobe = KeyCode.A;
    public MicrobeSO microbeSO;

    void Update()
    {
        if(Input.GetKeyDown(addMicrobe))
        {
            CarriedMicrobes _cm = GetComponent<CarriedMicrobes>();
            Microbe newMicrobe = new Microbe(
                initName:microbeSO.microbeName,
                initPop:microbeSO.population,
                initGrowthRate:microbeSO.growthRate,
                initCompetitors:new Dictionary<string, float>(),
                initRequiredResources:ResourceConverter.ConvertToDictionary(microbeSO.requiredResources),
                initProducedResources:ResourceConverter.ConvertToDictionary(microbeSO.producedResources),
                initToxins:ToxinConverter.ConvertToDictionary(microbeSO.toxins)
            );
            _cm.AddMicrobe(newMicrobe);
        }
    }
}
