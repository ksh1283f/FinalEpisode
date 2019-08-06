using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatternResultObject : MonoBehaviour, IFloatObject
{
    public Animator patternResultAnimator { get; private set; }

    void Start()
    {
        patternResultAnimator = GetComponent<Animator>();
    }

    public void Release()
    {
        Destroy(gameObject);
    }
}
