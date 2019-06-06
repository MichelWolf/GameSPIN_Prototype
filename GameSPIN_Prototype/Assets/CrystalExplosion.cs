using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalExplosion : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //int random = Random.Range(0, FindObjectOfType<Golem>().ownCrystals.Length);
        int random = 0;
        FindObjectOfType<Golem>().ownCrystals[random].GetComponent<SkinnedMeshRenderer>().material.EnableKeyword("_EMISSION");
        FindObjectOfType<Golem>().ownCrystals[random].GetComponent<EnemyPartHit>().active = true;
        Destroy(this.gameObject, this.gameObject.GetComponent<Animator>().GetCurrentAnimatorClipInfo(0).Length);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
