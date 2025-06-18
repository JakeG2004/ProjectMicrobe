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
    private float _preventForSeconds = 1.0f;
    private bool _canShowNotifs = false;

    void Start()
    {
        _npm = NotificationPanelManager.Instance;
        StartCoroutine(IPreventStartNotifs());
    }

    private IEnumerator IPreventStartNotifs()
    {
        float elapsedTime = 0.0f;

        while (elapsedTime <= _preventForSeconds)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        _canShowNotifs = true;
    }

    public void SendGeneralNotification(string notifBody)
    {
        if (!_canShowNotifs)
        {
            return;
        }

        // Early return when already animating
        if (!_npm || _npm.IsAnimating())
        {
            _npm = NotificationPanelManager.Instance;
        }

        // Do the animation
        _npm.ShowPanelForSeconds(notifBody);
    }

    public void SendStableStateNotif(string envName)
    {
        string notifString = envName + " Is Now Stable!";
        SendGeneralNotification(notifString);
    }

    public void SendExtinctionNotif(string microbeName)
    {
        string notifString = microbeName + " experienced an extinction event at " + GetComponent<MicrobePopSim>().GetEnvSO().envName;
        SendGeneralNotification(notifString);
    }
}
