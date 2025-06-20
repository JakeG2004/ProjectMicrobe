using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NotificationPanelManager : MonoBehaviour
{
    public static NotificationPanelManager Instance {get; private set; }

    private Animator _anim;
    private bool _isAnim = false;
    [SerializeField] private TMP_Text _notificationText;

    // Start is called before the first frame update
    void Start()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }

        _anim = GetComponent<Animator>();
    }

    public void ShowPanel()
    {
        _isAnim = true;
        _anim.SetTrigger("ShowPanel");
    }

    public void ShowPanel(string panelText)
    {
        SetPanelText(panelText);
        ShowPanel();
    }

    public void HidePanel()
    {
        _anim.SetTrigger("HidePanel");
        _isAnim = false;
    }

    public void ShowPanelForSeconds(int seconds)
    {
        ShowPanel();
        StartCoroutine(HidePanelAfterSeconds(seconds));
    }

    public void ShowPanelForSeconds(string panelText, int seconds = 3)
    {
        SetPanelText(panelText);
        ShowPanel();
        StartCoroutine(HidePanelAfterSeconds(seconds));
    }

    IEnumerator HidePanelAfterSeconds(int seconds)
    {
        yield return new WaitForSeconds(seconds);
        HidePanel();
    }

    public void SetPanelText(string panelText)
    {
        _notificationText.text = panelText;
    }

    public void UpdatePanelText(string panelText)
    {
        _notificationText.text = panelText;
        StopAllCoroutines();
        StartCoroutine(HidePanelAfterSeconds(1));
    }

    public bool IsAnimating()
    {
        return _isAnim;
    }
}
