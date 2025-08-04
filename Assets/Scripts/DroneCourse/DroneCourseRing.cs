using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneCourseRing : MonoBehaviour
{
    public System.Action OnPlayerPassthrough;
    [SerializeField] private bool _isActive = false;
    [SerializeField] private bool _isComplete = false;
    private Vector3 _scale;

    void Awake()
    {
        _scale = transform.parent.localScale;
    }

    // Gets collision with player, marks ring as complete
    private void OnTriggerEnter(Collider other)
    {
        if (!_isActive || _isComplete)
        {
            return;
        }

        if (other.CompareTag("Player"))
        {
            CompleteRing();
        }
    }

    public void ActivateRing()
    {
        _isActive = true;
        DirectionArrowScript.Instance?.ChangeTarget(transform);
        SetRingColor(Color.cyan);
    }

    public void ResetRing()
    {
        _isActive = false;
        _isComplete = false;
        SetRingColorImmediate(Color.red);
    }

    public void ShowRing()
    {
        transform.parent.gameObject.SetActive(true);
        StartCoroutine(SetRingScale(1));
    }

    public void HideRing()
    {
        StartCoroutine(SetRingScale(0));
    }

    private void CompleteRing()
    {
        _isActive = false;
        _isComplete = true;

        OnPlayerPassthrough?.Invoke();

        SetRingColor(Color.green);
    }

    // Sets the inner ring color of the parent
    private void SetRingColor(Color color)
    {
        Renderer parentRenderer = transform.parent.GetComponent<Renderer>();
        Material[] mats = parentRenderer.materials;
        Material lightMat;

        foreach (Material mat in mats)
        {
            if (mat.name.Contains("Light_White"))
            {
                lightMat = mat;
                StartCoroutine(LerpColor(lightMat, color));
                return;
            }
        }

        _isComplete = true;
    }

    private void SetRingColorImmediate(Color color)
    {
        Renderer parentRenderer = transform.parent.GetComponent<Renderer>();
        Material[] mats = parentRenderer.materials;
        Material lightMat;

        foreach (Material mat in mats)
        {
            if (mat.name.Contains("Light_White"))
            {
                lightMat = mat;
                mat.SetColor("_EmissionColor", color);
                return;
            }
        }
    }

    // Smoothly transitions from current color to target color
    private IEnumerator LerpColor(Material mat, Color color)
    {
        Color initColor = mat.GetColor("_EmissionColor");
        float elapsedTime = 0.0f;
        float totalTime = 0.25f;

        while (elapsedTime <= totalTime)
        {
            elapsedTime += Time.deltaTime;
            float ratio = elapsedTime / totalTime;

            mat.SetColor("_EmissionColor", Color.Lerp(initColor, color, ratio));
            yield return null;
        }

        initColor = color;
    }

    // Handles ring scale and active states
    private IEnumerator SetRingScale(float scale)
    {
        // Set initial scale
        Vector3 initScale = _scale;
        Vector3 targetScale = initScale * scale;

        // Scale of these things is non-uniform, we we get the scale then shrink them
        if (scale == 1)
        {
            transform.parent.localScale = Vector3.zero;
            initScale = Vector3.zero;
        }

        // Add delay so it doesnt immediately start to close
        if (scale == 0)
        {
            yield return new WaitForSeconds(1f);
        }

        // Change the scale over 0.5 seconds
        float elapsedTime = 0.0f;
        float totalTime = 0.5f;

        while (elapsedTime < totalTime)
        {
            elapsedTime += Time.deltaTime;
            float ratio = elapsedTime / totalTime;

            transform.parent.localScale = Vector3.Lerp(initScale, targetScale, ratio);
            yield return null;
        }

        // Snap to final scale
        transform.parent.localScale = targetScale;

        // Set active accordingly
        if (scale == 0)
        {
            transform.parent.gameObject.SetActive(false);
        }
    }

    public bool IsComplete()
    {
        return _isComplete;
    }
}
