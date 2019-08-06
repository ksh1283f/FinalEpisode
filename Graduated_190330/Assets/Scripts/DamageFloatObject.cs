using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageFloatObject : MonoBehaviour, IFloatObject
{
    public Animator damageFloatAnimator { get; private set; }

    void Start()
    {
        damageFloatAnimator = GetComponent<Animator>();
    }

    public void Release()
    {
        Destroy(gameObject);
    }
}
