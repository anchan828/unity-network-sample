using UnityEngine;
using System.Collections;

[AddComponentMenu("Third Person Player/Third Person Player Animation")]
public class ThirdPersonSimpleAnimation : MonoBehaviour
{

	public float runSpeedScale = 1.0f;
	public float walkSpeedScale = 1.0f;
	public Transform torso;
	private NetworkSyncAnimation networkSyncAnimation;
	void Awake ()
	{
		// By default loop all animations
		animation.wrapMode = WrapMode.Loop;
		
		// We are in full control here - don't let any other animations play when we start
		animation.Stop ();
		animation.Play ("idle");
	}

	void Update ()
	{
		ThirdPersonController marioController = GetComponent<ThirdPersonController> ();
		float currentSpeed = marioController.GetSpeed ();
		 networkSyncAnimation = GetComponent<NetworkSyncAnimation> ();
		
		// Fade in run
		if (currentSpeed > marioController.walkSpeed) {
			animation.CrossFade ("run");
			// We fade out jumpland quick otherwise we get sliding feet
			animation.Blend ("jumpland", 0);
			networkSyncAnimation.SendMessage ("SyncAnimation", "run");
			// Fade in walk
		} else if (currentSpeed > 0.1f) {
			animation.CrossFade ("walk");
			// We fade out jumpland realy quick otherwise we get sliding feet
			animation.Blend ("jumpland", 0);
			networkSyncAnimation.SendMessage ("SyncAnimation", "walk");
			// Fade out walk and run
		} else {
			animation.CrossFade ("idle");
			networkSyncAnimation.SendMessage ("SyncAnimation", "idle");
		}
		
		animation["run"].normalizedSpeed = runSpeedScale;
		animation["walk"].normalizedSpeed = walkSpeedScale;
		
		if (marioController.IsJumping ()) {
			if (marioController.IsCapeFlying ()) {
				animation.CrossFade ("jetpackjump", 0.2f);
				networkSyncAnimation.SendMessage ("SyncAnimation", "jetpackjump");
			} else if (marioController.HasJumpReachedApex ()) {
				animation.CrossFade ("jumpfall", 0.2f);
				networkSyncAnimation.SendMessage ("SyncAnimation", "jumpfall");
			} else {
				animation.CrossFade ("jump", 0.2f);
				networkSyncAnimation.SendMessage ("SyncAnimation", "jump");
			}
			// We fell down somewhere
		} else if (!marioController.IsGroundedWithTimeout ()) {
			animation.CrossFade ("ledgefall", 0.2f);
			networkSyncAnimation.SendMessage ("SyncAnimation", "ledgefall");
			// We are not falling down anymore
		} else {
			animation.Blend ("ledgefall", 0.0f, 0.2f);
		}
	}

	public void DidLand ()
	{
		animation.Play ("jumpland");
		networkSyncAnimation.SendMessage ("SyncAnimation", "jumpland");
	}

	public void DidPunch ()
	{
		animation.CrossFadeQueued ("punch", 0.3f, QueueMode.PlayNow);
	}

	public void DidButtStomp ()
	{
		animation.CrossFade ("buttstomp", 0.1f);
		networkSyncAnimation.SendMessage ("SyncAnimation", "buttstomp");
		animation.CrossFadeQueued ("jumpland", 0.2f);
	}

	public void ApplyDamage ()
	{
		animation.CrossFade ("gothit", 0.1f);
		networkSyncAnimation.SendMessage ("SyncAnimation", "gothit");
	}


	public void DidWallJump ()
	{
		// Wall jump animation is played without fade.
		// We are turning the character controller 180 degrees around when doing a wall jump so the animation accounts for that.
		// But we really have to make sure that the animation is in full control so 
		// that we don't do weird blends between 180 degree apart rotations
		animation.Play ("walljump");
		networkSyncAnimation.SendMessage ("SyncAnimation", "walljump");
	}
	
	
}
