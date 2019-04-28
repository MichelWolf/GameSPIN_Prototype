using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtkPulsate : MonoBehaviour
{
	public Material matA;
	float min, max,step;
	private Color clr;
    // Start is called before the first frame update
    void Start()
    {
        clr.a = 0.8f;
		min = 0.2f;
		max = 0.8f;
		step = 0.1f;
		clr = matA.color;
    }

    // Update is called once per frame
    void Update()
    {
        if(clr.a >= max){
			step = -step;	
		}
		if(clr.a <= min){
			step = -step;
		}
		clr.a += step;
    }
}
