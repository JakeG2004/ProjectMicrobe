using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ObjectivesDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _objectiveText;

    private Objective _objective;

    // Initialize a new objective display
    public void Init(Objective objective)
    {
        _objective = objective;
        _objectiveText.text = objective.GetStatusText();
        objective.OnValueChange += OnObjectiveValueChange;
        objective.OnComplete += OnObjectiveComplete;
    }

    // Set text upon objective completion
    private void OnObjectiveComplete()
    {
        _objectiveText.text = $"<s>{_objective.GetStatusText()}<s>";
    }

    // Set text upon objective value changed
    private void OnObjectiveValueChange()
    {
        _objectiveText.text = _objective.GetStatusText();
    }
}
