using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XCharts.Runtime;

public class MicrobeMenu : MonoBehaviour
{
    public static MicrobeMenu Instance { get; private set; }
    [SerializeField] private GameObject _menuPanel;
    [SerializeField] private LineChart _chart;
    private MicrobePopSim _curPylon;
    private bool _isActive = false;

    // Start is called before the first frame update
    void Start()
    {
        _menuPanel.SetActive(false);   

        // Set chart parameters
        _chart.EnsureChartComponent<Title>().show = true;
        _chart.EnsureChartComponent<Title>().text = "Microbe Populations";
        _chart.EnsureChartComponent<Tooltip>().show = true;
        _chart.EnsureChartComponent<Legend>().show = true;

        // Assign x and y axis
        var xAxis = _chart.EnsureChartComponent<XAxis>();
        var yAxis = _chart.EnsureChartComponent<YAxis>();

        // Show axis
        xAxis.show = true;
        yAxis.show = true;
    }

    public void ToggleState()
    {
        _isActive = !_isActive;
        _menuPanel.SetActive(_isActive);

        if(_isActive)
        {
            // Clear the chart
            _chart.RemoveData();
            foreach(Microbe microbe in _curPylon.GetMicrobes())
            {
                _chart.AddSerie<Line>(microbe.microbeName);
                foreach(float pop in microbe.popHistory)
                {
                    _chart.AddData(microbe.microbeName, pop);
                }
            }

            GetComponent<ShowHideMouse>().ShowMouse();
            //Time.timeScale = 0.0f;
        }

        else
        {
            GetComponent<ShowHideMouse>().HideMouse();
            //Time.timeScale = 1.0f;
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
