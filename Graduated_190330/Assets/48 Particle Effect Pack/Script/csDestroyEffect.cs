using UnityEngine;
using System.Collections;

public class csDestroyEffect : MonoBehaviour {
	
	void Start()
    {
        Invoke("DestroyThis",3f);
    }


    void DestroyThis()
    {
        Destroy(gameObject);
    }
}
