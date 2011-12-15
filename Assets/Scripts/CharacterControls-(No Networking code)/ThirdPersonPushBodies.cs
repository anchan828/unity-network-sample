using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ThirdPersonController))]
public class ThirdPersonPushBodies : MonoBehaviour {

public float pushPower = 0.5f;
public LayerMask pushLayers = -1;
private ThirdPersonController controller  ;


void Start ()
{
	controller = GetComponent <ThirdPersonController>();
}

void OnControllerColliderHit (ControllerColliderHit hit)
{
	Rigidbody body   = hit.collider.attachedRigidbody;
	// no rigidbody
	if (body == null || body.isKinematic)
		return;
	// Ignore pushing those rigidbodies
	int bodyLayerMask = 1 << body.gameObject.layer;
	if ((bodyLayerMask & pushLayers.value) == 0)
		return;
		
	// We dont want to push objects below us
	if (hit.moveDirection.y < -0.3) 
		return;
	
	// Calculate push direction from move direction, we only push objects to the sides
	// never up and down
	Vector3 pushDir =new Vector3 (hit.moveDirection.x, 0, hit.moveDirection.z);
	
	// push with move speed but never more than walkspeed
	body.velocity = pushDir * pushPower * Mathf.Min(controller.GetSpeed(), controller.walkSpeed);
}
}
