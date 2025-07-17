/*
Based on: 
https://blog.devgenius.io/scriptableobject-game-events-1f3401bbde72

Extended from BaseGameEvent to handle Objectives

Author: Jake Gendreau
Date:   5/16/25
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "StringGameEventSO", menuName = "ScriptableObjects/Events/StringGameEventSO")]
public class StringGameEventSO : BaseGameEventSO<string>
{

}
