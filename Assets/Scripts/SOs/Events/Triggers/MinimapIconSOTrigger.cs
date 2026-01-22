/*
Based on: 
https://blog.devgenius.io/scriptableobject-game-events-1f3401bbde72

Script for raising ObjectiveGameEvents

Author: Jake Gendreau
Date:   1/22/26
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapIconSOTrigger : MonoBehaviour
{
    [SerializeField] private MinimapIconSO _addIconEvent;
    [SerializeField] private MinimapIconSO _removeIconEvent;

    public void AddIcon(MinimapIcon item)
    {
        TriggerEvent(item, _addIconEvent);
    }

    public void RemoveIcon(MinimapIcon item)
    {
        TriggerEvent(item, _removeIconEvent);
    }

    private void TriggerEvent(MinimapIcon item, MinimapIconSO curEvent)
    {
        if (curEvent == null)
        {
            return;
        }

        curEvent.Raise(item);
    }
}
