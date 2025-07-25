using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegionSaveManager
{
    private SaveObject _currentState;

    public RegionSaveManager(SaveObject state)
    {
        _currentState = state;
    }

    public void UpdateState(SaveObject state)
    {
        _currentState = state;
    }

    // Saves the regions
    public void SaveRegions()
    {
        // Get each pylon
        MicrobePopSim[] sims = Object.FindObjectsOfType<MicrobePopSim>();

        // Check for empty, early return
        if (sims.Length == 0)
        {
            return;
        }

        // Get each of them, adding to the list
        foreach (MicrobePopSim sim in sims)
        {
            string envName = sim.GetEnvSO().envName;

            bool foundRegion = false;
            // Entry already exists
            foreach (RegionData region in _currentState.regionData)
            {
                if (region.regionName == envName)
                {
                    GetDataFromRegion(region, sim);
                    foundRegion = true;
                    break;
                }
            }

            // Prevent repeat entries
            if (foundRegion)
            {
                continue;
            }

            // Add new entry
            RegionData newRegion = new();
            newRegion.regionName = envName;

            GetDataFromRegion(newRegion, sim);

            // Add it to the list
            _currentState.regionData.Add(newRegion);
        }
    }

    // Helper function to get data from the regions
    private void GetDataFromRegion(RegionData region, MicrobePopSim sim)
    {
        // Copy the transform
        region.pylonPosition = sim.gameObject.transform.position;
        region.pylonRotation = sim.gameObject.transform.rotation;
        region.isActive = sim.gameObject.activeInHierarchy;

        // Copy the current microbes
        foreach (Microbe microbe in sim.GetMicrobes())
        {
            string name = microbe.microbeName;
            float population = microbe.population;

            bool foundMicrobe = false;
            foreach (StringFloatPair data in region.microbes)
            {
                if (data.name == name)
                {
                    data.amount = population;
                    foundMicrobe = true;
                    break;
                }
            }

            if (foundMicrobe)
            {
                continue;
            }

            // Create a new microbeSOPopPair to store the current populations
            StringFloatPair mnpp = new();
            mnpp.name = name;
            mnpp.amount = population;

            region.microbes.Add(mnpp);
        }

        // Copy the current environment
        foreach (var res in sim.GetEnv().resources)
        {
            // Get the values from the dictionary
            string name = res.Key;
            float amount = res.Value;

            bool foundResource = false;

            // Overwrite existing entry if exists
            foreach (StringFloatPair resourceEntry in region.resources)
            {
                if (resourceEntry.name == name)
                {
                    foundResource = true;
                    resourceEntry.amount = amount;
                    break;
                }
            }

            // Go to next resource if resource was found
            if (foundResource)
            {
                continue;
            }

            // Create a new StringFloatPair with the data
            StringFloatPair sfp = new();
            sfp.name = name;
            sfp.amount = amount;

            region.resources.Add(sfp);
        }

        // Get the health history
        region.healthHistory = sim.gameObject.GetComponent<PylonStatusEventsChecker>().GetHealthHist();
        region.mycorrhisArray = sim.GetMycorrhisArray();
    }

    // Loads the data from the regions
    public void LoadRegions()
    {
        PylonRegion[] regions = Object.FindObjectsOfType<PylonRegion>();

        // Early return if no regions
        if (regions.Length == 0)
        {
            return;
        }

        foreach (RegionData region in _currentState.regionData)
        {
            // Get the region name
            string regionName = region.regionName;

            // Find the region which corresponds to the pylon
            foreach (PylonRegion pylonRegion in regions)
            {
                if (pylonRegion.GetEnvSO().envName == regionName)
                {
                    // Instance a new pylon
                    GameObject newPylon = Object.Instantiate(pylonRegion.GetPylonPrefab());

                    // Set the region to correspond to the newPylon
                    pylonRegion.SetRegionPylon(newPylon);

                    // Set the new pylon transform
                    newPylon.transform.position = region.pylonPosition;
                    newPylon.transform.rotation = region.pylonRotation;

                    // Set the new pylon information
                    MicrobePopSim sim = newPylon.GetComponent<MicrobePopSim>();
                    sim.SetEnv(pylonRegion.GetEnvSO());

                    // Set the region for the PSED
                    newPylon.GetComponent<PylonStatusEventsChecker>().SetRegion(pylonRegion);

                    // Set microbes
                    foreach (StringFloatPair mnpp in region.microbes)
                    {
                        sim.QueueMicrobePop(mnpp);
                    }

                    // Set resources
                    foreach (StringFloatPair sfp in region.resources)
                    {
                        sim.QueueResourceAmount(sfp);
                    }

                    // Set health hist
                    sim.gameObject.GetComponent<PylonStatusEventsChecker>().SetHealthHist(region.healthHistory);
                    sim.SetMycorrhisArray(region.mycorrhisArray);

                    // Set the on/off state
                    pylonRegion.gameObject.SetActive(region.isActive);
                }
            }
        }
    }
}
