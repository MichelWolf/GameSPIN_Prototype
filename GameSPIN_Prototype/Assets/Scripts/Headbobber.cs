using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Headbobber: MonoBehaviour 
{

	private float timer = 0.0f;
	[Header("Gehen")]
	[Tooltip("Werte für den Headbob beim normalen gehen")]
	public float walkBobbingSpeed = 0.18f;
	public float walkBobbingAmount = 0.2f;
	[Header("Rennen")]
	public float runBobbingSpeed = 0.18f;
	public float runBobbingAmount = 0.2f;
	[Header("Schleichen")]
	public float crouchBobbingSpeed = 0.18f;
	public float crouchBobbingAmount = 0.2f;
	[Header("Kopfhöhe")]
	public float midpoint = 0f;

	public float standHeight = 1.8f;
	public float crouchHeight = 1.2f;
	public bool playedFootstep = false;
	float waveslice = 0.0f;

	Vector3 bobs;// = transform.localPosition;

	Character player;
	
	//Control Settings
	[Header("Controller")]
	public bool mouse = true;
	private InputManagerIF inputManager = new InputManagerMouse();
	void Start()
	{
		if(!mouse){
			inputManager = new InputManagerController();
		}
		player = FindObjectOfType<Character> ();
		bobs = transform.localPosition;
	}

	void Update ()
	{
		if (Time.timeScale != 0) 
		{

			waveslice = 0.0f;
			if (player.isGrounded) {
				//float horizontal = Input.GetAxis ("Horizontal");
				//float vertical = Input.GetAxis ("Vertical");
				Vector3 input = inputManager.MainJoystick ();



				if (Mathf.Abs (input.x) == 0 && Mathf.Abs (input.z) == 0) {
					timer = 0.0f;
				} else {

					if (player.walkingMode == Character.WalkingMode.walking) {
						timer = timer + walkBobbingSpeed;
					} else if (player.walkingMode == Character.WalkingMode.running) {
						timer = timer + runBobbingSpeed;
					} else if (player.walkingMode == Character.WalkingMode.crouching) {
						timer = timer + crouchBobbingSpeed;
					}

					if (timer > Mathf.PI * 2) {
						timer = timer - (Mathf.PI * 2);
					}
					//Debug.Log ("Timer: " + timer);
					waveslice = Mathf.Sin (timer);
					//Debug.Log ("Waveslice: " + waveslice);
				}
				if (waveslice != 0) {
					float translateChange = 0f;
					if (player.walkingMode == Character.WalkingMode.walking) {
						translateChange = waveslice * walkBobbingAmount;
					} else if (player.walkingMode == Character.WalkingMode.running) {
						translateChange = waveslice * runBobbingAmount;
					} else if (player.walkingMode == Character.WalkingMode.crouching) {
						translateChange = waveslice * crouchBobbingAmount;
					}

					float totalAxes = Mathf.Abs (input.x) + Mathf.Abs (input.z);
					totalAxes = Mathf.Clamp (totalAxes, 0.0f, 1.0f);
					translateChange = totalAxes * translateChange;
					bobs.y = midpoint + translateChange;
				} else {
					bobs.y = midpoint;
				}
				if (waveslice < -0.95 && !playedFootstep) {
					if (player.walkingMode != Character.WalkingMode.crouching) {
						//FMODUnity.RuntimeManager.PlayOneShotAttached (player.FootstepsEvent, this.transform.gameObject);
						playedFootstep = true;
					} else {
						//FMODUnity.RuntimeManager.PlayOneShotAttached (player.CrouchingFootstepsEvent, this.transform.gameObject);
						playedFootstep = true;
					}
				}
				if (waveslice > 0.5) {
					playedFootstep = false;
				}
				transform.localPosition = bobs;
			} else {
				timer = 0.0f;
				waveslice = 0.0f;
			}

			Vector3 parentPos = transform.parent.gameObject.transform.localPosition;
			if (player.walkingMode == Character.WalkingMode.walking || player.walkingMode == Character.WalkingMode.running) {
				if (parentPos.y < standHeight) {
					parentPos.y += 0.05f;
				}
				parentPos.y = Mathf.Clamp (parentPos.y, 0, standHeight);
			} else if (player.walkingMode == Character.WalkingMode.crouching) {
				if (parentPos.y > crouchHeight) {
					parentPos.y -= 0.05f;
				}
				parentPos.y = Mathf.Clamp (parentPos.y, crouchHeight, Mathf.Infinity);
			}
			transform.parent.gameObject.transform.localPosition = parentPos;

		}

	}

}