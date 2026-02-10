using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XCharts.Runtime;
using System.IO;
using SFB;

public class ExportGraphs : MonoBehaviour
{
    public void SaveGraphs()
    {
        StartCoroutine(SaveGraphsCoroutine());
    }

    // Use an asynchronous process to force things to happen in order
    private IEnumerator SaveGraphsCoroutine()
    {
        List<BaseChart> charts = MicrobeMenu.Instance.GetCharts();

        yield return null;

        var folders = StandaloneFileBrowser.OpenFolderPanel("Select folder to save graphs", "", false);

        if (folders == null || folders.Length == 0)
        {
            yield break;
        }

        string folderPath = folders[0];

        // Get the shared grandparent to child the graphs to
        Transform grandparent = charts[0].transform.parent.parent;

        // Keep track of and disable charts so they dont interfere
        List<bool> chartEnabledStates = new();
        foreach (BaseChart chart in charts)
        {
            chartEnabledStates.Add(chart.gameObject.activeSelf);
            chart.gameObject.SetActive(false);
        }

        // Save the graphs
        foreach (BaseChart chart in charts)
        {
            string path = Path.Combine(folderPath, chart.gameObject.name + ".png");
            yield return MoveAndSaveGraph(chart, grandparent, path);
        }

        // Reset all of the chart enabled states
        for (int i = 0; i < charts.Count; i++)
        {
            charts[i].gameObject.SetActive(chartEnabledStates[i]);
        }
    }

    private IEnumerator MoveAndSaveGraph(BaseChart chart, Transform grandparent, string savePath)
    {
        chart.gameObject.SetActive(true);

        // Store the original parent and position since we have to move the graph
        Transform originalParent = chart.transform.parent;
        Vector3 originalPos = chart.transform.localPosition;

        // Move the graph into view
        chart.transform.SetParent(grandparent, false);
        chart.transform.localPosition = originalPos;

        yield return ShowLegend(chart);

        // Wait for UI to update
        yield return new WaitForEndOfFrame();

        // Save the image
        chart.SaveAsImage("png", savePath);

        yield return new WaitForEndOfFrame();

        yield return HideLegend(chart);

        // Reset the position
        chart.transform.SetParent(originalParent, false);
        chart.transform.SetAsFirstSibling();
        chart.transform.localPosition = originalPos;

        chart.gameObject.SetActive(false);

        // Wait for UI to update
        yield return new WaitForEndOfFrame();
    }

    private IEnumerator ShowLegend(BaseChart chart)
    {
        yield return SetLegendVisibility(chart, true);
    }

    private IEnumerator HideLegend(BaseChart chart)
    {
        yield return SetLegendVisibility(chart, false);
    }

    private IEnumerator SetLegendVisibility(BaseChart chart, bool state)
    {
        var legend = chart.EnsureChartComponent<Legend>();
        legend.show = state;

        if (state)
        {
            legend.location.align = XCharts.Runtime.Location.Align.BottomCenter;
        }
        
        chart.RefreshChart();

        yield return new WaitForEndOfFrame();
    }

}
