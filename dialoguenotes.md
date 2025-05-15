# Pakicetus Objective system

## Process
1) Create "ObjectiveItem"
2) ObjectiveItem activates
	1) The ObjectiveItem is raised on the objectiveAddChannelSO for a listener to intercept
	2) If its complete, its marked as complete
	3) If the text length > 0, then raise it to objectivefeedback channel
	4) Set the objective to active
	5) Activate its onactivation event
3) PopupCreator gets the Objective through the channelSO
4) Creates a popup from prefab
5) Populate prefab with the information from the objective

https://blog.devgenius.io/scriptableobject-game-events-1f3401bbde72
