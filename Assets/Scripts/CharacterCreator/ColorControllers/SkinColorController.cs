using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkinColorController : BaseToggleGroupController
{
    private GameObject[] _skinObjs;

    protected override void Start()
    {
        base.Start();
        _skinObjs = CosmeticContainer.Instance?.GetSkinObjects();
    }

    protected override void OnTurnOn()
    {
        AssignColorToMaterial();
    }

    public void AssignColorToMaterial()
    {
        if (!_tg || !_tg.isOn)
        {
            return;
        }

        foreach (GameObject so in _skinObjs)
        {
            Renderer renderer = so.GetComponent<Renderer>();
            foreach (Material mat in renderer.materials)
            {
                if (mat.name.Contains("m_Ari_Skin"))
                {
                    mat.SetColor("_TintR", _colorTuple.r);
                    mat.SetColor("_TintG", _colorTuple.g);
                    mat.SetColor("_TintB", Color.white);
                    mat.SetColor("_SSS", _colorTuple.b);
                }

                if (mat.name.Contains("m_Ari_Mustache"))
                {
                    mat.SetColor("_TintR", _colorTuple.r);
                }
            }
        }
    }
}
