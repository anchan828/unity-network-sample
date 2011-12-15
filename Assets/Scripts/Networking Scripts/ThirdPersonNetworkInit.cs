using UnityEngine;
using System.Collections;

public class ThirdPersonNetworkInit : MonoBehaviour
{
	void OnNetworkInstantiate (NetworkMessageInfo msg)
	{
		// This is our own player
		if (networkView.isMine) {
			Camera.main.SendMessage ("SetTarget", transform);
			GetComponent<NetworkInterpolatedTransform> ().enabled = false;
			// This is just some remote controlled player
		} else {
			name += "Remote";
			GetComponent<ThirdPersonController> ().enabled = false;
			GetComponent<ThirdPersonSimpleAnimation> ().enabled = false;
			GetComponent<NetworkInterpolatedTransform> ().enabled = true;
		}
	}
}
