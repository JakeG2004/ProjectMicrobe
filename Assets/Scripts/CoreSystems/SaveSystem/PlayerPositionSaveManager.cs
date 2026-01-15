using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPositionSaveManager
{
    private SaveObject _currentState;

    public PlayerPositionSaveManager(SaveObject state)
    {
        _currentState = state;
    }

    public void UpdateState(SaveObject state)
    {
        _currentState = state;
    }

    public void SavePlayerPosition()
    {
        // Get the player
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        // Null check
        if (player == null)
        {
            Debug.Log("Failed to get player");
            return;
        }

        _currentState.playerData.position = Vector3ToFloat3(player.transform.position);
        _currentState.playerData.rotation = Vector3ToFloat3(player.transform.eulerAngles);
    }

    public void LoadPlayerPostion()
    {
        // Get the player
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        // Null check
        if (player == null)
        {
            Debug.Log("Failed to find player");
            return;
        }

        Rigidbody rb = player.GetComponent<Rigidbody>();

        rb.position = Float3ToVector3(_currentState.playerData.position);
        rb.rotation = Quaternion.Euler(Float3ToVector3(_currentState.playerData.rotation));

        Debug.Log("Set player pos!");
    }

    // Helper functions to convert between float arrays and vectors
    private Vector3 Float3ToVector3(float[] arr)
    {
        return new Vector3(arr[0], arr[1], arr[2]);
    }

    private float[] Vector3ToFloat3(Vector3 vec)
    {
        float[] floats = { vec.x, vec.y, vec.z };
        return floats;
    }
}
