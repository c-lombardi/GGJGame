using UnityEngine;
using System.Collections;

public class NavToPlayer : MonoBehaviour {
	public Transform target;
	public Light2D targetLight;
	public bool inPursuit = false;
	NavMeshAgent nma;
	void Start()
	{
		nma = GetComponent<NavMeshAgent>();
		Light2D.RegisterEventListener (LightEventListenerType.OnEnter, OnLightEnter);
		Light2D.RegisterEventListener (LightEventListenerType.OnExit, OnLightExit);
		Light2D.RegisterEventListener (LightEventListenerType.OnStay, OnLightStay);
	}

	void OnDestroy()
	{
		Light2D.UnregisterEventListener (LightEventListenerType.OnEnter, OnLightEnter);
		Light2D.UnregisterEventListener (LightEventListenerType.OnExit, OnLightExit);
		Light2D.UnregisterEventListener (LightEventListenerType.OnStay, OnLightStay);


	}
	void OnLightEnter(Light2D _lightObject, GameObject _gameObject)
	{
		if ((_gameObject.name == target.name))
		{
			inPursuit = true;
			transform.LookAt(target.position);
			nma.SetDestination (target.position);
		}
	}

	void OnLightStay (Light2D _lightObject, GameObject _gameObject)
	{
		if ((_gameObject.name == target.name))
		{
			inPursuit = true;
			nma.SetDestination (target.position);
		}
	}

	void OnLightExit (Light2D _lightObject, GameObject _gameObject)
	{
		StartCoroutine(Pause (0));
	}
	
	// Update is called once per frame
	void Update () {
		Debug.Log (inPursuit.ToString ());
		if(!inPursuit){
			StartCoroutine(Pause (1));
		}
	}

	//0 switches pursuit off
	//1 sets destination 
	private IEnumerator Pause(int flag)
	{
		switch (flag) {
		case 0:
			yield return new WaitForSeconds(10.0f);
			if (inPursuit)
			{
				inPursuit = false;
			}
			break;
		case 1:
			yield return new WaitForSeconds(10.0f);
			nma.SetDestination (Random.insideUnitSphere * Random.Range(10,100));
			break;
		default:
			break;
		}
	}
}
