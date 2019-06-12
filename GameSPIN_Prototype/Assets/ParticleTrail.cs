using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleTrail : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject golem;
    public GameObject particle;
    public float speed;
    private Transform target;
    private GameObject particleInst;

    bool toGolem = false;

    void Start()
    {
        target = transform;
        particleInst = null;
    }

    // Update is called once per frame
    void Update()
    {
        if (particleInst != null)
        {
            float step = speed * Time.deltaTime;
            particleInst.transform.position = Vector3.MoveTowards(particleInst.transform.position, target.position, step);

            if (Vector3.Distance(particleInst.transform.position, target.position) < 0.001f)
            {
                if (toGolem)
                {
                    target = transform;
                    toGolem = !toGolem;
                }
                else
                {
                    target = golem.transform;
                    toGolem = !toGolem;
                }
            }
        }
    }

   public void activateParticleTrail()
    {
        if(particleInst != null)
        {
            Destroy(particleInst);
        }
        particleInst = Instantiate(particle, golem.transform.position, Quaternion.identity);
    }

  
}
