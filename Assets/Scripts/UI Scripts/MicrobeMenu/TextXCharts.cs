using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XCharts.Runtime;

public class TextXCharts : MonoBehaviour
{
    public LineChart chart;

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddData()
    {
        // Show the title
        chart.EnsureChartComponent<Title>().show = true;

        // Rename the chart
        chart.EnsureChartComponent<Title>().text = "Test Chart";

        // Show tooltips and legend
        chart.EnsureChartComponent<Tooltip>().show = true;
        chart.EnsureChartComponent<Legend>().show = true;

        // Assign x and y axis
        var xAxis = chart.EnsureChartComponent<XAxis>();
        var yAxis = chart.EnsureChartComponent<YAxis>();

        // Show axis
        xAxis.show = true;
        yAxis.show = true;

        // Assing types (?)
        xAxis.type = Axis.AxisType.Category;
        yAxis.type = Axis.AxisType.Value;

        // ??
        //xAxis.splitNumber = 10;
        //xAxis.boundaryGap = true;

        // Remove any existing data
        chart.RemoveData();

        // Add a new line
        chart.AddSerie<Line>("Microbe1");
        chart.AddSerie<Line>("Microbe2");

        for(int i = 0; i < 20; i++)
        {
            chart.AddXAxisData(i.ToString());
            chart.AddData("Microbe1", Random.Range(10, 20));
            chart.AddData("Microbe2", Random.Range(10, 20));
        }
    }
}
