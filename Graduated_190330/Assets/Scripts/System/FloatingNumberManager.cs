using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public  class FloatingNumberManager : MonoBehaviour
{
    public static FloatingObject floatingObject;
    public static Canvas floatingArea;
    public Canvas canvas;
    public static FloatingNumberManager instance = null;
    public static FloatingNumberManager Instance
    {
        get
        {
            if(instance)
            {
                return instance;
            }
            else
            {
                instance = FindObjectOfType<FloatingNumberManager>();
                if (!instance)
                {
                    GameObject container = new GameObject();
                    container.name = "FloatingNumberManager";
                    instance = container.AddComponent<FloatingNumberManager>();
                }
                return instance;
            }
        }
    }

    private void Awake()
    {
        if(!instance)
        {
            instance = this;
        }
        else if(instance !=this)
        {
            Destroy(gameObject);
        }
        floatingObject = Resources.Load<FloatingObject>("Prefabs/System/FloatingSystem/FloatingObject");
        foreach(FloatingObject gamaobject in FindObjectsOfType<FloatingObject>())
        {
            if(gamaobject.name== "FloatingArea")
            {
                floatingArea = gamaobject.GetComponent<Canvas>();
            }
        }
        if(!floatingArea)
        {
            floatingArea = canvas;
        }
    }
    private void Start()
    {
    }
    public static void FloatingNumber(GameObject target,float number,E_FloatingType floatingType)
    {
        FloatingObject floatingGameObject = Instantiate<FloatingObject>(floatingObject);

        Animator floatingAnimator = floatingGameObject.floatingAnimator;
        Animator floatingAnimatorChildren = floatingGameObject.damageFloatingObjectAnimator;



        floatingGameObject.transform.SetParent(floatingArea.transform);
        Text floatingText = floatingGameObject.GetComponentInChildren<Text>();
        floatingText.text = ((int)number).ToString();
        // floatingGameObject.transform.position = target.transform.position + new Vector3(Random.value-0.5f,1.2f + Random.value, 0);
        floatingGameObject.transform.position = target.transform.position + new Vector3(Random.value-0.5f,1.5f+Random.value, 0f);
        // todo 체력바도 이것처럼 ui로 빼서 작업

        switch(floatingType)
        {
            case E_FloatingType.NonpenetratingDamage:
                {
                    floatingAnimator.Play("DamageFloating");
                    floatingAnimatorChildren.Play("NonPenetrationDamage");
                    break;
                }
            case E_FloatingType.FullPenetrationDamage:
                {
                    floatingAnimator.Play("DamageFloating");
                    floatingAnimatorChildren.Play("FullPenetrationDamage");
                    break;
                }
            case E_FloatingType.CriticalDamage:
                {
                    floatingAnimator.Play("CriticalFloating");
                    floatingAnimatorChildren.Play("CriticalDamage");
                    break;
                }
            case E_FloatingType.Heal:
                {
                    floatingAnimator.Play("HealFloating");
                    floatingAnimatorChildren.Play("Healing");
                    break;
                }
            default:
                {
                    break;
                }
        }
    }
}
