using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DifficultyManager : MonoBehaviour
{
    public static DifficultyManager Instance;
    [SerializeField] private TMP_Text _difficultyText;
    private string _curDiffText = "";

    void Awake()
    {
        if (Instance != this && Instance != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    void OnEnable()
    {
        SetDifficultyText();
    }

    public void IncreaseDifficulty()
    {
        ChangeDifficulty(-1);
    }

    public void DecreaseDifficulty()
    {
        ChangeDifficulty(1);
    }

    private void ChangeDifficulty(int dir)
    {
        MicrobeDampingChanger.Instance.JogDamping(dir);
        SetDifficultyText();
    }

    private void LoadDifficulty()
    {
        SetDifficultyText();
    }

    private void SetDifficultyText()
    {
        _curDiffText = MicrobeDampingChanger.Instance.GetDampingName();
        _difficultyText.text = "Difficulty: " + _curDiffText;
    }
}
