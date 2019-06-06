using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManagerController : InputManagerIF
{
	public static bool rightTriggerPressed = false;
	public static bool leftTriggerPressed = false;
	
	//Axis
	public float MainHorizontal()
	{
		float r = 0.0f;
		r += Input.GetAxis ("J_MainHorizontal");
		return Mathf.Clamp (r, -1.0f, 1.0f);
	}
	public float MainVertical()
	{
		float r = 0.0f;
		r += Input.GetAxis ("J_MainVertical");
		return Mathf.Clamp (r, -1.0f, 1.0f);
	}

	public Vector3 MainJoystick()
	{
		return new Vector3 (MainHorizontal (), 0, MainVertical ());
	}

	public float CameraHorizontal()
	{
		return Input.GetAxis ("J_CameraHorizontal") * 3f;
	}
	public float CameraVertical()
	{
		return Input.GetAxis ("J_CameraVertical") * 3f;
	}
	public Vector2 CameraJoystick()
	{
		return new Vector2 (CameraHorizontal (), CameraVertical ());
	}

	//Buttons
	public bool JumpButton()
	{
		return Input.GetButton("JumpJ");
	}
	public bool CrouchButton()
	{
		return Input.GetButtonDown("CrouchJ");
	}
	public bool ReloadButton()
	{
		return Input.GetButton("ReloadJ");
	}
	public bool InteractButton()
	{
		return Input.GetButton ("InteractJ");
	}
	public bool InteractButtonDown()
	{
		return Input.GetButtonDown ("InteractJ");
	}

	public bool DashButtonDown()
	{
		return Input.GetButtonDown("DashJ");
	}
	public bool AttackButton()
	{
		return Input.GetButtonDown("AttackJ");
	}
	public bool RunButtonDown()
	{
		return Input.GetButtonDown ("RunJ");
	}
	public bool TeleportButton()
	{
		return (LeftTrigger () > 0.8 || Input.GetButton ("TeleportJ"));
	}
	public bool TeleportButtonDown()
	{
		return (LeftTrigger () > 0.8 || Input.GetButtonDown ("TeleportJ"));
	}
	public bool PauseButton()
	{
		return Input.GetButtonDown ("PauseJ");
	}

	public float LeftTrigger()
	{
		return Input.GetAxis ("TriggerJ");
	}
}