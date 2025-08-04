using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FacePlayer : MonoBehaviour
{
    private GameObject _cam;

    // Start is called before the first frame update
    void Start()
    {
        FindCam();
        LevelLoader.Instance.OnSceneLoad += FindCam;
        LevelLoader.Instance.OnSceneUnload += RemoveCam;
    }

    void FindCam()
    {
        _cam = GameObject.FindGameObjectWithTag("MainCamera");
    }

    void RemoveCam()
    {
        _cam = null;
    }

    // Update is called once per frame
    void Update()
    {
        if(_cam == null)
        {
            return;
        }
        
        transform.LookAt(_cam.transform);
    }
}
