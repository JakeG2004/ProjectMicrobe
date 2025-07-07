using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameRateChecker : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log($"VSync: {QualitySettings.vSyncCount}");
        Debug.Log($"Target FPS: {Application.targetFrameRate}");   
    }
}
