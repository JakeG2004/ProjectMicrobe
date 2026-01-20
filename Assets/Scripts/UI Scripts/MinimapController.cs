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

    // Affine transform coefficients (from calibration)
    private const float A = -2.2330f;
    private const float B =  0.01936f;
    private const float C =  1.385f;

    private const float D = -0.02380f;
    private const float E = -2.2503f;
    private const float F = -680.72f;

    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").transform;    
    }

    void Update()
    {
        _mapImage.anchoredPosition = WorldToMap(_player);
        _scientistSprite.anchoredPosition = WorldToMap(_scientist);
        _playerIndicator.eulerAngles = new Vector3(0, 0, -_player.eulerAngles.y + 45);
    }

    private Vector2 WorldToMap(Transform target)
    {
        Vector3 pos = target.position;

        float mapX = A * pos.x + B * pos.z + C;
        float mapY = D * pos.x + E * pos.z + F;

        return new Vector2(mapX, mapY);
    }
}
