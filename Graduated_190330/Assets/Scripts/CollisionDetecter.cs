using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDetecter : MonoBehaviour
{

	// Use this for initialization
	void Start ()
    {
        foreach(Collider col in Physics.OverlapSphere(transform.position,5))
        {
            Debug.Log(col.name);
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
