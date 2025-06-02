// NotificationSender.cs
// A script for sending notifications to the NotificationPanelManager
// Author:  Jake Gendreau
// Date:    6/2/25

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotificationSender : MonoBehaviour
{
    private NotificationPanelManager _npm;

    void Start()
    {
        _npm = NotificationPanelManager.Instance;
    }
    public void SendGeneralNotification(string notifBody)
    {
        // Early return when already animating
        if (_npm.IsAnimating())
        {
            return;
        }

        // Do the animation
        _npm.ShowPanelForSeconds(notifBody);
    }

    public void SendStableStateNotif()
    {
        string notifString = GetComponent<MicrobePopSim>().envSO.envName + " Is Now Stable!";
        SendGeneralNotification(notifString);
    }

    public void SendExtinctionNotif(string microbeName)
    {
        string notifString = microbeName + " experienced an extinction event at " + GetComponent<MicrobePopSim>().envSO.envName;
        SendGeneralNotification(notifString);
    }
}
