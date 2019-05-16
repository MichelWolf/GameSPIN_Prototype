using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOverTime : MonoBehaviour
{
    // Start is called before the first frame update
    public float timeToDestroy = 0.5f;
    void Start()
    {
        Destroy(this.gameObject, timeToDestroy);
    }

}
