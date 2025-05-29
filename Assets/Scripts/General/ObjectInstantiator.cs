// ObjectInstantiator.cs
// Instances an Object on the current GO, with optional childing
// Author:  Jake Gendreau
// Date:    5/23/25

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectInstantiator : MonoBehaviour
{
    [SerializeField] private GameObject _objectToInstantiate;
    [SerializeField] private bool _instantiateAsChild = false;

    public void InstanceObject()
    {
        if(_instantiateAsChild)
        {
            GameObject newObj = Object.Instantiate(_objectToInstantiate, transform.position, transform.rotation, transform);
            return;
        }

        Object.Instantiate(_objectToInstantiate, transform.position, transform.rotation);
    }
}
