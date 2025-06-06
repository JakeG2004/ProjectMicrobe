using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainColorChange : MonoBehaviour
{
    [SerializeField] private Terrain _terrain;
    [SerializeField] private Color _color;

    void Start()
    {
        ChangeTerrainColor();
    }

    public void ChangeTerrainColor()
    {
        _terrain.terrainData.terrainLayers[0].diffuseRemapMax = _color;
    }
}
