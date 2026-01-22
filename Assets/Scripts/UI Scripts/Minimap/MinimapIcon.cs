using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinimapIcon : MinimapIconSOTrigger
{
    [SerializeField] private Sprite _iconImage;
    [SerializeField] private bool _addOnStart = false;
    private Transform _uiObject;

    void Start()
    {
        if(_addOnStart)
        {
            AddIconToMinimap();
        }
    }

    public void SetUIPosition(Vector2 pos)
    {
        _uiObject.localPosition = new Vector3(pos.x, pos.y, 0f);
    }

    public void SetUIElement(Transform element)
    {
        _uiObject = element;
    }

    public Sprite GetIconSprite()
    {
        return _iconImage;
    }

    public void AddIconToMinimap()
    {
        AddIcon(this);
    }
    
    public void RemoveIconFromMinimap()
    {
        RemoveIcon(this);
    }
}
