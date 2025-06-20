using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MicrobeEncyclopedia : MonoBehaviour
{
    [SerializeField] private List<MicrobeSO> _microbes = new();
    [SerializeField] private GameObject _panel;
    [SerializeField] private GameObject _microbeInfoPrefab;
    [SerializeField] private GameObject _microbeContainer;
    [SerializeField] private GameObject _microbeButtonSpawn;
    [SerializeField] private GameObject _microbeInfoContainer;
    [SerializeField] private TMP_Text _microbeTitle;
    [SerializeField] private TMP_Text _microbeBody;
    private bool _isActive = false;

    void Start()
    {
        UpdateEntries();
    }

    public void ToggleState()
    {
        _isActive = !_isActive;

        _panel.SetActive(_isActive);

        // Set UI control state
        GetComponent<ToggleCameraTracking>()?.SetCameraTracking(!_isActive);
        MovementController.instance.SetMovementState(!_isActive);
        GetComponent<ShowHideMouse>()?.SetState(_isActive);

        GetComponent<BoolGameEventTrigger>().TriggerEvent(_isActive);

        if(_isActive)
        {
            GetComponent<MenuSoundPlayer>().PlaySound(AudioType.MENU_OPEN);
        }

        else
        {
            GetComponent<MenuSoundPlayer>().PlaySound(AudioType.MENU_CLOSED);
        }
    }

    public void HideInfo()
    {
        _microbeContainer.SetActive(true);
        _microbeInfoContainer.SetActive(false);
    }

    public void ShowInfo(MicrobeSO microbe)
    {
        _microbeContainer.SetActive(false);
        _microbeInfoContainer.SetActive(true);

        _microbeTitle.text = microbe.microbeName;

        // Parse the microbe to fill in the description
        string microbeBodyText = microbe.description;

        // Handle required resources
        microbeBodyText += "\n\nRequired Resources:";
        if (microbe.requiredResources.Count == 0)
        {
            microbeBodyText += "\n  No Required Resources";
        }

        else
        {
            foreach (var res in microbe.requiredResources)
            {
                microbeBodyText += "\n  " + res.resourceName + ": " + res.amount.ToString();
            }   
        }

        // Handle produced resources
        microbeBodyText += "\n\nProduced Resources:";
        if (microbe.producedResources.Count == 0)
        {
            microbeBodyText += "\n  No Produced Resources";
        }

        else
        {
            foreach (var res in microbe.producedResources)
            {
                microbeBodyText += "\n  " + res.resourceName + ": " + res.amount.ToString();
            }   
        }

        // Handle toxins
        microbeBodyText += "\n\nToxins:";
        if (microbe.toxins.Count == 0)
        {
            microbeBodyText += "\n  No Toxins";
        }

        else
        {
            foreach (var toxin in microbe.toxins)
            {
                microbeBodyText += "\n  " + toxin.toxinName;
            }   
        }

        _microbeBody.text = microbeBodyText;
    }

    public void AddMicrobe(MicrobeSO newMicrobe)
    {
        // Handle microbe already existing
        if(_microbes.Contains(newMicrobe))
        {
            return;
        }

        // Insert the microbe
        _microbes.Add(newMicrobe);

        // Update microbe entries
        UpdateEntries();
    }

    public void UpdateEntries()
    {
        // Delete every child
        foreach(Transform child in _microbeButtonSpawn.transform)
        {
            Destroy(child.gameObject);
        }

        // Add every microbe to the UI
        foreach (MicrobeSO microbe in _microbes)
        {
            GameObject newMicrobeInfo = Instantiate(_microbeInfoPrefab, _microbeButtonSpawn.transform);
            newMicrobeInfo.GetComponent<MicrobeSOEventTrigger>().SetStoredSO(microbe);
        }
    }
}
