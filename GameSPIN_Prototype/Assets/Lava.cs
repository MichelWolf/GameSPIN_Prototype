using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lava : MonoBehaviour
{
	private bool waiting=false;
    // Start is called before the first frame update
    void Start()
    {
        
    }
	
		
	void OnTriggerEnter(Collider col){
		if(col.gameObject.tag == "Player")
		{
				col.gameObject.GetComponent<Character>().TakeDamage(10);
		}
	}
	void OnTriggerStay(Collider col){
		if(!waiting && col.gameObject.tag == "Player") {
		StartCoroutine(wait(.5f));
		col.gameObject.GetComponent<Character>().TakeDamage(5);
		}

	}
	
	IEnumerator wait(float timeSec){
		waiting = true;
		yield return new WaitForSeconds(timeSec);
		waiting = false;
	}
}


