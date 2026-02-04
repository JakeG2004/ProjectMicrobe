using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeliveryMenu : GeneralMenu
{
    [SerializeField] private Toggle _hasPylonToggle;

    // Mark the delivery as complete when the menu is turned off
    public override void ToggleMenu()
    {
        base.ToggleMenu();

        if (_isActive)
        {
            bool hasPylon = DroneManager.Instance.GetCurrentDelivery().hasPylon;

            // Show the user in the delivery menu whether they are getting the pylon or not
            _hasPylonToggle.isOn = hasPylon;
            
            if (hasPylon)
            {
                GetComponent<PlayerPylonManager>().GivePylon();
            }
        }

        else
        {
            GetComponent<VoidGameEventTrigger>().TriggerEvent();
        }
    }
}
