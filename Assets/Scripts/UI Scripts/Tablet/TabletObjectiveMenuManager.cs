using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TabletObjectiveMenuManager : MonoBehaviour
{
    [Header("Spawning References")]
    [SerializeField] private GameObject _objectiveButtonPrefab;

    [Tooltip("The object which the buttons will be instanced as children of")]
    [SerializeField] private Transform _parentObj;

    [Space(10)]
    [Header("Text entry fields")]
    [SerializeField] private TMP_Text _titleText;
    [SerializeField] private TMP_Text _descriptionText;
    [SerializeField] private TMP_Text _objGroup;

    private List<Objective> _currentObjectives = new();

    void Start()
    {
        LevelLoader.Instance.OnSceneUnload += UnsubscribeFromEvents;
    }

    void OnEnable()
    {
        ShowFirstObjective();
    }

    // Adds an objective button to the UI, ignoring it if it already exists
    public void AddObjectiveToList(Objective obj)
    {
        // Prevent duplicat entries from being added
        if (_currentObjectives.Contains(obj))
        {
            return;
        }
        _currentObjectives.Add(obj);

        // Store as a local varaible for lambda usage
        Objective curObj = obj;

        // Get the references        
        GameObject newButtonObj = Instantiate(_objectiveButtonPrefab, _parentObj);
        Button newButton = newButtonObj.GetComponent<Button>();

        // Set the text
        newButton.transform.GetChild(0).GetComponent<TMP_Text>().text = obj.GetObjectiveText();

        // Get the onclick event
        newButton.onClick.AddListener(() => ShowObjective(curObj));
    }

    public void ShowFirstObjective()
    {
        int childCount = _parentObj.childCount;

        Transform lastChild = _parentObj.GetChild(childCount - 1);

        Button curButton = lastChild.GetComponent<Button>();
        curButton.onClick.Invoke();
        curButton.Select();
    }

    // Update the objective text with the relevant information
    private void ShowObjective(Objective obj)
    {
        _titleText.text = obj.GetObjectiveText();
        _objGroup.text = obj.GetObjectiveGroup().gameObject.name;
        _descriptionText.text = obj.GetDescriptionText();
    }

    // Unsubscribe on scene unload
    private void UnsubscribeFromEvents()
    {
        // Unsubscribe from each button
        foreach (Transform child in _parentObj)
        {
            child.GetComponent<Button>().onClick.RemoveAllListeners();
        }

        // Unsubscribe from onsceneunload
        LevelLoader.Instance.OnSceneUnload -= UnsubscribeFromEvents;
    }
}
