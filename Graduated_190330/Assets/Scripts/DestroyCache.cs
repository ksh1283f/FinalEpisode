using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyCache : MonoBehaviour
{
	void Start ()
    {
        PlayerPrefs.DeleteAll();
	}
}
