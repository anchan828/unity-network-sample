using UnityEngine;
using System.Collections;

public class ConnectGui : MonoBehaviour
{

	public string remoteIP = "127.0.0.1";
	public int remotePort = 25001;
	public int listenPort = 25000;
	public int remoteGUID = 0;
	public bool useNat = false;
//	private string connectionInfo = "";

	void Awake ()
	{
		if (FindObjectOfType (typeof(ConnectGuiMasterServer)))
			this.enabled = false;
	}

	void OnGUI ()
	{
		GUILayout.Space (10);
		GUILayout.BeginHorizontal ();
		GUILayout.Space (10);
		if (Network.peerType == NetworkPeerType.Disconnected) {
			useNat = GUILayout.Toggle (useNat, "Use NAT punchthrough");
			GUILayout.EndHorizontal ();
			GUILayout.BeginHorizontal ();
			GUILayout.Space (10);
			
			GUILayout.BeginVertical ();
			if (GUILayout.Button ("Connect")) {
				if (useNat) {
					if (remoteGUID != 0)
						Debug.LogWarning ("Invalid GUID given, must be a valid one as reported by Network.player.guid or returned in a HostData struture from the master server");
					else
						Network.Connect (remoteGUID.ToString ());
				} else {
					Network.Connect (remoteIP, remotePort);
				}
			}
			if (GUILayout.Button ("Start Server")) {
				Network.InitializeServer (32, listenPort, useNat);
				// Notify our objects that the level and the network is ready
				foreach (GameObject go in FindObjectsOfType (typeof(GameObject)))
					go.SendMessage ("OnNetworkLoadedLevel", SendMessageOptions.DontRequireReceiver);
			}
			GUILayout.EndVertical ();
			if (useNat) {
				remoteGUID = int.Parse (GUILayout.TextField (remoteGUID.ToString (), GUILayout.MinWidth (145)));
			} else {
				remoteIP = GUILayout.TextField (remoteIP, GUILayout.MinWidth (100));
				remotePort = int.Parse (GUILayout.TextField (remotePort.ToString ()));
			}
		} else {
			if (useNat)
				GUILayout.Label ("GUID: " + Network.player.guid + " - ");
			GUILayout.Label ("Local IP/port: " + Network.player.ipAddress + "/" + Network.player.port);
			GUILayout.Label (" - External IP/port: " + Network.player.externalIP + "/" + Network.player.externalPort);
			GUILayout.EndHorizontal ();
			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Disconnect"))
				Network.Disconnect (200);
		}
		GUILayout.FlexibleSpace ();
		GUILayout.EndHorizontal ();
	}

	void OnServerInitialized ()
	{
		if (useNat)
			Debug.Log ("==> GUID is " + Network.player.guid + ". Use this on clients to connect with NAT punchthrough.");
		Debug.Log ("==> Local IP/port is " + Network.player.ipAddress + "/" + Network.player.port + ". Use this on clients to connect directly.");
	}

	void OnConnectedToServer ()
	{
		// Notify our objects that the level and the network is ready
		foreach (GameObject go in FindObjectsOfType (typeof(GameObject)))
			go.SendMessage ("OnNetworkLoadedLevel", SendMessageOptions.DontRequireReceiver);
	}

	void OnDisconnectedFromServer ()
	{
		if (this.enabled != false)
			Application.LoadLevel (Application.loadedLevel);
		else {
			NetworkLevelLoad n = (NetworkLevelLoad)FindObjectOfType (typeof(NetworkLevelLoad));
			n.OnDisconnectedFromServer ();
		}
	}
}
