using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public static CameraController instance;
    public static GameObject backGroundInstance;
    public GameObject backGround;
    public float backGroundMoveSpeed=0.1f;

    float deltaX;

    public float minX;
    public float maxX;
	// Use this for initialization
	void Start ()
    {
        deltaX = transform.position.x;
        if (instance==null)
        {
            instance = this;
        }
        if(backGroundInstance==null)
        {
            backGroundInstance = backGround;
        }
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (transform.position.x < minX)
        {
            if (InteractableObject.scrollCoroutine != null)
            {
                InteractableObject.coroutineObject.StopCoroutine(InteractableObject.scrollCoroutine);
            }
            transform.position = new Vector3(minX, transform.position.y);
        }
        else if (transform.position.x > maxX)
        {
            if (InteractableObject.scrollCoroutine != null)
            {
                InteractableObject.coroutineObject.StopCoroutine(InteractableObject.scrollCoroutine);
            }
            transform.position = new Vector3(maxX, transform.position.y);
        }

        deltaX = transform.position.x - deltaX;

        backGround.transform.position += new Vector3(deltaX* backGroundMoveSpeed, 0, 0);

        deltaX = transform.position.x;

    }
}
