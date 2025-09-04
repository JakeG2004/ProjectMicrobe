using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HatPositionManager : MonoBehaviour
{
    [SerializeField] private Transform _hat;
    [SerializeField] private Transform _head;
    private Transform _targetTransform;

    void Start()
    {
        if (_hat == null)
        {
            _hat = transform.GetChild(0);
        }
    }

    private void OnEnable()
    {
        UpdateHatPos();
    }

    private void Update()
    {
        // Position the hat at the head, applying the offset correctly
        _hat.position = _head.TransformPoint(_targetTransform.localPosition);
        _hat.rotation = _head.rotation * _targetTransform.localRotation;
    }

    public void UpdateHatPos()
    {
        if (gameObject.activeSelf == false || SaveSystem.Instance == null)
        {
            return;
        }

        // Figure out which hair is active
        HairStyle hair = (HairStyle)SaveSystem.Instance.GetHairIndex();

        string targetTransName = "";
        _targetTransform = _hat;

        // Set the position of the hat in accordance to which hair is active
        switch (hair)
        {
            // Get the name of the target transform
            case HairStyle.CURLS:
                targetTransName = "X_Curls";
                break;

            case HairStyle.DOWN:
                targetTransName = "X_Down";
                break;

            case HairStyle.SHAG:
                targetTransName = "X_Shag";
                break;

            case HairStyle.SPIKEY:
                targetTransName = "X_Spikey";
                break;

            case HairStyle.LONG_BRAID:
                targetTransName = "X_LongBraid";
                break;

            case HairStyle.PONYTAIL:
                targetTransName = "X_Ponytail";
                break;

            case HairStyle.FLOPPY:
                targetTransName = "X_Floppy";
                break;

            case HairStyle.POOF:
                targetTransName = "X_Poof";
                break;

            case HairStyle.LOOSE_BUN:
                targetTransName = "X_LooseBun";
                break;

            case HairStyle.BALD:
                targetTransName = "X_Bald";
                break;
        }

        // Get the transform based on the name
        foreach (Transform child in transform)
        {
            if (child.gameObject.name == targetTransName)
            {
                _targetTransform = child;
                break;
            }
        }

        _hat.localScale = _targetTransform.localScale;
    }

    // ORDER MATTERS PLEASE DONT CHANGE
    private enum HairStyle
    {
        CURLS,
        DOWN,
        SHAG,
        SPIKEY,
        LONG_BRAID,
        PONYTAIL,
        FLOPPY,
        POOF,
        LOOSE_BUN,
        BALD
    }
}
