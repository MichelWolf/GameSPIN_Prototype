﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Golem : MonoBehaviour
{
    // Start is called before the first frame update
	public float hitpoints = 100;
	private float currentHitpoints;
	private bool paused;
    public SimpleHealthBar healthBar;
	private GameObject[] players;
	private GameObject target;
	private GameObject secondTarget;
	private float aggroRange;
	private Animator anim;
	
	[Header("AttackProj")]
	public GameObject firePoint;
	public List<GameObject> vfx = new List<GameObject>();
	public GameObject crystalEnemy;
	private GameObject effectToSpawn;
	private Quaternion targetRot;
	private float speed = 0;
	private GameObject instantiatedProj;
	private float attackpause=2f;
    void Start()
    {
		aggroRange = 20f;
		currentHitpoints = hitpoints;
        healthBar.UpdateBar( hitpoints, hitpoints );
		players = GameObject.FindGameObjectsWithTag("Player");
		anim = this.gameObject.GetComponent<Animator>();
		effectToSpawn = vfx[0];
		speed += effectToSpawn.GetComponent<GolemProjectileMove>().speed;
    }
	

	
    void Update()
    {
        if(!paused && target != null){
			StartCoroutine(attack());			
		}		
				findClosestTarget();
    }
	
	bool playerInRange(){
		return true;
	}
	
	public IEnumerator attack(){
		// Execute animation and particle system
		paused = true;
		int randomNumber = Random.Range(1,3);
		if(randomNumber == 1){
			effectToSpawn = vfx[0];
		} else {effectToSpawn = vfx[1];}
		GameObject crystal;
		crystal = Instantiate(crystalEnemy,transform.position,transform.rotation);
		crystal.GetComponent<CrystalEnemy>().setTargetPlayer(secondTarget);
		anim.SetTrigger("atk"+randomNumber);	
		//SpawnVFX();
		if(anim.GetCurrentAnimatorStateInfo(0).IsName("Attack1") || anim.GetCurrentAnimatorStateInfo(0).IsName("Attack2")){
		yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length+anim.GetCurrentAnimatorStateInfo(0).normalizedTime);
		}else{
			yield return new WaitForSeconds(2f);
		}
		
		yield return new WaitForSeconds(attackpause);
		paused = false;
		
	}
	
	public void receiveDamage(float damage){
		currentHitpoints = currentHitpoints - damage;
		healthBar.UpdateBar( currentHitpoints, hitpoints );
	}
	
	public void findClosestTarget(){
		float disttmp=aggroRange;
		target = null;
		foreach(GameObject enemy in players){
			float dist = Vector3.Distance(enemy.transform.position, transform.position);
			
			if(dist < aggroRange && dist < disttmp ){
				secondTarget = target;
				disttmp = dist;
				target = enemy;
			}
		}
		if(target != null){
		 var targetPoint = target.transform.position;
		var targetRotation = Quaternion.LookRotation (targetPoint - transform.position, Vector3.up);
		targetRot = targetRotation;
		transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 2.0f);
		}
	}
	
	public void fireVFX(){
		instantiatedProj.GetComponent<GolemProjectileMove>().dot = false;
		var targetPoint = target.transform.position;
		var targetRotation = Quaternion.LookRotation (targetPoint - transform.position, Vector3.up);
		instantiatedProj.transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 0.1f);
		instantiatedProj.GetComponent<GolemProjectileMove>().speed += speed;
	}
	
		void SpawnVFX(){
		GameObject vfx;
		if(firePoint != null){
			GameObject aimAtObj = new GameObject();
			aimAtObj.transform.position = target.transform.position;
				aimAtObj.transform.rotation = target.transform.rotation;
				//target.GetComponent<CapsuleCollider>().height 
			aimAtObj.transform.position += new Vector3(0f, 1.8f+ 0.34f,0f);
			firePoint.transform.LookAt(aimAtObj.transform);
			//Vector3 temp = firePoint.transform.rotation.eulerAngles;
			//temp.y += target.GetComponent<CharacterController>().height;
			//Debug.Log(target.GetComponent<CapsuleCollider>().height);
			
			
			vfx = Instantiate(effectToSpawn, new Vector3(firePoint.transform.position.x,  target.transform.position.y +1.8f+ 0.34f, firePoint.transform.position.z), firePoint.transform.rotation);
			vfx.GetComponent<GolemProjectileMove>().speed = 0;
			instantiatedProj = vfx;
			Debug.Log("out");
		} else {
			Debug.Log("No Fire Point");
		}
	}
	
	public void spawnVFXPlayer(){
		GameObject vfx;
		vfx = Instantiate(effectToSpawn, new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z), target.transform.rotation);
		vfx.GetComponent<GolemProjectileMove>().speed = 0;
		instantiatedProj = vfx;
	}
}