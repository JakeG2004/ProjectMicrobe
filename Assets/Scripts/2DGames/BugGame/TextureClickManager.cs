using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureClickManager : MonoBehaviour
{
    [SerializeField] private Camera _2DGameCam;
    [SerializeField] private GameObject _CollisionCheck;
    
    private RectTransform _rt;

    void Start()
    {
        _rt = GetComponent<RectTransform>();
    }

    void Update()
    {
        // Get the uv of the mouse position on the render texture
        Vector2 uv = GetMousePosAsUV();
    }

    Vector3 CalculateCoordinate(Vector2 uv)
    {
        float worldX = Mathf.Lerp(_2DGameCam.ViewportToWorldPoint(new Vector3(0, 0, 0)).x, _2DGameCam.ViewportToWorldPoint(new Vector3(1, 0, 0)).x, uv.x);
        float worldY = Mathf.Lerp(_2DGameCam.ViewportToWorldPoint(new Vector3(0, 0, 0)).y, _2DGameCam.ViewportToWorldPoint(new Vector3(0, 1, 0)).y, uv.y);
    
        return new Vector3(worldX, worldY, -800);
    }

    Vector2 GetMousePosAsUV()
    {
        if (Input.GetMouseButton(0))
        {
            Vector2 localPoint;

            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(_rt, Input.mousePosition, null, out localPoint))
            {
                Vector2 size = _rt.rect.size;
                Vector2 uv = (localPoint + size * 0.5f) / size;

                // Calculate world position
                Vector3 worldSpaceCoordinate = CalculateCoordinate(uv);

                // Convert to local space of parent
                Transform parent = _CollisionCheck.transform.parent;
                Vector3 localPosition = parent.InverseTransformPoint(worldSpaceCoordinate);

                // Set local Y to -2.5
                localPosition.x = Mathf.Clamp(localPosition.x, -3.6f, 3.6f);
                localPosition.y = -2.5f;

                // Apply local position
                _CollisionCheck.transform.localPosition = localPosition;

                return uv;
            }
        }

        return Vector2.zero;
    }
}
