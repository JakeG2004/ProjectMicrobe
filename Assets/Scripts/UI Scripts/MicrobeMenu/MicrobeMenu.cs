using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XCharts.Runtime;
using UnityEngine.UI;

public class MicrobeMenu : GeneralMenu
{
    // Singleton instance
    public static MicrobeMenu Instance { get; private set; }

    [Space(20)]
    // Graphing variables
    [SerializeField] private List<string> _dontGraphTheseResources = new();
    [SerializeField] private LineChart _microbesLineChart;
    [SerializeField] private BarChart _microbesBarChart;
    [SerializeField] private LineChart _resourcesLineChart;
    [SerializeField] private BarChart _resourcesBarChart;
    [SerializeField] private LegendManager _microbeLegend;
    [SerializeField] private LegendManager _envLegend;
    [SerializeField] private int _graphEntries = 10;

    [Space(20)]

    // Env health slider
    [SerializeField] private CustomSlider _envHealthSlider;

    // Menu UI Elements
    [SerializeField] private GameObject _menuPanel;

    // Helpers
    private MicrobePopSim _curPylon;

    // Start is called before the first frame update
    protected override void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        else
        {
            Destroy(this.gameObject);
        }
        
        base.Start();

        InitChart(_microbesLineChart, "Microbe Populations");
        InitChart(_microbesBarChart, "Microbe Populations");
        InitChart(_resourcesLineChart, "Resource Amounts");
    }

    void InitChart(BaseChart chart, string name)
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

    // Toggles between line chart and bar chart
    public void SwitchGraphStyle()
    {
        _microbesBarChart.gameObject.SetActive(!_microbesBarChart.gameObject.activeSelf);
        _microbesLineChart.gameObject.SetActive(!_microbesLineChart.gameObject.activeSelf);

        _resourcesBarChart.gameObject.SetActive(!_resourcesBarChart.gameObject.activeSelf);
        _resourcesLineChart.gameObject.SetActive(!_resourcesLineChart.gameObject.activeSelf);
    }

    public override void ToggleMenu()
    {
        base.ToggleMenu();

        // Update the charts
        UpdateCharts(_dontGraphTheseResources);
    }

    public void DisableScrollMask()
    {
        SetScrollMask(false);
    }

    public void EnableScrollMask()
    {
        SetScrollMask(true);
    }

    private void SetScrollMask(bool state)
    {
        Mask mask = transform.GetChild(0).GetChild(0).GetComponent<Mask>();
        mask.enabled = state;
    }
    
    public List<BaseChart> GetCharts()
    {
        List<BaseChart> charts = new List<BaseChart> { _microbesLineChart, _microbesBarChart, _resourcesLineChart, _resourcesBarChart };
        return charts;
    }

    public LineChart GetMicrobeChart()
    {
        return _microbesLineChart;
    }
    
    public LineChart GetResourceChart()
    {
        return _resourcesLineChart;
    }

    public void UpdateCharts(List<string> dontGraphTheseResources)
    {
        _dontGraphTheseResources = dontGraphTheseResources;

        if (_isActive)
        {
            SetMicrobeLineChartData();
            SetResourcesLineChartData(dontGraphTheseResources);
            SetMicrobeBarChartData();
            SetResourcesBarChartData(dontGraphTheseResources);
            SetEnvHealthSlider();
        }
    }

    private void SetMicrobeBarChartData()
    {
        // Variables for setting the legends
        ThemeStyle theme = _microbesBarChart.theme;

        // Clear the chart
        _microbesBarChart.RemoveData();

        int curIdx = 0;

        // Iterate through each microbe
        foreach (Microbe microbe in _curPylon.GetMicrobes())
        {
            if (microbe.population <= 0)
            {
                curIdx++;
                continue;
            }

            // Add a line for the microbe
            Serie serie = _microbesBarChart.AddSerie<Bar>(microbe.microbeName);
            serie.itemStyle.color = theme.colorPalette[curIdx];

            int numElements = microbe.popHistory.Count;
            if (numElements > 0)
            {
                // Set its value to the last item in the population list
                _microbesBarChart.AddData(microbe.microbeName, microbe.popHistory[numElements - 1]);
            }

            curIdx++;
        }
    }
    
    private void SetResourcesBarChartData(List<string> _dontGraphTheseResources)
    {
        ThemeStyle theme = _microbesBarChart.theme;

        _resourcesBarChart.RemoveData();

        int curIdx = 0;

        foreach (var res in _curPylon.GetEnv().resourceHistory)
        {
            if (_dontGraphTheseResources.Contains(res.Key) || res.Value[res.Value.Count - 1] <= 0)
            {
                curIdx++;
                continue;
            }

            // Add line for resource
            Serie newSerie = _resourcesBarChart.AddSerie<Bar>(res.Key);
            newSerie.itemStyle.color = theme.colorPalette[curIdx];

            _resourcesBarChart.AddData(res.Key, res.Value[res.Value.Count - 1]);

            curIdx++;
        }
    }

    public void SetMicrobeLineChartData()
    {
        // Variables for setting the legends
        ThemeStyle theme = _microbesLineChart.theme;

        // Destroy the existing legend entries
        _microbeLegend.DestroyEntries();

        int curIndex = 0;

        float max = 0.0f;

        // Clear the chart
        _microbesLineChart.RemoveData();

        // Iterate through each microbe
        foreach (Microbe microbe in _curPylon.GetMicrobes())
        {
            // Add a line for the microbe
            _microbesLineChart.AddSerie<Line>(microbe.microbeName);

            // Add an entry to the legend
            _microbeLegend.AddEntry(theme.colorPalette[curIndex], microbe.microbeName);

            curIndex++;

            // Show a maximum of 10 timesteps at a time
            int numElements = microbe.popHistory.Count;
            if (numElements > _graphEntries)
            {
                for (int i = numElements - _graphEntries; i < numElements; i++)
                {
                    _microbesLineChart.AddData(microbe.microbeName, microbe.popHistory[i]);
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
                    _microbesLineChart.AddData(microbe.microbeName, pop);
                    if (pop > max)
                    {
                        max = pop;
                    }
                }
            }
        }

        _microbesLineChart.EnsureChartComponent<YAxis>().max = max;
        _microbesLineChart.EnsureChartComponent<YAxis>().min = 0;
    }

    public void SetResourcesLineChartData(List<string> _dontGraphTheseResources)
    {
        ThemeStyle theme = _microbesLineChart.theme;

        int curIndex = 0;

        _envLegend.DestroyEntries();

        _resourcesLineChart.RemoveData();

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
            _resourcesLineChart.AddSerie<Line>(res.Key);

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

                    _resourcesLineChart.AddData(res.Key, val);
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
                    _resourcesLineChart.AddData(res.Key, val);

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

        _resourcesLineChart.EnsureChartComponent<YAxis>().max = max;
        _resourcesLineChart.EnsureChartComponent<YAxis>().min = 0;
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

    public void AddMicrobe(StringFloatPair microbe, float population)
    {
        microbe.amount = population;

        if (!_curPylon)
        {
            Debug.Log("Failed to get cur pylon");
        }

        if (_curPylon.GetMicrobePopulation(microbe.name) != -1)
        {
            _curPylon.IncreaseMicrobePopulation(microbe.name, microbe.amount);
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