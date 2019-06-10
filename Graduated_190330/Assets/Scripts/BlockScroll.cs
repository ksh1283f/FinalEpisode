using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class BlockScroll : MonoBehaviour
{
}
public partial class BlockScroll : MonoBehaviour
{
    public SpriteRenderer spritePrefab;
    
    [Range(3,10)]
    public int blockCount;
    float blockWidth;
    float viewZoneMargin;

    public Transform followTarget;
    float lastTargetPosiotionX;

    List<Transform> blockList = new List<Transform>();
    float blockHeight;
    int leftIndex;
    int rightIndex;

    public float parallaxSpeed;

    public bool scrolling;
    public bool paralaxing;
}
public partial class BlockScroll : MonoBehaviour
{
    private void Start()
    {
        int pivot = blockCount / 2;
        blockWidth = spritePrefab.bounds.size.x;
        blockHeight = transform.position.y;

        for (int index = 0; index < blockCount; index++)
        {
            blockList.Add(Instantiate(spritePrefab, new Vector3( blockWidth*((-blockCount/2)+index), blockHeight, 0) ,transform.rotation, transform).transform);
        }

        viewZoneMargin = blockWidth / 2;
        leftIndex = 0;
        rightIndex = blockCount-1;

        lastTargetPosiotionX = followTarget.position.x;


        

    }
    private void Update()
    {
        if (paralaxing)
        {
            float deltaX = followTarget.position.x - lastTargetPosiotionX;
            transform.position += Vector3.right * (deltaX * parallaxSpeed);
        }
        lastTargetPosiotionX = followTarget.position.x;

        if (scrolling)
        {
            if (followTarget.position.x < (blockList[leftIndex].transform.position.x + viewZoneMargin))
            {
                ScrollLeft();
            }
            if (followTarget.position.x > (blockList[rightIndex].transform.position.x - viewZoneMargin))
            {
                ScrollRight();
            }
        }

    }

    void ScrollLeft()
    {
        int lastRight = rightIndex;
        blockList[rightIndex].position = Vector3.right * (blockList[leftIndex].position.x - blockWidth) + new Vector3(0, blockHeight);
        leftIndex = rightIndex;
        rightIndex--;
        if (rightIndex < 0)
        {
            rightIndex = blockList.Count - 1;
        }
    }
    void ScrollRight()
    {
        int lastLeft = leftIndex;
        blockList[leftIndex].position = Vector3.right * (blockList[rightIndex].position.x + blockWidth) + new Vector3(0, blockHeight);
        rightIndex = leftIndex;
        leftIndex++;
        if (leftIndex == blockList.Count)
        {
            leftIndex = 0;
        }
    }
}
