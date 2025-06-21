using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clicker : MonoBehaviour
{
    private int _numBugsClicked = 0;
    private SoundPlayer _sp;

    void Start()
    {
        _sp = GetComponent<SoundPlayer>();
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        col.gameObject.GetComponent<RandomBugMovement>().ResetBug();
        _numBugsClicked++;
    }

    public void Click()
    {
        gameObject.SetActive(true);
        StopAllCoroutines();
        StartCoroutine(ITurnOffClicker());
    }

    private IEnumerator ITurnOffClicker()
    {
        yield return new WaitForSeconds(0.1f);
        GetComponent<IntGameEventTrigger>().TriggerEvent(_numBugsClicked);
        _numBugsClicked = 0;
        gameObject.SetActive(false);
    }
}
