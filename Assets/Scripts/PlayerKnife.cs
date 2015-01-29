using UnityEngine;
using System.Collections;

public class PlayerKnife : MonoBehaviour
{


	void OnTriggerStay (Collider col) 
	{
		if (col.tag == "Enemy" && Input.GetKey(KeyCode.E)) {
			Destroy (col.gameObject);
			Application.LoadLevel ("GameOver");
		}
	}

}