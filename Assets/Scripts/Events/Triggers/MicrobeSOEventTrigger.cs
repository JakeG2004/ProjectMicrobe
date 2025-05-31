/*
Based on: 
https://blog.devgenius.io/scriptableobject-game-events-1f3401bbde72

Script for raising ObjectiveGameEvents

Author: Jake Gendreau
Date:   5/16/25
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MicrobeSOEventTrigger : MonoBehaviour
{
    [SerializeField] private MicrobeGameEventSO _event;
    [SerializeField] private TMP_Text _microbeName;
    private MicrobeSO _thisMicrobeSO;
    public void TriggerGeneralEvent(MicrobeSO item)
    {
        if (_event == null)
        {
            return;
        }

        _event.Raise(item);
    }

    public void TriggerStoredSO()
    {
        TriggerGeneralEvent(_thisMicrobeSO);
    }

    public void SetStoredSO(MicrobeSO newMicrobeSO)
    {
        _thisMicrobeSO = newMicrobeSO;

        if(_microbeName)
        {
            _microbeName.text = newMicrobeSO.microbeName;
        }
    }
}
