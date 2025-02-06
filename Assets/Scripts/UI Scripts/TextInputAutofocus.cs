using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextInputAutofocus : MonoBehaviour
{
    private TMP_InputField inputField;
    private bool activateNextFrame = false;

    private void Awake()
    {
        inputField = GetComponent<TMP_InputField>();
    }

    private void Update()
    {
        if (activateNextFrame)
        {
            inputField.ActivateInputField();
            activateNextFrame = false;
        }
    }

    private void OnEnable()
    {
        // i could not tell you why this is necessary, but it is
        activateNextFrame = true;
    }

    private void OnDisable()
    {
        inputField.DeactivateInputField();
    }
}