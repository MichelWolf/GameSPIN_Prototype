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
    void Start()
    {
		currentHitpoints = hitpoints;
        healthBar.UpdateBar( hitpoints, hitpoints );
    }

    // Update is called once per frame
    void Update()
    {
        if(!paused && playerInRange()){
			StartCoroutine(attack());			
		}		
    }
	
	bool playerInRange(){
		return true;
	}
	
	public IEnumerator attack(){
		// Execute animation and particle system
		paused = true;
		int randomNumber = Random.Range(1,2);
		yield return new WaitForSeconds(4);
		paused = false;
		
	}
	
	public void doDamage(float damage){
		currentHitpoints = currentHitpoints - damage;
		healthBar.UpdateBar( currentHitpoints, hitpoints );
	}
}
