using UnityEngine;

public class RuntimeSkinnedMeshUpdater : MonoBehaviour
{
    public SkinnedMeshRenderer targetSkin;
    public Transform rootBone;
    public bool includeInactive = false;

    [ContextMenu("Update Skinned Mesh Bones")]
    public void UpdateSkinnedMesh()
    {
        if (targetSkin == null || rootBone == null)
        {
            Debug.LogWarning("Assign both a target SkinnedMeshRenderer and a root bone.");
            return;
        }

        string rootName = targetSkin.rootBone != null ? targetSkin.rootBone.name : "";
        Transform newRoot = null;

        Transform[] newBones = new Transform[targetSkin.bones.Length];
        Transform[] existingBones = rootBone.GetComponentsInChildren<Transform>(includeInactive);

        int missingBones = 0;
        for (int i = 0; i < targetSkin.bones.Length; i++)
        {
            if (targetSkin.bones[i] == null)
            {
                Debug.LogWarning("Bone at index " + i + " is null. Don't delete original bones before processing.");
                missingBones++;
                continue;
            }

            string boneName = targetSkin.bones[i].name;
            bool found = false;
            foreach (var newBone in existingBones)
            {
                if (newBone.name == rootName)
                    newRoot = newBone;

                if (newBone.name == boneName)
                {
                    newBones[i] = newBone;
                    found = true;
                    Debug.Log("✔ Found bone: " + boneName);
                    break;
                }
            }

            if (!found)
            {
                Debug.LogWarning("✘ Missing bone: " + boneName);
                missingBones++;
            }
        }

        targetSkin.bones = newBones;

        if (newRoot != null)
        {
            Debug.Log("✔ Setting new root bone: " + rootName);
            targetSkin.rootBone = newRoot;
        }

        Debug.Log("✅ Done! Missing bones: " + missingBones);
    }
}
