// CharacterIconGrabber.cs
// A script for grabbing the character icon from the character camera
// Author:  Jake Gendreau
// Date:    6/10/25

using UnityEngine;
using System;
using System.Collections;

public class CharacterIconGrabber : MonoBehaviour
{
    [SerializeField] private Camera _ssCam;
    [SerializeField] private RenderTexture _renTex;
    private SpriteRenderer _sr;

    void Start()
    {
        _sr = GetComponent<SpriteRenderer>();
    }

    public void GetCharIcon()
    {
        StartCoroutine(CaptureScreenshot());
    }

    private IEnumerator CaptureScreenshot()
    {
        // Wait until end of fram so rendering is complete
        yield return new WaitForEndOfFrame();

        // Set the target texture
        _ssCam.targetTexture = _renTex;

        // Get a frame from the camera
        _ssCam.Render();

        // Set the active render texture
        RenderTexture.active = _renTex;

        // Create a new Texture2D with same dimensions
        Texture2D tex = new Texture2D(_renTex.width, _renTex.height, TextureFormat.RGB24, false);
        tex.ReadPixels(new Rect(0, 0, _renTex.width, _renTex.height), 0, 0);
        tex.Apply();

        // Cleanup
        _ssCam.targetTexture = null;
        RenderTexture.active = null;

        // Create sprite from texture
        Sprite newSprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
        _sr.sprite = newSprite;
    }
}
