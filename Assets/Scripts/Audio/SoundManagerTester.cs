using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManagerTester : MonoBehaviour
{
    public void PlaySound()
    {
        SoundManager.PlaySound(this.transform, SoundType.MENU_OPEN);
    }
}
