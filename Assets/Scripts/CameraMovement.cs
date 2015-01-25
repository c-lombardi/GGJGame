using UnityEngine;
public class CameraMovement : MonoBehaviour {
	public Transform target;
	void Start () {

	}


	void Update(){
		if (target != null) {
			Vector3 v = target.position;
			v.y = transform.position.y;
			transform.position = v;
		}
	}
 
 }
 