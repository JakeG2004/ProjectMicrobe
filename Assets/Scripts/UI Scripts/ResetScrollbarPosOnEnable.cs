// ResetScrollbarPosOnEnable.cs
// A script which resets a scrollbarpos to a certain position on enable
// Author:  Jake Gendreau
// Date:    6/1/25

using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ResetScrollbarPosOnEnable : MonoBehaviour
{
    [SerializeField] private float _scrollBarPos = 1.0f;

    private void OnEnable()
    {
        StartCoroutine(ResetScrollbar());
    }

    private IEnumerator ResetScrollbar()
    {
        // Wait until the end of the frame to ensure UI is fully initialized
        yield return new WaitForEndOfFrame();

        Scrollbar scrollbar = GetComponent<Scrollbar>();
        if (scrollbar != null)
        {
            scrollbar.value = _scrollBarPos;
        }
    }
}

