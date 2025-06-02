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
            // Get the material from the renderer
            Material mat = _objRenderer.material;

            // Change the material color to green if stable, red if not
            Color col = isStable ? Color.green : Color.red;

            mat.color = col;
            mat.SetColor("_EmissionColor", col);
        }
    }
}
