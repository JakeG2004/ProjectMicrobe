using UnityEngine;
using UnityEngine.UI;


/***************************************
 This goes on the Image Object.
 It will set the image layers and colors
 as well as modifying materials and
 enabling/disabling meshes on the
 character when the button is pressed
 to reflect selection.

 I'm planning to use inheritance so that
 everything can be accesed the same way
 through this but the scripts don't get
 as long.

 Maybe better to use the event system?
 using listeners would be much more
 flexible, so maybe I'll try to do that
 soon. This is first pass.
***************************************/

public class CCOptions : MonoBehaviour {

	[SerializeField]
	[TextArea(5, 10)]
	string description;  // for our own reference. Visible in inspector so that we can add descriptions to prefabs


	[Space(10)]
	public OptionType selectedOption;
	public enum OptionType {
		Hair,
		Eyes,
		Skin,
		Accessory,
		UpperBody,
		LowerBody,
	}



	Color[] colors;
	Image[] uiImages;




}
