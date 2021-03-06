﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camMouseLook : MonoBehaviour {

	Vector2 mouseLook;
	Vector2 smoothV;

	public float sensitivity = 5.0f;
	public float smoothing = 2.0f;

	GameObject character;
	// Use this for initialization
	void Start () {
		character = this.transform.parent.transform.parent.gameObject;
	}

	// Update is called once per frame
	void Update () {
		if (Time.timeScale != 0)
		{
			var md = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
			if (md.x == 0 && md.y == 0)
			{
				md = InputManager.CameraJoystick();
			}
			md = Vector2.Scale(md, new Vector2(sensitivity * smoothing, sensitivity * smoothing));
			smoothV.x = Mathf.Lerp(smoothV.x, md.x, 1f / smoothing);
			smoothV.y = Mathf.Lerp(smoothV.y, md.y, 1f / smoothing);
			mouseLook += smoothV;
			mouseLook.y = Mathf.Clamp(mouseLook.y, -90, 90);

			transform.localRotation = Quaternion.AngleAxis(-mouseLook.y, Vector3.right);
			character.transform.localRotation = Quaternion.AngleAxis(mouseLook.x, character.transform.up);
		}
	}
}
