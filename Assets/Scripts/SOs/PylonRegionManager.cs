using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PylonRegionManager : MonoBehaviour
{
    public void EnablePylonPlacement(string regionName)
    {
        SetPylonPlacementStatus(regionName, true);
    }

    public void DisablePylonPlacement(string regionName)
    {
        SetPylonPlacementStatus(regionName, false);
    }

    private void SetPylonPlacementStatus(string regionName, bool state)
    {
        foreach (Transform child in transform)
        {
            if (child.gameObject.name == regionName)
            {
                child.gameObject.GetComponent<PylonRegion>().SetCanPlacePylon(state);
                return;
            }
        }

        Debug.Log("Failed to find pylon region with name " + regionName);
    }
}
