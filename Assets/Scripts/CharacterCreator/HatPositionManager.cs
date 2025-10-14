using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HatPositionManager : MonoBehaviour
{
    [SerializeField] private Transform _hat;
    [SerializeField] private Transform _head;

    [SerializeField] private bool _singlePosition = false;

    private Transform _targetTransform;
    private Transform _originalTransform;

    private GameObject _flattenedHair;
    private GameObject _realHair;

    void Awake()
    {
        _originalTransform = _hat.parent;
    }

    void Start()
    {
        if (_hat == null)
        {
            _hat = transform.GetChild(0);
        }
    }

    // When a hat is enabled, set it correctly
    private void OnEnable()
    {
        UpdateHatPos();
    }

    // When a hat is reset, set its parent back to its original and turn off flattened hair
    public void ResetHat()
    {
        // Exit early if reset hat is called prior to initialization
        if (_targetTransform == null)
        {
            return;
        }

        _hat.parent = _originalTransform;
        _targetTransform.gameObject.SetActive(false);

        // Destroy the flattened hair if it exists
        if(_flattenedHair != null)
        {
            Destroy(_flattenedHair);
            _flattenedHair = null;
            SetHairActiveState(true);
        }
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

        // Destroy flattened hair so that only one flattened hair can exist at a time
        Destroy(_flattenedHair);
        _flattenedHair = null;

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

        if(_singlePosition)
        {
            _hat.parent = _head;
            return;
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

        // Handle the hair
        if(_targetTransform.childCount != 0)
        {
            // Turn off the real hair
            SetHairActiveState(false);

            // Turn on the flattened hair and copy materials (color) from the actual hair
            _flattenedHair = GameObject.Instantiate(_targetTransform.GetChild(0).gameObject, _hat);
            _flattenedHair.GetComponent<Renderer>().materials = _realHair.GetComponent<Renderer>().materials;
            _flattenedHair.SetActive(true);
        }

        // Set the new parent
        _hat.parent = _head;

        // Set the new offsets
        _hat.localPosition = _targetTransform.localPosition;
        _hat.localScale = _targetTransform.localScale;
        _hat.rotation = _targetTransform.rotation;
    }

    private void SetHairActiveState(bool state)
    {
        if(state)
        {
            _realHair.SetActive(true);
            _realHair = null;
            return;
        }

        foreach(Transform child in transform.parent)
        {
            // Skip any objects whose name doesn't start with "Hair" or aren't active
            if (!child.gameObject.activeSelf || child.gameObject.name.Substring(0, 4) != "Hair")
            {
                continue;
            }

            // Store the object as the real hair
            _realHair = child.gameObject;
            _realHair.SetActive(false);
            return;
        }
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
