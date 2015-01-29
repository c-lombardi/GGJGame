using UnityEngine;
using System.Collections;

public class GameExit : MonoBehaviour
{

	
	void OnTriggerEnter (Collider col) 
	{
		if (col.tag == "Player") {
			Application.LoadLevel ("GameOver");
		}
	}
}