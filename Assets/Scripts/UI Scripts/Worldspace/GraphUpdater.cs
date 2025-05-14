using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphUpdater : MonoBehaviour
{
    [SerializeField] private PylonScreenController _psc;

    private MicrobeMenu _microbeMenu;
    void Start()
    {
        _microbeMenu = GameObject.FindGameObjectWithTag("MicrobeMenu").GetComponent<MicrobeMenu>();
    }

    // Public function called to update the graphs for UI and worldspace
    public void UpdateGraphs()
    {
        UpdatePylonScreenGraphs();
        UpdateUIGraphs();
    }

    // Update worldpsace graphs
    private void UpdatePylonScreenGraphs()
    {
        _psc.UpdateMicrobeGraphs();
    }

    // Update UI graphs
    private void UpdateUIGraphs()
    {
        _microbeMenu.UpdateCharts();
    }
}
