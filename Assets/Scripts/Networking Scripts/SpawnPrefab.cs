using UnityEngine;
using System.Collections;

public class SpawnPrefab : MonoBehaviour
{

	public Transform playerPrefab;

	void OnNetworkLoadedLevel ()
	{
		Network.Instantiate (playerPrefab, transform.position, transform.rotation, 0);
	}

	void OnPlayerDisconnected (NetworkPlayer player)
	{
		Debug.Log ("Server destroying player");
		Network.RemoveRPCs (player, 0);
		Network.DestroyPlayerObjects (player);
	}
	
}
