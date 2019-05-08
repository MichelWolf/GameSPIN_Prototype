using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPartHit : MonoBehaviour
{
	//0 -> no player hits part of mesh ; 1 -> player 1 hits ; 2 -> player 2 hits ; 3 -> both player hit 
	public int hitByPlayers;
	public float damagePerHit=5;
	
	public float damageCapacity=20;
	public bool detachable=false;
	public GameObject detachableObject;
	
	public Golem golem;

    void Start()
    {
        hitByPlayers=0;
    }
	
	
	public void attack(){
		golem.receiveDamage(damagePerHit);
		damageCapacity -= damagePerHit;
		if(detachable && damageCapacity < 0){
		//	Destroy(gameObject.GetComponent("SkinnedMeshRenderer"));
			detachableObject.SetActive(true);
			this.gameObject.SetActive(false);
		}
	}

	
}