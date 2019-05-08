using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolemProjectileMove : MonoBehaviour
{
    // Start is called before the first frame update
	
	public float speed;
	public float fireRate;
	public int spellDamageImpact;	
	public float range;
	
	public bool staticSpell;
	public bool dot;
	public int dotDmg;
	
	private Vector3 startPos = new Vector3(0,0,0);
	private bool waiting=false;
    void Start()
    {
		startPos +=transform.position;         
    }

    // Update is called once per frame
    void Update()
    {
		if(speed != 0){
				if(!staticSpell){
					transform.position += transform.forward * (speed * Time.deltaTime);
				} else {
					transform.position += Vector3.up * (speed * Time.deltaTime);
				}
				isOutOfRange();
		} 
		
        
    }
	
	void OnTriggerEnter(Collider col){
		if(col.gameObject.tag == "Player")
		{
			if(!dot){
				col.gameObject.GetComponent<Character>().TakeDamage(spellDamageImpact);
				Destroy(gameObject);
			} else {
				col.gameObject.GetComponent<Character>().TakeDamage(dotDmg);
			}
		}

	}
	void OnTriggerStay(Collider col){
		if(!waiting && col.gameObject.tag == "Player") {
		StartCoroutine(wait(.5f));
			if(!dot){
				col.gameObject.GetComponent<Character>().TakeDamage(spellDamageImpact);
				} else {
				col.gameObject.GetComponent<Character>().TakeDamage(dotDmg);
				}			
			}
	}
	public void isOutOfRange(){
		if(Vector3.Distance(startPos, transform.position) >= range){
			Destroy(gameObject);
		}
	}
	
	IEnumerator wait(float timeSec){
		waiting = true;
		yield return new WaitForSeconds(timeSec);
		waiting = false;
	}
}
