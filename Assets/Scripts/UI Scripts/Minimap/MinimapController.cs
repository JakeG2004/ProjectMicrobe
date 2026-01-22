using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinimapController : MinimapIconSOListener
{
    [SerializeField] private RectTransform _mapImage;
    [SerializeField] private RectTransform _playerIndicator;
    [SerializeField] private GameObject _minimapIconPrefab;
    [SerializeField] private List<MinimapIcon> _minimapIcons;
    [SerializeField] private float _maxMapDistance = 85f;

    private Transform _player;
    private float[] _mapConstants = { -2.2330f, 0.01936f, 1.385f, -0.02380f, -2.2503f, -680.72f };
    private float[] _iconConstants = { 2.2515697f, -0.01267084f, 14.025911f, 0.002689249f, 2.1984585f, 662.21553f };

    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        UpdateMap();
        UpdateIcons();
    }

    private void UpdateIcons()
    {
        foreach (MinimapIcon curIcon in _minimapIcons)
        {
            UpdateIconPos(curIcon);
        }
    }

    // Updates the position of the map based on the player position.
    // Also updates the rotation of the player indicator
    private void UpdateMap()
    {
        _mapImage.anchoredPosition = WorldToMap(_player.position, _mapConstants);

        // Account for the 45 degree rotation in the indicator sprite
        _playerIndicator.eulerAngles = new Vector3(0, 0, -_player.eulerAngles.y + 45);
    }

    // Updates the position of an icon based on its world object position
    private void UpdateIconPos(MinimapIcon icon)
    {
        Vector3 iconPos = icon.transform.position;
        Vector3 playerPos = _player.position;

        Vector3 dir = iconPos - playerPos;
        dir.y = 0f;

        float distance = dir.magnitude;

        // If the target is too far away, have it sit on the edge of the minimap.
        if (distance >= _maxMapDistance)
        {
            dir.Normalize();
            iconPos = playerPos + dir * _maxMapDistance;
        }

        icon.SetUIPosition(WorldToMap(iconPos, _iconConstants));
    }

    // Translates world object positions to map positions
    private Vector2 WorldToMap(Vector3 pos, float[] constants)
    {
        float mapX = constants[0] * pos.x + constants[1] * pos.z + constants[2];
        float mapY = constants[3] * pos.x + constants[4] * pos.z + constants[5];

        return new Vector2(mapX, mapY);
    }

    // Inherited from the MinimapIconSOListener. Triggers whenever a minimap icon is added
    public override void OnEventRaised(MinimapIcon icon)
    {
        if (!_minimapIcons.Contains(icon))
        {
            // Create a new object as a child of the map
            GameObject newIcon = Instantiate(_minimapIconPrefab, _mapImage);
            newIcon.GetComponent<Image>().sprite = icon.GetIconSprite();

            // Update the reference for the ui object in the icon
            icon.SetUIElement(newIcon.transform);

            // Add it to the list
            _minimapIcons.Add(icon);
        }
    }
    
    public void RemoveIconFromMinimap(MinimapIcon icon)
    {
        _minimapIcons.Remove(icon);
    }
}