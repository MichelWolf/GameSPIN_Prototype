using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPartHit : MonoBehaviour
{
	//0 -> no player hits part of mesh ; 1 -> player 1 hits ; 2 -> player 2 hits ; 3 -> both player hit 
	public int hitByPlayers;

    void Start()
    {
        hitByPlayers=0;
    }

}
