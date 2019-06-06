using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalExplosion : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        FindObjectOfType<Golem>().crystals[Random.Range(0, FindObjectOfType<Golem>().crystals.Length)].GetComponent<SkinnedMeshRenderer>().material.EnableKeyword("_EMISSION");
        Destroy(this.gameObject, this.gameObject.GetComponent<Animator>().GetCurrentAnimatorClipInfo(0).Length);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
