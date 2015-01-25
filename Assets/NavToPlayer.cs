using UnityEngine;
using System.Collections;

public class NavToPlayer : MonoBehaviour {
	public Transform target;
	NavMeshAgent nma;
	// Use this for initialization
	void Start () {
		nma = GetComponent<NavMeshAgent> ();
	}
	
	// Update is called once per frame
	void Update () {
		StartCoroutine(MyCoroutine ());

	}

	IEnumerator MyCoroutine()
	{
		//This is a coroutine
		
		yield return new WaitForSeconds(Random.Range(2, 5));
		nma.SetDestination (Random.insideUnitSphere * Random.Range(10,100));
	}
}
