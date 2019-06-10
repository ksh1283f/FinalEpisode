using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    public float minX = -7.7f;
    public float maxX = 2.4f;
    public float dragSpeed = 1;
    public float velocity;
    public float smooth;
    public float clickCancelDistance = 2;

    bool canClick = false;
    float clickCancelCount = 0;

    public GameObject canvas;

    /*** 이 클래스를 좀 더 범용적으로 사용하려면 수정이 필요 ***/
    public LobbyContents lobbyContents;
    /***********************************************************/

    public static InteractableObject coroutineObject;
    public static Coroutine scrollCoroutine;
    public static Coroutine forcusingCoroutine;
    public SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        lobbyContents = GetComponent<LobbyContents>();
    }

    private void OnMouseDrag()
    {
        //if (!canvas.activeInHierarchy)
        if (UIManager.Instance.openedUiDic.Count == 1)
        {
            float x = Input.GetAxis("Mouse X");
            clickCancelCount += x;
            velocity = x;
            CameraController.instance.transform.position += new Vector3(x * dragSpeed, 0, 0);
            if (clickCancelCount > clickCancelDistance || clickCancelCount < -clickCancelDistance)
            {
                canClick = false;
            }
        }
    }
    private void OnMouseDown()
    {
        //if (!canvas.activeInHierarchy)
        if (UIManager.Instance.openedUiDic.Count == 1)
        {
            spriteRenderer.color = new Color32(150, 150, 150, 255);
            canClick = true;
            clickCancelCount = 0;

            if (scrollCoroutine != null && coroutineObject != null)
            {
                coroutineObject.StopCoroutine(scrollCoroutine);
            }
            if (forcusingCoroutine != null && coroutineObject != null)
            {
                coroutineObject.StopCoroutine(forcusingCoroutine);
            }
        }
    }

    private void OnMouseUp()
    {
        //if (!canvas.activeInHierarchy)
        // 1인 이유는 해당 ui가 켜져있을때는 드래그가 안되도록 하기 위함임
        if (UIManager.Instance.openedUiDic.Count == 1)
        {
            spriteRenderer.color = new Color32(255, 255, 255, 255);
            if (canClick)
                forcusingCoroutine = StartCoroutine(Forcusing());

            if (scrollCoroutine != null && coroutineObject != null)
            {
                coroutineObject.StopCoroutine(scrollCoroutine);
            }
            coroutineObject = this;
            scrollCoroutine = StartCoroutine(SmoothMoving());
            if (lobbyContents == null)
                return;

            lobbyContents.OnExecuteContets.Execute(lobbyContents.contentsType);
        }
    }

    IEnumerator Forcusing()
    {
        while (true)
        {
            CameraController.instance.transform.position = Vector3.Lerp(CameraController.instance.transform.position, new Vector3(transform.position.x, CameraController.instance.transform.position.y, CameraController.instance.transform.position.z), 0.1f);
            if (CameraController.instance.transform.position.x < minX)
            {
                CameraController.instance.transform.position = new Vector3(minX, CameraController.instance.transform.position.y, CameraController.instance.transform.position.z);
                break;
            }
            if (CameraController.instance.transform.position.x > maxX)
            {
                CameraController.instance.transform.position = new Vector3(maxX, CameraController.instance.transform.position.y, CameraController.instance.transform.position.z);
                break;
            }
            if (CameraController.instance.transform.position.x - transform.position.x < 0.01f && CameraController.instance.transform.position.x - transform.position.x > -0.01f)
            {
                break;
            }
            yield return null;
        }
    }

    IEnumerator SmoothMoving()
    {
        while (true)
        {
            CameraController.instance.transform.position += new Vector3(velocity * dragSpeed, 0, 0);
            velocity *= smooth;
            if (velocity > 0.01f || velocity < -0.01f)
            {
            }
            else
            {
                break;
            }
            yield return null;
        }
    }
}
