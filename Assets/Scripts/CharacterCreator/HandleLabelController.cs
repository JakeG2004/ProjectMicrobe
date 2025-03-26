using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HandleLabelController : MonoBehaviour
{
    private TMP_Text _handleLabel;

    // Start is called before the first frame update
    void Start()
    {
        _handleLabel = GetComponent<TMP_Text>();
    }

    public void UpdateHandleText(float newTxt)
    {
        _handleLabel.text = (newTxt + 1).ToString();
    }
}
