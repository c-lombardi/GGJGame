using UnityEngine;
using System.Collections;

public class EnemySerializer : MonoBehaviour {
	public Transform player;
	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info) {
		Vector3 position = player != null ? player.position : Vector3.zero;
		Quaternion rotation = player != null ? player.rotation : Quaternion.identity;
		bool flashlightOn = player != null ? player.GetComponent<CharacterComponentProvider>().flashlight.LightEnabled : false;

		stream.Serialize(ref position);
		stream.Serialize(ref rotation);
		stream.Serialize(ref flashlightOn);

		transform.position = position;
		transform.rotation = rotation;
		transform.GetComponent<CharacterComponentProvider>().flashlight.LightEnabled = flashlightOn;
	}
}
