using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InteractText : MonoBehaviour
{
    [SerializeField] private TMP_Text _interactText;
    private Animator _anim;

    void Start()
    {
        _anim = GetComponent<Animator>();
    }

    public void SetText(string text)
    {
        _interactText.text = text;
    }

    public void SetPos(Vector3 pos)
    {
        transform.position = pos;
    }

    public void ShowText(InteractableObject io)
    {
        StopAllCoroutines();
        SetPos(io.gameObject.transform.position);
        //SetText(io.GetInteractText());

        foreach (Transform child in _interactText.transform)
        {
            child.GetComponent<SpriteRenderer>().enabled = true;
        }

        _anim.SetBool("isUp", true);
    }

    public void HideText()
    {
        StartCoroutine(IHideText());
    }

    private IEnumerator IHideText()
    {
        _anim.SetBool("isUp", false);
        yield return new WaitForSeconds(0.1f);
        SetText("");

        foreach (Transform child in _interactText.transform)
        {
            child.GetComponent<SpriteRenderer>().enabled = false;
        }
    }
}
