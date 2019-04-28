using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class InputManager 
{
	public static bool rightTriggerPressed = false;
	public static bool leftTriggerPressed = false;
	
	//Axis
	public static float MainHorizontal()
	{
		float r = 0.0f;
		r += Input.GetAxis ("J_MainHorizontal");
		r += Input.GetAxis ("K_MainHorizontal");
		return Mathf.Clamp (r, -1.0f, 1.0f);
	}
	public static float MainVertical()
	{
		float r = 0.0f;
		r += Input.GetAxis ("J_MainVertical");
		r += Input.GetAxis ("K_MainVertical");
		return Mathf.Clamp (r, -1.0f, 1.0f);
	}

	public static Vector3 MainJoystick()
	{
		return new Vector3 (MainHorizontal (), 0, MainVertical ());
	}

	public static float CameraHorizontal()
	{
		return Input.GetAxis ("J_CameraHorizontal") * 3f;
	}
	public static float CameraVertical()
	{
		return Input.GetAxis ("J_CameraVertical") * 3f;
	}
	public static Vector2 CameraJoystick()
	{
		return new Vector2 (CameraHorizontal (), CameraVertical ());
	}

	//Buttons
	public static bool JumpButton()
	{
		return Input.GetButton("Jump");
	}
	public static bool CrouchButton()
	{
		return Input.GetButtonDown("Crouch");
	}
	public static bool ReloadButton()
	{
		return Input.GetButton("Reload");
	}
	public static bool InteractButton()
	{
		return Input.GetButton ("Interact");
	}
	public static bool InteractButtonDown()
	{
		return Input.GetButtonDown ("Interact");
	}

	public static bool AimButton()
	{
		return Input.GetButton("Aim");
	}
	public static bool AttackButton()
	{
		return Input.GetButtonDown("Attack");
	}
	public static bool RunButtonDown()
	{
		return Input.GetButtonDown ("Run");
	}
	public static bool TeleportButton()
	{
		return (LeftTrigger () > 0.8 || Input.GetButton ("Teleport"));
	}
	public static bool TeleportButtonDown()
	{
		return (LeftTrigger () > 0.8 || Input.GetButtonDown ("Teleport"));
	}
	public static bool PauseButton()
	{
		return Input.GetButtonDown ("Pause");
	}

	public static float LeftTrigger()
	{
		return Input.GetAxis ("Trigger");
	}
}
