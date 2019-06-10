using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockGroundGenerater : MonoBehaviour {

    public int blockCount = 30;
    public float blockDistance = -0.52f;
    public GameObject blockPrefeb;

    Vector3 offset;

	// Use this for initialization
	void Start ()
    {
        offset = new Vector3(0, 0, 0);
        for(int index=0;index<blockCount;index++)
        {
            Transform tempTransform = Instantiate(blockPrefeb).transform;
            tempTransform.parent = transform;
            tempTransform.localPosition = offset;
            offset.x += blockDistance;
        }

		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
