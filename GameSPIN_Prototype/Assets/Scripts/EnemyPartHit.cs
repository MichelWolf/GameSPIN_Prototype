using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPartHit : MonoBehaviour
{
	//0 -> no player hits part of mesh ; 1 -> player 1 hits ; 2 -> player 2 hits ; 3 -> both player hit 
	public int hitByPlayers;
	public float damagePerHit=5;
	
	public float damageCapacity=20;
    internal float maxCapacity = 20;
	public bool detachable=false;
	public GameObject detachableObject;
    public bool active = false;
	
	public Golem golem;

    void Start()
    {
        hitByPlayers=0;
    }
	
	
	public void attack(){
        if(active && golem.state == true)
        { 
		    golem.receiveDamage(damagePerHit);
		    damageCapacity -= damagePerHit;
		    if(detachable && damageCapacity < 0){
		    //	Destroy(gameObject.GetComponent("SkinnedMeshRenderer"));
			    detachableObject.SetActive(true);
			    this.gameObject.SetActive(false);
		    }
            else if (damageCapacity <= 0)
            {
                active = false;
                this.GetComponent<SkinnedMeshRenderer>().material.DisableKeyword("_EMISSION");
                damageCapacity = maxCapacity;
                golem.ActivateNewExplodingCrystal();
            }
        }
	}

	
}