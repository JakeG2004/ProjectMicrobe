using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StabilityLightController : MonoBehaviour
{
    [SerializeField] private Renderer _objRenderer;

    // Call this method to update the color based on stability
    public void UpdateStability(bool isStable)
    {
        if (_objRenderer != null)
        {
            // Change the material color to green if stable, red if not
            _objRenderer.material.color = isStable ? Color.green : Color.red;
        }
    }
}
