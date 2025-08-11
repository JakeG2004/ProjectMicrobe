// MainMenuScript.cs
// A script for managing the main menu
// Author:  Jake Gendreau
// Date:    7/8/25

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuScript : MonoBehaviour
{
    [Header("Microbe Information")]
    [SerializeField] private Image _microbeIcon;
    [SerializeField] private Image _microbeFace;
    [SerializeField] private List<Sprite> _microbeBodies = new();
    [SerializeField] private List<Sprite> _microbeFaces = new();

    [Space(10)]
    [Header("Button Info")]
    [SerializeField] private GameObject _completeStar;

    void Start()
    {
        NewInputController.Instance.SetMenuMode();

        // Make a random microbe
        _microbeIcon.sprite = _microbeBodies[Random.Range(0, _microbeBodies.Count)];
        Color[] colors = { Color.red, Color.blue, Color.green, Color.yellow, Color.cyan, Color.magenta };
        _microbeIcon.color = colors[Random.Range(0, colors.Length)];
        _microbeFace.sprite = _microbeFaces[Random.Range(0, _microbeFaces.Count)];

        // Set gold star to indicate whether game is complete or not
        CheckStar();
    }

    public void CheckStar()
    {
        StartCoroutine(WaitForGameCompleteStats());
    }

    private IEnumerator WaitForGameCompleteStats()
    {
        yield return null;
        _completeStar.SetActive(SaveSystem.Instance.GameIsComplete());
    }

    public void Play()
    {
        // Directly load the level if a save exists, otherwise go to character creator
        if (!SaveSystem.Instance.IsDefaultSave())
        {
            LevelLoader.Instance.LoadLevel("TutorialScene");
        }

        else
        {
            LevelLoader.Instance.LoadLevel("CharacterCreator");
        }
    }

    public void CharacterCreator()
    {
        LevelLoader.Instance.LoadLevelNoSave("CharacterCreator");
    }

    public void QuitGame()
    {
        LevelLoader.Instance.QuitGame();
    }
}
