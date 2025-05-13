using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XCharts.Runtime;

public class MicrobeMenu : MonoBehaviour
{
    public static MicrobeMenu Instance { get; private set; }
    [SerializeField] private GameObject _menuPanel;
    [SerializeField] private LineChart _microbesChart;
    [SerializeField] private LineChart _resourcesChart;
    [SerializeField] private int _graphEntries = 10;
    private MicrobePopSim _curPylon;
    private bool _isActive = false;

    // Start is called before the first frame update
    void Start()
    {
        _menuPanel.SetActive(false);   

        InitChart(_microbesChart, "Microbe Populations");
        InitChart(_resourcesChart, "Resource Amounts");
    }

    void InitChart(LineChart chart, string name)
    {
        // Set chart parameters
        chart.EnsureChartComponent<Title>().show = true;
        chart.EnsureChartComponent<Title>().text = name;
        chart.EnsureChartComponent<Tooltip>().show = true;
        chart.EnsureChartComponent<Legend>().show = true;

        // Assign x and y axis
        var xAxis = chart.EnsureChartComponent<XAxis>();
        var yAxis = chart.EnsureChartComponent<YAxis>();

        // Show axis
        xAxis.show = true;
        yAxis.show = true;
    }

    public void ToggleState()
    {
        _isActive = !_isActive;
        _menuPanel.SetActive(_isActive);

        // Set UI control state
        GetComponent<ToggleCameraTracking>()?.SetCameraTracking(!_isActive);
        MovementController.instance.SetMovementState(!_isActive);
        GetComponent<ShowHideMouse>()?.SetState(_isActive);  

        if(_isActive)
        {
            SetMicrobeChartData();
            SetResourcesChartData();
            //_curPylon = null;
        }
    }

    public void SetMicrobeChartData()
    {
        // Clear the chart
        _microbesChart.RemoveData();

        // Iterate through each microbe
        foreach(Microbe microbe in _curPylon.GetMicrobes())
        {
            // Add a line for the microbe
            _microbesChart.AddSerie<Line>(microbe.microbeName);

            // Show a maximum of 10 timesteps at a time
            int numElements = microbe.popHistory.Count;
            if(numElements > _graphEntries)
            {
                for(int i = numElements - _graphEntries; i < numElements; i++)
                {
                    _microbesChart.AddData(microbe.microbeName, microbe.popHistory[i]);
                }
            }

            else
            {
                foreach(float pop in microbe.popHistory)
                {
                    _microbesChart.AddData(microbe.microbeName, pop);
                }
            }
        }
    }

    public void SetResourcesChartData()
    {
        _resourcesChart.RemoveData();

        foreach(var res in _curPylon.GetEnv().resourceHistory)
        {
            // Add line for resource
            _resourcesChart.AddSerie<Line>(res.Key);

            int numElements = res.Value.Count;
            if(numElements > _graphEntries)
            {
                for(int i = numElements - _graphEntries; i < numElements; i++)
                {
                    // Prevent graph from forming the weird bumps with floats. Flatten small numbers :)
                    float val = res.Value[i];
                    if(val < 0.1f)
                    {
                        val = 0.0f;
                    }

                    _resourcesChart.AddData(res.Key, val);
                }
            }

            else
            {
                foreach(var resAmt in res.Value)
                {
                    float val = resAmt;
                    if(resAmt < 0.1f)
                    {
                        val = 0.0f;
                    }
                    _resourcesChart.AddData(res.Key, val);
                }
            }
        }
    }

    public void SetCurrentPylon(GameObject pylon)
    {
        if(!pylon)
        {
            Debug.Log("No valid gameobjct passed.");
        }

        _curPylon = pylon.GetComponent<MicrobePopSim>();

        Debug.Log(_curPylon);
    }

    public void AddMicrobe(Microbe microbe, float population)
    {
        microbe.population = population;

        if(!_curPylon)
        {
            Debug.Log("Failed to get cur pylon");
        }

        if(_curPylon.GetMicrobePopulation(microbe.microbeName) != -1)
        {
            _curPylon.IncreaseMicrobePopulation(microbe.microbeName, microbe.population);
            return;
        }

        _curPylon.AddMicrobe(microbe);
    }

    public bool HasPylon()
    {
        return (_curPylon != null);
    }
}
