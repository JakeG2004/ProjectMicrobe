using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TerrainBinder : EditorWindow
{
    public enum MaterialCopyType
    {
        OFF,
        SHARE,
        CLONE
    }
    public Terrain curTerrain;
    public Terrain leftTerrain;
    public Terrain rightTerrain;
    public Terrain topTerrain;
    public Terrain bottomTerrain;
    public bool alsoSetTerrainLayers = false;
    public MaterialCopyType alsoSetMaterial = MaterialCopyType.OFF;
    private static readonly Vector2Int size = new Vector2Int(500, 500);

    [MenuItem("Tools/Terrain/TerrainBinder")]
    public static void ShowWindow()
    {
        EditorWindow window = GetWindow<TerrainBinder>();
        window.minSize = size;
        window.maxSize = size;
    }

    private void OnEnable()
    {
        curTerrain = Selection.activeGameObject.GetComponent<Terrain>();
        GetNeighorTerrains();
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Current Terrain", EditorStyles.boldLabel);
        curTerrain = (Terrain)EditorGUILayout.ObjectField("Center", curTerrain, typeof(Terrain), true);


        if (GUILayout.Button("Refresh Neighbors from Center"))
        {
            if (curTerrain != null)
            {
                GetNeighorTerrains();
            }
        }

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Neighbor Terrains", EditorStyles.boldLabel);
        leftTerrain = (Terrain)EditorGUILayout.ObjectField("Left (Negative X)", leftTerrain, typeof(Terrain), true);
        rightTerrain = (Terrain)EditorGUILayout.ObjectField("Right (Positive X)", rightTerrain, typeof(Terrain), true);
        topTerrain = (Terrain)EditorGUILayout.ObjectField("Top (Positive Z)", topTerrain, typeof(Terrain), true);
        bottomTerrain = (Terrain)EditorGUILayout.ObjectField("Bottom (Negative Z)", bottomTerrain, typeof(Terrain), true);

        EditorGUILayout.Space();

        alsoSetTerrainLayers = EditorGUILayout.Toggle("Also Set Terrain Layers", alsoSetTerrainLayers);
        alsoSetMaterial = (MaterialCopyType)EditorGUILayout.EnumFlagsField("Also Set Terrain Material", alsoSetMaterial);

        // Sets neighbors and autoconnect accordingly
        if (GUILayout.Button("Bind Neighbors"))
        {
            if (curTerrain != null)
            {
                curTerrain.SetNeighbors(leftTerrain, topTerrain, rightTerrain, bottomTerrain);
                curTerrain.allowAutoConnect = true;
                Debug.Log("Center Terrain bound");

                Terrain[] neighbors = new Terrain[4] { leftTerrain, rightTerrain, topTerrain, bottomTerrain };
                string[] neighborNames = new string[4] { "Left", "Right", "Top", "Bottom" };
                foreach (Terrain neighbor in neighbors)
                {
                    // Skip null terrains
                    if (neighbor == null)
                    {
                        continue;
                    }

                    // Bind and autoconnect
                    neighbor.allowAutoConnect = true;
                    switch (System.Array.IndexOf(neighbors, neighbor))
                    {
                        case 0:
                            leftTerrain.SetNeighbors(leftTerrain.leftNeighbor, leftTerrain.topNeighbor, curTerrain, leftTerrain.bottomNeighbor);
                            break;

                        case 1:
                            rightTerrain.SetNeighbors(curTerrain, rightTerrain.topNeighbor, rightTerrain.rightNeighbor, rightTerrain.bottomNeighbor);
                            break;

                        case 2:
                            topTerrain.SetNeighbors(topTerrain.leftNeighbor, topTerrain.topNeighbor, topTerrain.rightNeighbor, curTerrain);
                            break;

                        case 3:
                            bottomTerrain.SetNeighbors(bottomTerrain.leftNeighbor, curTerrain, bottomTerrain.rightNeighbor, bottomTerrain.bottomNeighbor);
                            break;
                    }

                    // Handle Setting Terrain layers
                    if (alsoSetTerrainLayers)
                    {
                        neighbor.terrainData.terrainLayers = curTerrain.terrainData.terrainLayers;
                    }

                    // Handle setting material. Creates a copy in case one wants to be modified without the other
                    switch (alsoSetMaterial)
                    {
                        case MaterialCopyType.OFF:
                            break;

                        case MaterialCopyType.SHARE:
                            neighbor.materialTemplate = curTerrain.materialTemplate;
                            break;

                        case MaterialCopyType.CLONE:
                            neighbor.materialTemplate = new Material(curTerrain.materialTemplate);
                            break;
                    }

                    Debug.Log($"Terrain {neighborNames[System.Array.IndexOf(neighbors, neighbor)]} bound!");
                }

                Debug.Log("Neighbors set successfully.");
            }
            else
            {
                Debug.LogWarning("Current terrain is null.");
            }
        }
    }

    private void GetNeighorTerrains()
    {
        leftTerrain = curTerrain.leftNeighbor;
        rightTerrain = curTerrain.rightNeighbor;
        topTerrain = curTerrain.topNeighbor;
        bottomTerrain = curTerrain.bottomNeighbor;
    }
}
