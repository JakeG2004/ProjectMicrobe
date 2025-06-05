using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LegendEntryScript : MonoBehaviour
{
    [SerializeField] private Image _colImage;
    [SerializeField] private TMP_Text _legendName;

    public void SetColor(Color col)
    {
        _colImage.color = col;
    }

    public void SetText(string text)
    {
        _legendName.text = text;
    }
}
