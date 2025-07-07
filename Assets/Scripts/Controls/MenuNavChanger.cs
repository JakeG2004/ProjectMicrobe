using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuNavChanger : MonoBehaviour
{
    [SerializeField] private List<NavObject> _navObjs;

    public void UpdateNavObjects()
    {
        foreach (NavObject no in _navObjs)
        {
            Navigation nav = no.nav;
            Navigation curNav = no.selectObj.navigation;
            
            Navigation newNav = new();
            newNav.mode = nav.mode;

            newNav.selectOnDown = nav.selectOnDown == null ? curNav.selectOnDown : nav.selectOnDown;
            newNav.selectOnLeft = nav.selectOnLeft == null ? curNav.selectOnLeft : nav.selectOnLeft;
            newNav.selectOnRight = nav.selectOnRight == null ? curNav.selectOnRight : nav.selectOnRight;
            newNav.selectOnUp = nav.selectOnUp == null ? curNav.selectOnUp : nav.selectOnUp;

            no.selectObj.navigation = newNav;
        }
    }
}

[System.Serializable]
public class NavObject
{
    [SerializeField] public Selectable selectObj;
    [SerializeField] public Navigation nav;
}
