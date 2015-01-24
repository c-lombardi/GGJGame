using UnityEngine;
using System.Collections;

public class ToggleLightScript : MonoBehaviour {
	public Light2D l2d;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update()
	{
		if (Input.GetKeyUp (KeyCode.Space)) {
			l2d.ToggleLight();
		}
	}
}
