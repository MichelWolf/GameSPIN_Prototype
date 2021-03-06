﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface InputManagerIF
{
	float MainHorizontal();
	float MainVertical();
	Vector3 MainJoystick();

    float CameraHorizontal();
    float CameraVertical();
    Vector2 CameraJoystick();

    bool JumpButton();
	bool CrouchButton();
	bool ReloadButton();
	bool InteractButton();
	bool InteractButtonDown();
	bool DashButtonDown();
	bool AttackButtonRight();
    bool AttackButtonLeft();
    bool RunButtonDown();
	bool TeleportButton();
	bool TeleportButtonDown();
	bool PauseButton();
	float LeftTrigger();
   
}
