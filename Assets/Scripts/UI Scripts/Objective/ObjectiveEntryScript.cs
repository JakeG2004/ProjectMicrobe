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
    }

    public void SetObjText(string objText)
    {
        _objText.text = objText;
        _completedText.text = "";
    }

    // Initialize the entry with the objective
    public void InitEntry(Objective obj)
    {
        SetObjText(obj.GetObjectiveText());
        obj.LinkToPopup(this);
        Show();
    }

    public void CompleteObjective()
    {
        _completedText.text = "X";
    }

    public void Hide()
    {
        _anim.SetBool("HasEntry", false);
    }

    public void DelayHide(float time)
    {
        StartCoroutine(DelayHideIEnum(time));
    }

    public void Show()
    {
        _anim.SetBool("HasEntry", true);
    }

    // Called to start an objective switch
    public void SwitchObjective(float time, Objective obj)
    {
        StartCoroutine(SwitchObjectiveIEnum(time, obj));
    }

    private IEnumerator SwitchObjectiveIEnum(float time, Objective obj)
    {
        yield return new WaitForSeconds(time);
        Hide();
        yield return new WaitForSeconds(time);
        InitEntry(obj);
        Show();
    }

    private IEnumerator DelayHideIEnum(float time)
    {
        yield return new WaitForSeconds(time);
        Hide();
    }
}
