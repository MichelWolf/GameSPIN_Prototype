using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gravity : MonoBehaviour {

    internal CharacterController charContr;
    public float verticalVelocity;
    public float gravity = 20.0f;
    // Use this for initialization
    void Start () {
        charContr = GetComponent<CharacterController>();
	}
	
	// Update is called once per frame
	void Update () {
        verticalVelocity -= gravity * Time.deltaTime;
        charContr.Move(new Vector3(0, verticalVelocity, 0) * Time.deltaTime);
    }
}
