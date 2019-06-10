using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpArrow : MonoBehaviour
{
    public List<Vector3> jumpPositionList = new List<Vector3>();
    int count = 0;
    int frameCount = 0;

    // Use this for initialization
    void Start ()
    {
	}
    IEnumerator frameChecker()
    {
        while(true)
        {
            Debug.Log(frameCount);
            frameCount = 0;
            yield return new WaitForSeconds(10);
        }
    }

	// Update is called once per frame
	void Update ()
    {


    }
    private void OnPostRender()
    {

    }
    private void OnGUI()
    {
        
    }
    private void FixedUpdate()
    {
        frameCount++;
    }
}
