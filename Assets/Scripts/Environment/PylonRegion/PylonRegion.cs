using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PylonRegion : MonoBehaviour
{
    [SerializeField] private EnvironmentSO _envSO;
    [SerializeField] private GameObject _regionPylon;
    [SerializeField] private UnityEvent _getPylonEvent;
    [SerializeField] private GameObject _plantParent;
    [SerializeField] private GameObject _pylonPrefab;
    private float _updateTime = 15.0f;
    private float _oldHealth = 0.0f;
    private Dictionary<GameObject, Vector3> _plantScales = new();

    void Start()
    {
        foreach (Transform child in _plantParent.transform)
        {
            _plantScales.Add(child.gameObject, child.localScale);
            child.localScale = Vector3.zero;
        }
    }

    public EnvironmentSO GetEnvSO()
    {
        return _envSO;
    }

    public void SetEnvSO(EnvironmentSO newEnvSO)
    {
        _envSO = newEnvSO;
    }

    public void SetRegionPylon(GameObject pylon)
    {
        _regionPylon = pylon;
        _getPylonEvent?.Invoke();

    }

    public bool HasPylon()
    {
        return (_regionPylon != null);
    }

    public GameObject GetPylon()
    {
        return _regionPylon;
    }

    public GameObject GetPylonPrefab()
    {
        return _pylonPrefab;
    }

    public void SetEnvHealth(float envHealth)
    {
        StartCoroutine(LerpEnvHealth(envHealth));
        _oldHealth = envHealth;
    }

    public IEnumerator LerpEnvHealth(float envHealth)
    {
        TerrainBlender tb = GetComponent<TerrainBlender>();
        float elapsed = 0.0f;
        float start = tb.GetBlendFactor();
        float totalChange = envHealth - start;

        while (elapsed < _updateTime)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / _updateTime);

            // Linear interpolation
            float blendVal = start + totalChange * t;

            tb.SetBlendFactor(blendVal);
            tb.SetDetailDensity(blendVal);
            UpdatePlantSize(blendVal);

            yield return null;
        }

        // Snap to final value to ensure precision
        tb.SetBlendFactor(envHealth);
        tb.SetDetailDensity(envHealth);
    }

    public void SetUpdateTime(float newTime)
    {
        _updateTime = newTime;
    }

    public void UpdatePlantSize(float size)
    {
        foreach (var kvp in _plantScales)
        {
            kvp.Key.transform.localScale = new Vector3(size * kvp.Value.x, size * kvp.Value.y, size * kvp.Value.z);
        }
    }
}
