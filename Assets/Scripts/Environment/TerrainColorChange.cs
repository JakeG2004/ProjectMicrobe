using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainColorChange : MonoBehaviour
{
    [SerializeField] private Terrain _terrain;
    [SerializeField] private Color _color;

    public void ChangeTerrainColor()
    {
        _terrain.materialTemplate.color = _color;
    }
}
