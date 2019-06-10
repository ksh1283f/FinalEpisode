using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TestScript : MonoBehaviour
{
    EventTrigger trigger;


    void Start()
    {
        // trigger = GetComponent<EventTrigger>();
        // trigger.OnPointerDown(PointerEventData)
    }
    public void OnClick()
    {
        Debug.Log("Test");
    }

    

}
