
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using SFB;
using UnityEngine.UI;
using TMPro;

public class SaveMenu : MonoBehaviour
{
    [SerializeField] private TMP_Text _savingTextbox;
    [SerializeField] private TMP_Text _deleteTextbox;
    [SerializeField] private GameObject _star;
    private bool _pressedDelete = false;

    public void ExportSave()
    {
        var extensions = new[]
        {
            new ExtensionFilter("Ari Save File", "ari"),
        };

        var path = StandaloneFileBrowser.SaveFilePanel("Save File", "", "save.ari", extensions);

        if (string.IsNullOrEmpty(path))
        {
            return;
        }

        SaveSystem.Instance.SaveTo(path);
    }

    public void ImportSave()
    {
        // Open file with filter
        var extensions = new[] {
            new ExtensionFilter("Ari Save Files", "ari"),
        };
        var paths = StandaloneFileBrowser.OpenFilePanel("Open File", "", extensions, false);

        if (paths == null || string.IsNullOrEmpty(paths[0]))
        {
            return;
        }

        SaveSystem.Instance.LoadFrom(paths[0]);
    }

    public void SaveGame()
    {
        SaveSystem.Instance.SaveState();
        StartCoroutine(SaveGameText(0.5f));
    }

    public void DeleteSave()
    {
        if (!_pressedDelete)
        {
            _deleteTextbox.text = "Press Again to Confirm";
            _pressedDelete = true;
            return;
        }

        _pressedDelete = false;
        _deleteTextbox.text = "Deleted Save!";
        SaveSystem.Instance.DeleteSave();
    }

    private IEnumerator SaveGameText(float delay)
    {
        if (_savingTextbox == null)
        {
            yield break;
        }

        _savingTextbox.text = "Saving";

        for (int i = 0; i < 3; i++)
        {
            _savingTextbox.text += ".";
            yield return new WaitForSeconds(delay);
        }

        _savingTextbox.text = "Game Saved!";
    }

    void OnEnable()
    {
        if (_savingTextbox == null)
        {
            return;
        }

        _savingTextbox.text = "Save Game";

        _pressedDelete = false;

        if (_savingTextbox == null)
        {
            _deleteTextbox.text = "Delete Save";
        }
    }

    public void CheckStarState()
    {
        _star.SetActive(SaveSystem.Instance.GameIsComplete());
    }
}
