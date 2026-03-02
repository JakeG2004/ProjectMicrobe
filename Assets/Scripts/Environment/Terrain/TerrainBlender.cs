using UnityEngine;

public class TerrainBlender : MonoBehaviour
{
    [SerializeField] private Terrain _terrain;
    // 0 for brown, 1 for green
    [SerializeField] private float _blendFactor = 0.0f;
    [SerializeField] private float _pinkBlendFactor = 1f;
    [SerializeField] private float _detailDensity = 0.1f;
    private float _initialDetailDensity = 0.1f;
    private const string _blendFactorPropertyName = "_CustomBlendFactor";
    private const string _pinkBlendFactorPropertyName = "_PinkBlendFactor";
    private Material _terrainMat;

    void Start()
    {
        // Warn if no terrain
        if (_terrain == null)
        {
            // Debug.LogWarning("No terrain!");
            return;
        }

        _terrainMat = _terrain.materialTemplate;
        if (_terrainMat == null)
        {
            // Debug.LogError("No terrin material found!");
            return;
        }

        _initialDetailDensity = _detailDensity;
        UpdateTerrainProperties();
        SetDetailDensity(_detailDensity);
    }

    void Update()
    {
        if (_terrain == null)
        {
            return;
        }

        bool blendFactorChanged = _terrainMat.GetFloat(_blendFactorPropertyName) != _blendFactor;
        bool pinkBlendFactorChanged = _terrainMat.GetFloat(_pinkBlendFactorPropertyName) != _pinkBlendFactor;
        bool detailDensityChanged = _terrain.detailObjectDensity != _detailDensity;

        if (blendFactorChanged || detailDensityChanged || pinkBlendFactorChanged)
        {
            UpdateTerrainProperties();
        }
    }

    public void UpdateTerrainProperties()
    {
        if (_terrain == null)
        {
            return;
        }

        if (_terrainMat != null)
        {
            _terrainMat.SetFloat(_blendFactorPropertyName, 1 - _blendFactor);
            _terrainMat.SetFloat(_pinkBlendFactorPropertyName, 1 - _pinkBlendFactor);
        }

        if (_terrain != null)
        {
            _terrain.detailObjectDensity = _detailDensity;
        }
    }

    public void SetBlendFactor(float val)
    {
        _blendFactor = Mathf.Clamp01(val);
        UpdateTerrainProperties();
    }

    public void SetPinkBlendFactor(float val)
    {
        _pinkBlendFactor = Mathf.Clamp01(val);
        UpdateTerrainProperties();
    }

    public void SetDetailDensity(float val)
    {
        // Normalize to [initialVal, .65]
        _detailDensity = _initialDetailDensity + (((val - _initialDetailDensity) * (.65f - _initialDetailDensity)) / (.65f - _initialDetailDensity));
        UpdateTerrainProperties();
    }

    public float GetBlendFactor()
    {
        return _blendFactor;
    }
} 