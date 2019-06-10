using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animation))]
public class Model : MonoBehaviour
{

    private SpriteRenderer SpriteRenderer { get; set; }
    private Animation Animation { get; set; }

    void Awake()
    {
        SpriteRenderer = GetComponent<SpriteRenderer>();
        Animation = GetComponent<Animation>();
    }

    public void Play(string animationName)
    {
        Animation.Play(animationName);
    }

    public void SetNextSprite(Sprite sprite)
    {
        SpriteRenderer.sprite = sprite;
    }

    public float rotateDegree;
    public float rotateSpeed;
    public void Turning()
    {
        StartCoroutine(turn(rotateDegree));
    }

    IEnumerator turn(float degree)
    {
        float direction = -1;
        if(Random.value<0.5f)
        {
            direction = 1;
        }
        transform.Rotate(new Vector3(transform.rotation.x, transform.rotation.y, transform.rotation.z + Random.Range(-degree, degree)));
        while (true)
        {
            transform.Rotate(new Vector3(transform.rotation.x, transform.rotation.y, transform.rotation.z + (rotateSpeed * direction)));
            yield return null;
        }
    }


    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}