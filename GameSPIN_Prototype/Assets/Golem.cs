using System.Collections;
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
	private float aggroRange;
	private Animator anim;
    void Start()
    {
		aggroRange = 20f;
		currentHitpoints = hitpoints;
        healthBar.UpdateBar( hitpoints, hitpoints );
		players = GameObject.FindGameObjectsWithTag("Player");
		anim = this.gameObject.GetComponent<Animator>();
    }
	

	
    void Update()
    {
        if(!paused && target != null){
			StartCoroutine(attack());			
		}		
			 
             //   foreach(GameObject player in players)
              //  {
					
				//}
				findClosestTarget();
    }
	
	bool playerInRange(){
		return true;
	}
	
	public IEnumerator attack(){
		// Execute animation and particle system
		paused = true;
		int randomNumber = Random.Range(1,3);
		anim.SetTrigger("atk"+randomNumber);		
		if(anim.GetCurrentAnimatorStateInfo(0).IsName("Attack1") || anim.GetCurrentAnimatorStateInfo(0).IsName("Attack2")){
		yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length+anim.GetCurrentAnimatorStateInfo(0).normalizedTime);
		}else{
			yield return new WaitForSeconds(0f);
		}
	
		paused = false;
		
	}
	
	public void doDamage(float damage){
		currentHitpoints = currentHitpoints - damage;
		healthBar.UpdateBar( currentHitpoints, hitpoints );
	}
	
	public void findClosestTarget(){
		float disttmp=aggroRange;
		target = null;
		foreach(GameObject enemy in players){
			float dist = Vector3.Distance(enemy.transform.position, transform.position);
			
			if(dist < aggroRange && dist < disttmp ){
				disttmp = dist;
				target = enemy;
			}
		}
		if(target != null){
		 var targetPoint = target.transform.position;
		var targetRotation = Quaternion.LookRotation (targetPoint - transform.position, Vector3.up);
		transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 2.0f);
		}
	}
}
