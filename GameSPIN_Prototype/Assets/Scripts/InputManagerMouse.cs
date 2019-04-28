using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManagerMouse :InputManagerIF
{
	public static bool rightTriggerPressed = false;
	public static bool leftTriggerPressed = false;
	
	//Axis
	public float MainHorizontal()
	{
		float r = 0.0f;
		r += Input.GetAxis ("K_MainHorizontal");
		return Mathf.Clamp (r, -1.0f, 1.0f);
	}
	public float MainVertical()
	{
		float r = 0.0f;
		r += Input.GetAxis ("K_MainVertical");
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
		return Input.GetButton("Jump");
	}
	public bool CrouchButton()
	{
		return Input.GetButtonDown("Crouch");
	}
	public bool ReloadButton()
	{
		return Input.GetButton("Reload");
	}
	public bool InteractButton()
	{
		return Input.GetButton ("Interact");
	}
	public bool InteractButtonDown()
	{
		return Input.GetButtonDown ("Interact");
	}

	public bool AimButton()
	{
		return Input.GetButton("Aim");
	}
	public bool AttackButton()
	{
		return Input.GetButtonDown("Attack");
	}
	public bool RunButtonDown()
	{
		return Input.GetButtonDown ("Run");
	}
	public bool TeleportButton()
	{
		return (LeftTrigger () > 0.8 || Input.GetButton ("Teleport"));
	}
	public bool TeleportButtonDown()
	{
		return (LeftTrigger () > 0.8 || Input.GetButtonDown ("Teleport"));
	}
	public bool PauseButton()
	{
		return Input.GetButtonDown ("Pause");
	}

	public float LeftTrigger()
	{
		return Input.GetAxis ("Trigger");
	}
}