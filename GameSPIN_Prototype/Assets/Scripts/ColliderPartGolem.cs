using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderPartGolem : MonoBehaviour
{
	public GameObject collisionObject;
	
    // Start is called before the first frame update
    void Start()
    {
        if(collisionObject == null){
			Debug.Log("Collider has no object attached");
		}
    }

    // Update is called once per frame
    void Update()
    {
        
    }
	
	public void hitCollider(){
		collisionObject.gameObject.GetComponent<EnemyPartHit>().attack();
	}
}
