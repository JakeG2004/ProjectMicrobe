using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XCharts.Runtime;

public class MicrobeMenu : MonoBehaviour
{
    // Singleton instance
    public static MicrobeMenu Instance { get; private set; }

    // Graphing variables
    [SerializeField] private List<string> _dontGraphTheseResources = new();
    [SerializeField] private LineChart _microbesChart;
    [SerializeField] private LineChart _resourcesChart;
    [SerializeField] private int _graphEntries = 10;

    // Env health slider
    [SerializeField] private CustomSlider _envHealthSlider;

    // Menu UI Elements
    [SerializeField] private GameObject _menuPanel;
    [SerializeField] private LegendManager _microbeLegend;
    [SerializeField] private LegendManager _envLegend;

    // Helpers
    private BoolGameEventTrigger _menuStateTracker;
    private MicrobePopSim _curPylon;
    private bool _isActive = false;

    // Start is called before the first frame update
    void Start()
    {
        _menuPanel.SetActive(false);

        InitChart(_microbesChart, "Microbe Populations");
        InitChart(_resourcesChart, "Resource Amounts");

        _menuStateTracker = GetComponent<BoolGameEventTrigger>();
    }

    void InitChart(LineChart chart, string name)
    {
        // Set chart parameters
        chart.EnsureChartComponent<Title>().show = true;
        chart.EnsureChartComponent<Title>().text = name;
        chart.EnsureChartComponent<Tooltip>().show = true;
        chart.EnsureChartComponent<Legend>().show = false;

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

        // Set menu state
        _menuPanel.SetActive(_isActive);

        // Set UI control state
        GetComponent<ToggleCameraTracking>()?.SetCameraTracking(!_isActive);
        MovementController.instance.SetMovementState(!_isActive);
        GetComponent<ShowHideMouse>()?.SetState(_isActive);

        // Update the menu open indicator
        _menuStateTracker.TriggerEvent(_isActive);

        // Update the charts
        UpdateCharts(_dontGraphTheseResources);
    }

    public void UpdateCharts(List<string> dontGraphTheseResources)
    {
        _dontGraphTheseResources = dontGraphTheseResources;

        if (_isActive)
        {
            SetMicrobeChartData();
            SetResourcesChartData(dontGraphTheseResources);
            SetEnvHealthSlider();
        }
    }

    public void SetMicrobeChartData()
    {
        // Variables for setting the legends
        ThemeStyle theme = _microbesChart.theme;

        // Destroy the existing legend entries
        _microbeLegend.DestroyEntries();

        int curIndex = 0;

        float max = 0.0f;

        // Clear the chart
        _microbesChart.RemoveData();

        // Iterate through each microbe
        foreach (Microbe microbe in _curPylon.GetMicrobes())
        {
            // Add a line for the microbe
            _microbesChart.AddSerie<Line>(microbe.microbeName);

            // Add an entry to the legend
            _microbeLegend.AddEntry(theme.colorPalette[curIndex], microbe.microbeName);

            curIndex++;

            // Show a maximum of 10 timesteps at a time
            int numElements = microbe.popHistory.Count;
            if (numElements > _graphEntries)
            {
                for (int i = numElements - _graphEntries; i < numElements; i++)
                {
                    _microbesChart.AddData(microbe.microbeName, microbe.popHistory[i]);
                    if (microbe.popHistory[i] > max)
                    {
                        max = microbe.popHistory[i];
                    }
                }
            }

            else
            {
                foreach (float pop in microbe.popHistory)
                {
                    _microbesChart.AddData(microbe.microbeName, pop);
                    if (pop > max)
                    {
                        max = pop;
                    }
                }
            }
        }

        _microbesChart.EnsureChartComponent<YAxis>().max = max;


    }

    public void SetResourcesChartData(List<string> _dontGraphTheseResources)
    {
        ThemeStyle theme = _microbesChart.theme;

        int curIndex = 0;

        _envLegend.DestroyEntries();

        _resourcesChart.RemoveData();

        float max = 0.0f;

        foreach (var res in _curPylon.GetEnv().resourceHistory)
        {
            if (_dontGraphTheseResources.Contains(res.Key))
            {
                continue;
            }

            _envLegend.AddEntry(theme.colorPalette[curIndex], res.Key);
            curIndex++;

            // Add line for resource
            _resourcesChart.AddSerie<Line>(res.Key);

            int numElements = res.Value.Count;
            if (numElements > _graphEntries)
            {
                for (int i = numElements - _graphEntries; i < numElements; i++)
                {
                    // Prevent graph from forming the weird bumps with floats. Flatten small numbers :)
                    float val = res.Value[i];
                    if (val < 0.2f)
                    {
                        val = 0.0f;
                    }

                    if (val > max)
                    {
                        max = val;
                    }

                    _resourcesChart.AddData(res.Key, val);
                }
            }

            else
            {
                foreach (var resAmt in res.Value)
                {
                    float val = resAmt;
                    if (resAmt < 0.5f)
                    {
                        val = 0.0f;
                    }
                    _resourcesChart.AddData(res.Key, val);

                    if (resAmt > max)
                    {
                        max = resAmt;
                    }
                }
            }
        }

        if (max > 50)
        {
            max = 50;
        }

        _resourcesChart.EnsureChartComponent<YAxis>().max = max;
    }

    public void SetCurrentPylon(GameObject pylon)
    {
        if (!pylon)
        {
            Debug.Log("No valid gameobjct passed.");
        }

        _curPylon = pylon.GetComponent<MicrobePopSim>();
    }

    public MicrobePopSim GetCurrentPylon()
    {
        return _curPylon;
    }

    public void AddMicrobe(Microbe microbe, float population)
    {
        microbe.population = population;

        if (!_curPylon)
        {
            Debug.Log("Failed to get cur pylon");
        }

        if (_curPylon.GetMicrobePopulation(microbe.microbeName) != -1)
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

    public void SetEnvHealthSlider()
    {
        // Get the pylon status event checker
        PylonStatusEventsChecker psec = _curPylon.gameObject.GetComponent<PylonStatusEventsChecker>();
        float envHealth = psec.GetEnvHealth();
        _envHealthSlider.SetSliderFill(envHealth);
    }
}
