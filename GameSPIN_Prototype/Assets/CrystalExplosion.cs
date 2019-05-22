using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalExplosion : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Destroy(this.gameObject, this.gameObject.GetComponent<Animator>().GetCurrentAnimatorClipInfo(0).Length);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
