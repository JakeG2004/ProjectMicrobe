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
        LineChart microbeChart = MicrobeMenu.Instance.GetMicrobeChart();
        LineChart resourcesChart = MicrobeMenu.Instance.GetResourceChart();

        var folders = StandaloneFileBrowser.OpenFolderPanel("Select folder to save graphs", "", false);

        if (folders == null || folders.Length == 0)
            return;

        string folderPath = folders[0];

        string microbePath = Path.Combine(folderPath, "microbeChart.png");
        string resourcePath = Path.Combine(folderPath, "resourceChart.png");

        microbeChart.SaveAsImage("png", microbePath);
        resourcesChart.SaveAsImage("png", resourcePath);
    }
}
