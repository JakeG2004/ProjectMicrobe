using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MicrobeMenu : MonoBehaviour
{
    public static MicrobeMenu Instance { get; private set; }
    [SerializeField] private GameObject _menuPanel;
    private MicrobePopSim _curPylon;
    private bool _isActive = false;

    // Start is called before the first frame update
    void Start()
    {
        _menuPanel.SetActive(false);    
    }

    public void ToggleState()
    {
        _isActive = !_isActive;
        _menuPanel.SetActive(_isActive);

        if(_isActive)
        {
            GetComponent<ShowHideMouse>().ShowMouse();
            Time.timeScale = 0.0f;
        }

        else
        {
            GetComponent<ShowHideMouse>().HideMouse();
            Time.timeScale = 1.0f;
        }
    }

    public void SetCurrentPylon(GameObject pylon)
    {
        _curPylon = pylon.GetComponent<MicrobePopSim>();
    }

    public void AddMicrobe(MicrobeSO microbeSO, float quantity)
    {
        if(_curPylon.GetMicrobePopulation(microbeSO.name) != -1)
        {
            _curPylon.IncreaseMicrobePopulation(microbeSO.name, (int)quantity);
        }

        Microbe newMicrobe = new Microbe(
            initName:microbeSO.microbeName,
            initPop:quantity,
            initGrowthRate:microbeSO.growthRate,
            initCompetitors:new Dictionary<string, float>(),
            initRequiredResources:ResourceConverter.ConvertToDictionary(microbeSO.requiredResources),
            initProducedResources:ResourceConverter.ConvertToDictionary(microbeSO.producedResources),
            initToxins:ToxinConverter.ConvertToDictionary(microbeSO.toxins)
        );
        _curPylon.AddMicrobe(newMicrobe);
    }
}
