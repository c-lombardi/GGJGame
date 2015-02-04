using UnityEngine;
using System.Collections;

public class TitleScreen : MonoBehaviour {

	// Update is called once per frame
	void Update () {
	
		if(Input.GetKeyUp(KeyCode.Return)){
			Application.LoadLevel ("GameOver");
		}

	}
}
