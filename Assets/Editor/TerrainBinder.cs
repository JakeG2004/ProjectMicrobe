using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainBinder : EditorWindow
{
    public Terrain curTerrain;
    public Terrain leftTerrain;
    public Terrain rightTerrain;
    public Terrain topTerrain;
    public Terrain bottomTerrain;

    private static readonly Vector2Int size = new Vector2Int(500, 500);

    [MenuItem("Tools/Terrain/TerrainBinder")]
    public static void ShowWindow()
    {
        EditorWindow window = GetWindow<TerrainBinder>();
        window.minSize = size;
        window.maxSize = size;
    }

    
    
}
