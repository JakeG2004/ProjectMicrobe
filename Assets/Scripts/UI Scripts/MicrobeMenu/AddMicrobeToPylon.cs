using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AddMicrobeToPylon : MonoBehaviour
{
    [SerializeField] private TMP_InputField _numToAdd;
    [SerializeField] private MicrobeSO _microbeSO;
    private MicrobeMenu _microbeMenu;

    void Start()
    {
        _microbeMenu = GameObject.FindGameObjectWithTag("MicrobeMenu").GetComponent<MicrobeMenu>();
    }

    public void InsertMicrobes()
    {
        _microbeMenu.AddMicrobe(_microbeSO, float.Parse(_numToAdd.text));
    }
}
