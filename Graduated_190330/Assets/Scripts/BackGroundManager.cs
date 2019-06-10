using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class BackGroundManager : MonoBehaviour
{
    static BackGroundManager instance;
    public static BackGroundManager Instance
    {
        get
        {
            if (!instance)
            {
                instance = FindObjectOfType<BackGroundManager>();
                if (!instance)
                {
                    GameObject container = new GameObject();
                    container.name = "BackGroundManager";
                    instance = container.AddComponent<BackGroundManager>();
                }
            }
            return instance;
        }
    }
}
public partial class BackGroundManager : MonoBehaviour
{
    public bool scrolling;
    public bool paralaxing;

    public float backgroundSize;

    public Transform followTarget;
    public Transform[] grounds;
    public float viewZone;
    float groundHeight;
    int leftIndex;
    int rightIndex;

    public float paralaxSpeed;
    float lastCameraX;


    public List<SpriteRenderer> groundList = new List<SpriteRenderer>();

    public SpriteRenderer groundObject;
    public SpriteRenderer backGroundObject;
    public SpriteRenderer farBackGroundObject;

    public Sprite groundSprite;
    public Sprite backGroundSprite;
    public Sprite farBackGroundSprite;

    public Vector3 offset;



    /*
     * -자원 종류-
     * 
     * 뒷배경
     * 움직이는 배경
     * 바닥
     */

    /*
     * -기능-
     * 이름을 기준으로 자원을 불러온다.
     * 
     * 
     * 캐릭터가 전진시 바닥이 모자르면 하나를 더 놓는다.
     * 
     * 캐릭터가 이동시 배경을 움직인다.
     */



    public void LoadStageAsset(string stageName)
    {
        groundSprite = Resources.Load<Sprite>("Stage/Sprites/" + stageName + "/Ground");
        backGroundSprite = Resources.Load<Sprite>("Stage/Sprites/" + stageName + "/BackGround");
        farBackGroundSprite = Resources.Load<Sprite>("Stage/Sprites/" + stageName + "/FarBackGround");
        groundList.Add(Instantiate(groundObject));
        groundList.Add(Instantiate(groundObject));
        groundList.Add(Instantiate(groundObject));

        foreach (SpriteRenderer ground in groundList)
        {
            ground.sprite = groundSprite;
        }
    }

}
public partial class BackGroundManager : MonoBehaviour
{
    private void Start()
    {
        if(!instance)
        {
            instance = this;
        }
        //LoadStageAsset("Winter");
        backgroundSize = (float)grounds[0].GetComponent<SpriteRenderer>().bounds.size.x;
        leftIndex = 0;
        rightIndex = grounds.Length - 1;
        groundHeight = grounds[0].transform.position.y;

        lastCameraX = followTarget.position.x;

    }
    private void Update()
    {
        if(paralaxing)
        {
            float deltaX = followTarget.position.x - lastCameraX;
            transform.position += Vector3.right * (deltaX * paralaxSpeed);
        }
        lastCameraX = followTarget.position.x;

        if(scrolling)
        {
            if (followTarget.position.x < (grounds[leftIndex].transform.position.x + viewZone))
            {
                ScrollLeft();
            }
            if (followTarget.position.x > (grounds[rightIndex].transform.position.x - viewZone))
            {
                ScrollRight();
            }
        }
        
    }

    void ScrollLeft()
    {
        int lastRight = rightIndex;
        grounds[rightIndex].position = Vector3.right * (grounds[leftIndex].position.x - backgroundSize) + new Vector3(0,groundHeight);
        leftIndex = rightIndex;
        rightIndex--;
        if(rightIndex<0)
        {
            rightIndex = grounds.Length - 1;
        }
    }
    void ScrollRight()
    {
        int lastLeft = leftIndex;
        grounds[leftIndex].position = Vector3.right * (grounds[rightIndex].position.x + backgroundSize) + new Vector3(0, groundHeight);
        rightIndex = leftIndex;
        leftIndex++;
        if (leftIndex == grounds.Length)
        {
            leftIndex = 0;
        }
    }


    public void StartFollowing()
    {
        StartCoroutine(Following());
    }
    IEnumerator Following()
    {
        while(true)
        {
            //6만큼 거리가 이동되면 뒤의 이미지를 앞으로 전진배치
            yield return null;
        }
    }
}
