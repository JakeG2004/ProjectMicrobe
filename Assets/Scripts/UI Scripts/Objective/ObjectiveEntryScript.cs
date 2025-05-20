/*
* Script to manage an Objective Entry (i.e. set its fields)
*
* Author:   Jake Gendreau
* Date:     5/16/25
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ObjectiveEntryScript : MonoBehaviour
{
    [SerializeField] private TMP_Text _objText;
    [SerializeField] private TMP_Text _completedText;
    private Animator _anim;

    void Start()
    {
        _anim = GetComponent<Animator>();
        _objText.gameObject.SetActive(false);
        _completedText.transform.parent.gameObject.SetActive(false);
    }

    public void SetObjText(string objText)
    {
        _objText.text = objText;
    }

    // Initialize the entry with the objective
    public void InitEntry(Objective obj)
    {
        // Show the objects
        _objText.gameObject.SetActive(true);
        _completedText.transform.parent.gameObject.SetActive(true);

        SetObjText(obj.GetObjectiveText());
        obj.LinkToPopup(this);
    }

    public void CompleteObjective()
    {
        _completedText.text = "X";
    }

    public void Hide()
    {
        _anim.SetBool("HasEntry", false;)
    }

    public void Show()
    {
        _anim.SetBool("HasEntry, true");
    }

    public void SwitchObjective(float time, Objective obj)
    {
        Hide();
        StartCoroutine(SwitchObjectiveIEnum(time, obj));
    }

    public IEnumerator SwitchObjectiveIEnum(float time Objective obj)
    {
        yield return new WaitForSeconds(time);
        InitEntry(obj);
        Show();
    }
}
