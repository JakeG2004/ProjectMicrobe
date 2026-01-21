using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinimapController : MonoBehaviour
{
    [SerializeField] private RectTransform _mapImage;
    [SerializeField] private RectTransform _playerIndicator;
    [SerializeField] private RectTransform _scientistSprite;
    //[SerializeField] private List<MinimapIcon> _minimapIcons;

    private Transform _player;
    [SerializeField] private Transform _scientist;
    private float[] _mapConstants = { -2.2330f, 0.01936f, 1.385f, -0.02380f, -2.2503f, -680.72f };
    private float[] _iconConstants = { 2.2515697f, -0.01267084f, 14.025911f, 0.002689249f, 2.1984585f, 662.21553f };

    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").transform;    
    }

    void Update()
    {
        Debug.Log("Scientist pos: " + _scientist.position + "\nPlayer pos: " + _player.position);
        _mapImage.anchoredPosition = GetMapPos();
        _scientistSprite.anchoredPosition = GetIconPos(_scientist);
        _playerIndicator.eulerAngles = new Vector3(0, 0, -_player.eulerAngles.y + 45);
    }

    private Vector2 GetMapPos()
    {
        return WorldToMap(_player, _mapConstants);
    }

    private Vector2 GetIconPos(Transform target)
    {
        return WorldToMap(target, _iconConstants);
    }

    private Vector2 WorldToMap(Transform target, float[] constants)
    {
        Vector3 pos = target.position;

        float mapX = constants[0] * pos.x + constants[1] * pos.z + constants[2];
        float mapY = constants[3] * pos.x + constants[4] * pos.z + constants[5];

        return new Vector2(mapX, mapY);
    }
}
