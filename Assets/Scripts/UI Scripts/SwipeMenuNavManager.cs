// SwipeMenuNavManager.cs
// A script to manage navigation between two side-by-side swipe menus
// Author:  Jake Gendreau
// Date:    8/15/25

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwipeMenuNavManager : MonoBehaviour
{
    [SerializeField] private SwipeMenuManager _leftMenu;
    [SerializeField] private SwipeMenuManager _rightMenu;

    public void SetOtherMenuNav(SwipeMenuManager menu, Selectable selectable)
    {
        if (menu == _rightMenu)
        {
            SetLeftMenuNav(selectable);
        }

        else if (menu == _leftMenu)
        {
            SetRightMenuNav(selectable);
        }
    }

    private void SetRightMenuNav(Selectable selectable)
    {
        Navigation newNav = new Navigation();
        newNav.mode = Navigation.Mode.Explicit;
        newNav.selectOnLeft = selectable;
        _rightMenu.SetNav(newNav);
    }

    private void SetLeftMenuNav(Selectable selectable)
    {
        Navigation newNav = new Navigation();
        newNav.mode = Navigation.Mode.Explicit;
        newNav.selectOnRight = selectable;
        _leftMenu.SetNav(newNav);
    }
}
