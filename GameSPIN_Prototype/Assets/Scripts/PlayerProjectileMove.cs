using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProjectileMove : MonoBehaviour
{
    // Start is called before the first frame update
	
	public float speed;
	public float fireRate;
	public int spellDamage;
	public float range;
	private UiScript ui;
	public GameObject explosion;
    public GameObject crystalForceField;
	private Vector3 startPos = new Vector3(0,0,0);
    void Start()
    {
		startPos +=transform.position; 
    }

    // Update is called once per frame
    void Update()
    {
		if(speed != 0){				
				transform.position += transform.forward * (speed * Time.deltaTime);
				isOutOfRange();
		}      
    }
	
	void OnTriggerEnter(Collider col){
		if(col.gameObject.tag == "Enemy")
		{
			Instantiate(explosion,transform.position, transform.rotation);
			col.gameObject.GetComponent<ColliderPartGolem>().hitCollider();
			if(ui != null){
			ui.activateHitmarker();
		}
			Destroy(gameObject);		
		}else if(col.gameObject.tag == "EnemySmall"){
			Instantiate(explosion,transform.position, transform.rotation);
			col.gameObject.GetComponent<CrystalEnemy>().receiveDamage();
			if(ui != null){
			ui.activateHitmarker();
			}
			Destroy(gameObject);
		}
        else if(col.gameObject.tag == "ExplodingCrystal")
        {
            Instantiate(crystalForceField, transform.position, transform.rotation);
            Destroy(col.gameObject);
        }
	}
	
	public void isOutOfRange(){
		if(Vector3.Distance(startPos, transform.position) >= range){
			Destroy(gameObject);
		}
	}
	
	public void setUiScript(UiScript tUi){
		ui=tUi;
	}
	
	void OnDestroy(){
		//Debug.Log("destr");
		//GameObject vfx = Instantiate(explosion,transform.position, transform.rotation);
	}
}
