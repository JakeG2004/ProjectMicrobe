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
        LineChart microbeChart = MicrobeMenu.Instance.GetMicrobeChart();
        LineChart resourcesChart = MicrobeMenu.Instance.GetResourceChart();

        yield return null;

        var folders = StandaloneFileBrowser.OpenFolderPanel("Select folder to save graphs", "", false);

        if (folders == null || folders.Length == 0)
        {
            yield break;
        }

        string folderPath = folders[0];

        string microbePath = Path.Combine(folderPath, "microbeChart.png");
        string resourcePath = Path.Combine(folderPath, "resourceChart.png");

        // Get the shared grandparent to child the graphs to
        Transform grandparent = microbeChart.transform.parent.parent;

        // Save the graphs
        yield return MoveAndSaveGraph(microbeChart, grandparent, microbePath);
        yield return MoveAndSaveGraph(resourcesChart, grandparent, resourcePath);
    }

    private IEnumerator MoveAndSaveGraph(LineChart chart, Transform grandparent, string savePath)
    {
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

        // Wait for UI to update
        yield return new WaitForEndOfFrame();
    }

    private IEnumerator ShowLegend(LineChart chart)
    {
        var legend = chart.EnsureChartComponent<Legend>();
        legend.show = true;

        legend.location.align = XCharts.Runtime.Location.Align.BottomCenter;

        chart.RefreshChart();

        yield return new WaitForEndOfFrame();
    }

    private IEnumerator HideLegend(LineChart chart)
    {
        var legend = chart.EnsureChartComponent<Legend>();
        legend.show = false;

        chart.RefreshChart();

        yield return new WaitForEndOfFrame();
    }

}
