using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestController : MonoBehaviour
{
    public Vector3 startPosition = new Vector3(0, -1.956f, 0);
    public Vector3 targetPosition = new Vector3(0, -1.956f, 0);
    public float moveSpeed = 1;
    public float targetErrorRange = 0;
    public Animator animator;
    public string motionName;
    float backScale;

    bool isEnd = false;
    bool isBack = false;


    private void Start()
    {
        backScale = transform.localScale.x;
        transform.position = startPosition;
        animator = GetComponent<Animator>();
    }
    // Update is called once per frame
    void Update () {
        if(!isEnd)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * moveSpeed);
        }
        if(transform.position.x<= targetErrorRange && (!isEnd))
        {
            isEnd = true;
            animator.Play(motionName, 0,0.01f);
        }


        if (isEnd&&animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            transform.localScale = new Vector3(-backScale, transform.localScale.y, transform.localScale.z);
            isBack = true;
            GetComponent<AnimationController>().SetFaceEmotion(E_motion.Groggy);
            moveSpeed = 6;
        }
        if(isBack&& animator.GetCurrentAnimatorStateInfo(0).IsName("Run"))
        {
            transform.position = Vector3.MoveTowards(transform.position, startPosition, Time.deltaTime * moveSpeed);
        }
		
	}
}
