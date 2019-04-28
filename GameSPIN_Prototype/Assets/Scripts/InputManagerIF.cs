using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface InputManagerIF
{
	float MainHorizontal();
	float MainVertical();
	Vector3 MainJoystick();	
	bool JumpButton();
	bool CrouchButton();
	bool ReloadButton();
	bool InteractButton();
	bool InteractButtonDown();
	bool AimButton();
	bool AttackButton();
	bool RunButtonDown();
	bool TeleportButton();
	bool TeleportButtonDown();
	bool PauseButton();
	float LeftTrigger();
   
}
