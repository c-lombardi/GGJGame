using UnityEngine;
using System.Collections;

public class Multiplayer : MonoBehaviour {
	public Transform playerPrefab;
	public Transform pursuerPrefab;

	private static string typeName = "PanicAtTheWarehouse";
	private static string roomName = "" + System.Environment.MachineName;
	
	private Vector3 playerInitialPosition = Vector3.zero;
	private Quaternion playerInitialQuat = Quaternion.identity;

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
		MasterServer.ipAddress = "192.168.43.213";
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI() {
		int buttonNum = 0;
		if (!isSinglePlayer && !Network.isClient && !Network.isServer) {
			if (hostList == null) {
				if (GUI.Button(new Rect(buttonHStart, buttonY(buttonNum++), buttonWidth, buttonHeight), "Start Single-player")) {
					StartSinglePlayer();
				}
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
		isSinglePlayer = true;
		AddLocalPursuer(AddLocalPlayer());
	}
	void OnFailedToConnectToMasterServer(NetworkConnectionError info) {
		Debug.Log("Could not connect to master server: " + info);
	}

	void AddNetworkPlayer() {
		Transform player = AddLocalPlayer();
		Transform pursuer = (Transform)Network.Instantiate (pursuerPrefab, playerInitialPosition, playerInitialQuat, 0);
		pursuer.GetComponent<EnemySerializer>().player = player;
		pursuer.GetComponent<NetworkView> ().enabled = false;
		pursuer.GetComponent<NetworkView> ().enabled = true;
		for (int i = 0; i < pursuer.childCount; i++) {
			Object.Destroy(pursuer.GetChild(i).gameObject);
		}
	}
	Transform AddLocalPlayer() {
		Transform player = (Transform) Object.Instantiate (playerPrefab, playerInitialPosition, playerInitialQuat);
		GetComponent<CameraMovement>().target = player;
		return player;
	}
	Transform AddLocalPursuer(Transform player) {
		Transform pursuer = (Transform)Object.Instantiate (pursuerPrefab, playerInitialPosition, playerInitialQuat);
		pursuer.GetComponent<NavMeshAgent>().enabled = true;
		pursuer.GetComponent<NavToPlayer>().enabled = true;
		pursuer.GetComponent<NavToPlayer>().target = player;
		pursuer.GetComponent<NavToPlayer>().targetLight = player.GetComponent<Light2D>();
		return pursuer;
	}
}
