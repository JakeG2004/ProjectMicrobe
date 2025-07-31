using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryMenu : GeneralMenu
{
    // Mark the delivery as complete when the menu is turned off
    public override void ToggleMenu()
    {
        base.ToggleMenu();

        if (!_isActive)
        {
            GetComponent<VoidGameEventTrigger>().TriggerEvent();
        }
    }
}
