/*
* Scriptable Object to store all objective channels for universal access
*
* Author:   Jake Gendreau
* Date:     5/16/25
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ObjectiveChannelsSO", menuName = "ScriptableObjects/Events/ObjectiveChannelsSO")]
public class ObjectiveChannelsSO : ScriptableObject
{
    public ObjectiveGameEventSO objectiveAddChannelSO;
    public ObjectiveGameEventSO objectiveCompleteChannelSO;
    public ObjectiveGameEventSO objectiveFailedChannelSO;
    public ObjectiveGameEventSO objectiveSkippedSO;
}
