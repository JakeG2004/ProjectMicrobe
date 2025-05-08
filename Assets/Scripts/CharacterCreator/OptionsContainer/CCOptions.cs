using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

/***************************************
Plan for this was that it would go on the Toggle object and have all the information
for ever possible option including what to change in the UI Images and the materials
on the player. 
Then I would have a fancy Editor script controlled by the enum that makes it so that
only the options needed for your selction are given to fill out.
Colors would be applied and Objects would be enabled/ dasabled based on the enum too,
when the toggle is selected.
***************************************/

public class CCOptions : MonoBehaviour {

	[SerializeField]
	[TextArea(2, 6)]
	string description;  // for our own reference. Visible in inspector so that we can add descriptions to prefabs

	[Space(10)]
	[SerializeField] Image lockImage; // set in start depending on toggle object interactability

	[Space(10)]
	public OptionType selectedOption;
	public enum OptionType {
		HairA,
		HairB,
		Eyes,
		Skin,
		Accessory,
		UpperBody,
		LowerBody,
	}

	//UI Images
	[SerializeField] Image imageBackground; // base toggle image. always on

	// MATERIALS
	[SerializeField] Material[] allHairMaterials; // no need to separate because all changes should be done on all
	[SerializeField] Material skin;
	[SerializeField] Material eyeBase;
	[SerializeField] Material eyeLense;
	[SerializeField] Material glassesFrame;
	[SerializeField] Material glassesLense;
	[SerializeField] Material GogglesFrame;
	[SerializeField] Material GogglesLense;
	[SerializeField] Material shirt;
	[SerializeField] Material hoodie;
	[SerializeField] Material labCoat;
	[SerializeField] Material pants;
	[SerializeField] Material shorts;
	[SerializeField] Material shoes;

	// COLORS
	[SerializeField] Color hairTintR; // hair Primary
	[SerializeField] Color hairTintG; // hair Secondary
	[SerializeField] Color hairTintB; // hair Accessory
	[SerializeField] Color hairHighlightColor; // hair highlight color



	#region HairA
	// Hair primary color. (_TintR)
	// Hair specualar color is tied to this option. (_HighlightColor)
	// Changing this option should change both these values in all hair materials.
	// imageBackground should be set to Hair primary color.
	#endregion

	#region HairB
	// Hair secondary color. (_TintG)
	// hange secondary hair color in all hair materials.
	// imageBackground should be set to Hair secondary color.
	#endregion




	/*********** !!!!!!!!!!!! un-comment to use! *********************************************
	void Start() {
		lockImage.enabled = !GetComponent<Toggle>().interactable;
	}
	******************************************************************************************/

}



// FANCY editor script to make this easier to use

#if UNITY_EDITOR

[CustomEditor(typeof(CCOptions))]
public class CCOptionsEditor : Editor {
	public override void OnInspectorGUI() {
		serializedObject.Update();

		SerializedProperty selectedOption = serializedObject.FindProperty("selectedOption");
		EditorGUILayout.PropertyField(selectedOption);

		CCOptions.OptionType option = (CCOptions.OptionType)selectedOption.enumValueIndex;

		switch(option) {
			case CCOptions.OptionType.HairA:
				EditorGUILayout.PropertyField(serializedObject.FindProperty("hairATexture"));
				break;
			case CCOptions.OptionType.HairB:
				EditorGUILayout.PropertyField(serializedObject.FindProperty("hairBTexture"));
				break;
			case CCOptions.OptionType.Eyes:
				EditorGUILayout.PropertyField(serializedObject.FindProperty("eyeColor"));
				break;
			case CCOptions.OptionType.Skin:
				EditorGUILayout.PropertyField(serializedObject.FindProperty("skinColor"));
				break;
			case CCOptions.OptionType.Accessory:
				EditorGUILayout.PropertyField(serializedObject.FindProperty("accessoryMaterial"));
				break;
			case CCOptions.OptionType.UpperBody:
				EditorGUILayout.PropertyField(serializedObject.FindProperty("upperBodyMaterial"));
				break;
			case CCOptions.OptionType.LowerBody:
				EditorGUILayout.PropertyField(serializedObject.FindProperty("lowerBodyMaterial"));
				break;
		}

		serializedObject.ApplyModifiedProperties();
	}
}
#endif