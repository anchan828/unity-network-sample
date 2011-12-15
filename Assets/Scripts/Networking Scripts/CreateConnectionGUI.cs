using UnityEngine;
using System.Collections;

public class CreateConnectionGUI : MonoBehaviour
{

	public GameObject connectionGUI;
	void Awake ()
	{
		ConnectGuiMasterServer cgm = FindObjectOfType (typeof(ConnectGuiMasterServer)) as ConnectGuiMasterServer;
		if (cgm == null)
			Instantiate (connectionGUI, Vector3.zero, Quaternion.identity);
	}
}
