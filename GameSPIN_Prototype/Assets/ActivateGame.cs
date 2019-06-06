using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateGame : MonoBehaviour
{
    public GameObject startExplosionPosition;
    public Material basicCrystalMaterial;
    public GameObject explosionPrefab;
    // Start is called before the first frame update
    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            Instantiate(explosionPrefab, startExplosionPosition.transform.position, startExplosionPosition.transform.rotation);
            startExplosionPosition.gameObject.GetComponent<MeshRenderer>().material = basicCrystalMaterial;
            //FindObjectOfType<Golem>().ownCrystals[0].GetComponent<SkinnedMeshRenderer>().material.EnableKeyword("_EMISSION");
            //FindObjectOfType<Golem>().ownCrystals[0].GetComponent<EnemyPartHit>().active = true;
            GameObject.FindObjectOfType<Golem>().standUp();
            Destroy(this.gameObject);
        }
    }
}
