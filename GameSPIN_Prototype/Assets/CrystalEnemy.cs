﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalEnemy : MonoBehaviour
{
	public float health;
	public int damageToPlayer;
	public float damageOnHit;
	public float speed;
	public GameObject targetPlayer;
	
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
		moveToUntargetedPlayer();
    }
	
	public void receiveDamage(){
		health -= damageOnHit;
		if(health <= 0){
			Destroy(gameObject);
		}
	}
	
	void OnTriggerEnter(Collider col){
		if(col.gameObject.tag == "Player")
		{
			col.gameObject.GetComponent<Character>().TakeDamage(damageToPlayer);
			Destroy(gameObject);
		}
	}
	
	void moveToUntargetedPlayer(){
		if(speed != 0){				
			var targetPoint = targetPlayer.transform.position;
			var targetRotation = Quaternion.LookRotation (targetPoint - transform.position, Vector3.up);
			transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * speed);
			transform.position += transform.forward * (speed * Time.deltaTime);
				
		}     
	}
	
	public void setTargetPlayer(GameObject player){
		targetPlayer = player;
	}
}