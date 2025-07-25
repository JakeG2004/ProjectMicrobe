using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackpackSaveManager
{
    private SaveObject _currentState;

    public BackpackSaveManager(SaveObject state)
    {
        _currentState = state;
    }

    public void UpdateState(SaveObject state)
    {
        _currentState = state;
    }

    public void SavePlayerBackpack()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player == null)
        {
            return;
        }

        CarriedPylon cp = player.GetComponent<CarriedPylon>();
        CarriedMicrobes cm = player.GetComponent<CarriedMicrobes>();

        // Early return for no player
        if (player == null || cp == null || cm == null)
        {
            return;
        }

        // Pick up whether the player has a pylon
        _currentState.backpack.hasPylon = cp.HasPylon();

        // Pick up the microbes from the player
        _currentState.backpack.carriedMicrobes = cm.GetMicrobes();
    }

    public void LoadPlayerBackpack()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player == null)
        {
            return;
        }

        CarriedMicrobes cm = player.GetComponent<CarriedMicrobes>();
        CarriedPylon cp = player.GetComponent<CarriedPylon>();

        // Early return if references not found
        if (cm == null || cp == null)
        {
            return;
        }

        cp.SetHasPylon(_currentState.backpack.hasPylon);

        foreach (StringFloatPair microbe in _currentState.backpack.carriedMicrobes)
        {
            cm.AddMicrobe(microbe);
        }
    }
}
