using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MicrobeDampingChanger : MonoBehaviour
{
    public static MicrobeDampingChanger Instance;

    [SerializeField] private float _damping = 0.25f;
    private int _curIdx = 2;
    private float[] _dampingVals = { 0f, .25f, .5f, .75f, 1f };
    private string[] _dampingNames = { "X-Hard", "Hard", "Medium", "Easy", "X-Easy" };

    void Awake()
    {
        if (Instance != this && Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

        SetDamping();
    }

    void Start()
    {
        _curIdx = SaveSystem.Instance.LoadDifficultyIdx();
    }

    public float GetDamping()
    {
        return _damping;
    }

    public string GetDampingName()
    {
        return _dampingNames[_curIdx];
    }

    public void SetDamping()
    {
        _damping = _dampingVals[_curIdx];
    }

    public void JogDampingUp()
    {
        JogDamping(1);
    }

    public void JogDampingDown()
    {
        JogDamping(-1);
    }
    
    public void JogDamping(int direction)
    {
        _curIdx += direction;

        if (_curIdx < 0)
        {
            _curIdx = 4;
        }

        if (_curIdx > 4)
        {
            _curIdx = 0;
        }

        SetDamping();
        SaveSystem.Instance.SaveDifficultyIdx(_curIdx);
    }
}
