using UnityEngine;
using System.Collections;

public class Menu : MonoBehaviour {
	public Transform playerPrefab;
	public Transform pursuerPrefab;

	private static string typeName = "PanicAtTheWarehouse";
	private static string roomName = "" + System.Environment.MachineName;

	private int buttonHeight = 30;
	private int buttonWidth = 250;
	private int buttonVSpacing = 10;
	private int buttonVStart = 30;
	private int buttonHStart = 30;

	private int buttonY(int buttonNum) {
		return buttonVStart + (buttonHeight + buttonVSpacing) * buttonNum;
	}
	
	private bool isSinglePlayer = false;
	private HostData[] hostList = null;

	// Use this for initialization
	void Start () {
		MasterServer.ipAddress = "192.168.1.7";
	}
	
	// Update is called once per frame
	void Update () {
	
	}


	 void OnGUI() {
		int buttonNum = 0;
		if (!isSinglePlayer && !Network.isClient && !Network.isServer) {
			if (hostList == null) {
				//if (GUI.Button(new Rect(buttonHStart, buttonY(buttonNum++), buttonWidth, buttonHeight), "Start Single-player")) {
					//StartSinglePlayer();
				//}
				if (GUI.Button(new Rect(buttonHStart, buttonY(buttonNum++), buttonWidth, buttonHeight), "Multiplayer")) {
					FindServer();
				}
			} else {
				if (GUI.Button(new Rect(buttonHStart, buttonY(buttonNum++), buttonWidth, buttonHeight), "Start New Room")) {
					StartServer();
				}
				for (int i = 0; i < hostList.Length; i++)
				{
					if (GUI.Button(new Rect(buttonHStart, buttonY(buttonNum++), buttonWidth, buttonHeight), "Join " + hostList[i].gameName)) {
						JoinServer(hostList[i]);
					}
				}
			}
		}
	}


	private void StartServer() {
		Network.InitializeServer(4, 25000, false);
	}
	void OnServerInitialized() {
		AddNetworkPlayer();
		MasterServer.RegisterHost(typeName, roomName);
	}

	
	private void FindServer()
	{
		MasterServer.RequestHostList(typeName);
	}
	
	void OnMasterServerEvent(MasterServerEvent msEvent)
	{
		switch (msEvent) {
			case MasterServerEvent.RegistrationSucceeded:
				break;
			case MasterServerEvent.HostListReceived:
				hostList = MasterServer.PollHostList ();
				break;

		}
	}

	private void JoinServer(HostData hostData) {
		Network.Connect(hostData);
	}
	
	void OnConnectedToServer()
	{
		hostList = null;
		AddNetworkPlayer ();
	}

	void StartSinglePlayer() {
		/*
		isSinglePlayer = true;
		AddLocalPursuer(AddLocalPlayer());
		*/
		Application.LoadLevel ("SinglePlayer");
	}
	void OnFailedToConnectToMasterServer(NetworkConnectionError info) {
		Debug.Log("Could not connect to master server: " + info);
	}


	private Vector3 playerInitialPosition() {
		int random = Mathf.FloorToInt(Random.Range(0.0F, 4.0F));

		if (random == 0) {
			Vector3 v = transform.position;
			v.y = 0;
			v.x = 11.72f;
			v.z = -6.1f;
			return v;
		}
		else if (random == 1) {
			Vector3 v = transform.position;
			v.y = 0;
			v.x = -10.87327f;
			v.z = -6.419117f;
			return v;
		}
		else if (random == 2) {
			Vector3 v = transform.position;
			v.y = 0;
			v.x = -11.47327f;
			v.z = 14.91088f;
			return v;
		}
		else {
			Vector3 v = transform.position;
			v.y = 0;
			v.x = 1.61f;
			v.z = 7.97f;
			return v;
		}


	}

	private Vector3 pursuerInitalPosition() {
		// They probably shouldn't show up in the same place
		return playerInitialPosition ();
	}

	void AddNetworkPlayer() {
		Transform player = AddLocalPlayer();
		Transform pursuer = (Transform)Network.Instantiate (pursuerPrefab, playerInitialPosition(), Quaternion.identity, 0);
		pursuer.GetComponent<EnemySerializer>().player = player;
		pursuer.GetComponent<NetworkView> ().enabled = false;
		pursuer.GetComponent<NetworkView> ().enabled = true;
		for (int i = 0; i < pursuer.childCount; i++) {
			Object.Destroy(pursuer.GetChild(i).gameObject);
		}
	}
	Transform AddLocalPlayer() {
		Transform player = (Transform) Object.Instantiate (playerPrefab, playerInitialPosition(), Quaternion.identity);
		GetComponent<CameraMovement>().target = player;
		return player;
	}
	Transform AddLocalPursuer(Transform player) {
		Transform pursuer = (Transform)Object.Instantiate (pursuerPrefab, playerInitialPosition(), Quaternion.identity);
		pursuer.GetComponent<NavMeshAgent>().enabled = true;
		pursuer.GetComponent<NavToPlayer>().enabled = true;
		pursuer.GetComponent<NavToPlayer>().target = player;
		pursuer.GetComponent<NavToPlayer>().targetLight = player.GetComponent<Light2D>();
		return pursuer;
	}
}
