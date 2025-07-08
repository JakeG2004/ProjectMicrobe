
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using SFB;

public class SaveMenu : MonoBehaviour
{
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
}
