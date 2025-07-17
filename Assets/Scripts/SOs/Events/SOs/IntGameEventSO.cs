/*
Based on: 
https://blog.devgenius.io/scriptableobject-game-events-1f3401bbde72

Extended from BaseGameEvent to handle Integers

Author: Jake Gendreau
Date:   5/15/25
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "IntGameEventSO", menuName = "ScriptableObjects/Events/IntGameEventSO")]
public class IntGameEventSO : BaseGameEventSO<int>
{

}
