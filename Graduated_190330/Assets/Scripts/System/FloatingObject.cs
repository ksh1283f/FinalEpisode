using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingObject : MonoBehaviour
{
    public Animator floatingAnimator;
    public Animator damageFloatingObjectAnimator;
    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}
