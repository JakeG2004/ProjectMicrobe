using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectivePanel : MonoBehaviour
{
    [SerializeField] private ObjectivesDisplay _objectiveDisplayPrefab;

    [SerializeField] private Transform _objectiveDisplayParent;

    private readonly List<ObjectivesDisplay> _listDisplay = new List<ObjectivesDisplay>();

    void Start()
    {
        
    }
}
