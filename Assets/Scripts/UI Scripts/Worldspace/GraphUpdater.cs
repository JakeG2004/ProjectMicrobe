using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphUpdater : MonoBehaviour
{
    [SerializeField] private PylonScreenController _psc;
    private MicrobeMenu _microbeMenu;

    [SerializeField] private List<string> _dontGraphTheseResources;

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
        _psc.UpdateMicrobeGraphs(_dontGraphTheseResources);
    }

    // Update UI graphs
    private void UpdateUIGraphs()
    {
        // Check that the microbe menu current pylon is equal to the current pylon
        if (_microbeMenu && (_microbeMenu.GetCurrentPylon() == GetComponent<MicrobePopSim>()))
        {
            _microbeMenu.UpdateCharts(_dontGraphTheseResources);
        }
    }
}
