using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Lofle.Tween;
using TMPro;
using Graduate.Unit.Player;

public class TestCode : Singletone<Testcode>
{
    [SerializeField] Dictionary<int, string> dialogueDic = new Dictionary<int, string>();
    [SerializeField] string showText = string.Empty;
    private Text uiText;
    private Button btnText;
    [SerializeField]private int remainTextCount;
    [SerializeField]private int dialogueIndex = 0;
    [SerializeField]private int dialogueDicIndex =0;

    IEnumerator coroutine;

    public PlayerUnit Obj;

    void SetDialogDic()
    {
        dialogueDic.Add(0, "안녕하세요ㅠㅏㅓㅠㅏㅓㅠㅓㅠㅓㅏㅠㅡㅠㅡㅜㅠㅏㅗㅠㅏ");
        dialogueDic.Add(1, "nice to meet you ㄴㅇㄹㄴㅇㄹㅊㄴㅇㅊㄴㅇㄹㅈㄷㄹㄴㅇㅍㄴㅇㅍ");
        dialogueDic.Add(2, "안녕하세duㄹㄴㄹㄴㅇㅍㄴㅎㄱㄷa");
        dialogueDic.Add(3, "안dd뉸ㄱㅎㅊㅍㅈㅎㄴㅇ");
        dialogueDic.Add(4, "f세요ㅎㅎㅈ곶ㄱㅎㅈ곶곤ㄹㅎㄴㅇ");
        dialogueDic.Add(5, "안g요ㄴㅎㅎㄴㅎㄸㅎㅎㄸㅎㅎㄷㅎㄸㅎ쪼ㅗ쯎던ㅍㄴㅇㅎㄴㅎ");
        dialogueDic.Add(6, "안녕svsdfsfd요");
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            Obj.PlayerUnitState = Graduate.Unit.E_UnitState.Idle;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Obj.PlayerUnitState = Graduate.Unit.E_UnitState.Move;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {

        }
    }

    void Start()
    {
        // SetDialogDic();

        
        // uiText = GetComponent<Text>();
        // btnText = GetComponent<Button>();
        
        // if (uiText == null)
        //     return;

        // if (btnText == null)
        //     return;

        // uiText.text = string.Empty;

        // btnText.onClick.AddListener(OnClickedText);
        
    }

    void ShowDialogue(string dialogue)
    {
        remainTextCount = dialogue.Length;
        coroutine = Typing(dialogue);
        StartCoroutine(coroutine);
    }

    IEnumerator Typing(string dialogue)
    {
        if(string.IsNullOrEmpty(dialogue))
        {
            Debug.LogError("There is no dialogue");
            yield break;
        }
        uiText.text = string.Empty;
        dialogueIndex = 0;
        while (remainTextCount > 0)
        {
            // uiText.text += showText[index];
            uiText.text += dialogue[dialogueIndex];
            dialogueIndex++;
            remainTextCount--;
            yield return new WaitForSeconds(0.1f);
        }
        OnTypingEnd();
        Debug.LogError("Typing ends");
    }

    void OnTypingEnd()
    {
        if (coroutine != null)
            coroutine = null;       
    }

    void OnClickedText()
    {
        // 타이핑이 진행중인 경우
        if(coroutine != null)
        {
            Debug.LogError("OnClickedText, coroutine isn't null");
            StopCoroutine(coroutine);
            uiText.text = showText;
            coroutine = null;
            dialogueDicIndex++;
        }
        else    // 타이핑이 끝난 경우 or 진행 중이 아닌 경우
        {
            Debug.LogError("OnClickedText, coroutine is null");
            if(dialogueDicIndex>dialogueDic.Count-1)
            {
                //todo 대화가 끝난 후의 프로세스를 여기서 작업
                Debug.LogError("Showing text is end!");
                return;
            }

            string dialogue = dialogueDic[dialogueDicIndex];
            showText = dialogue;
            ShowDialogue(dialogue);
        }        
    }

    // void Update()
    // {
    //     if (Input.GetKeyDown(KeyCode.Alpha1))
    //     {
    //         FloatingNumberManager.FloatingNumber(Object, 3000, E_FloatingType.NonpenetratingDamage);
    //     }
    //     if (Input.GetKeyDown(KeyCode.Alpha2))
    //     {
    //         FloatingNumberManager.FloatingNumber(Object, 3000, E_FloatingType.FullPenetrationDamage);
    //     }
    //     if (Input.GetKeyDown(KeyCode.Alpha3))
    //     {
    //         FloatingNumberManager.FloatingNumber(Object, 3000, E_FloatingType.CriticalDamage);
    //     }
    //     if (Input.GetKeyDown(KeyCode.Alpha4))
    //     {
    //         FloatingNumberManager.FloatingNumber(Object, 3000, E_FloatingType.Heal);
    //     }
    // }

    // [SerializeField] UITweenColorAlpha tweenColorAlpha;

    // void Start()
    // {
    //     tweenColorAlpha.PlayForward();
    // }
    // public float minimum = 0.0f;
    // public float maximum = 1f;
    // public float duration = 5.0f;
    // private float startTime;
    // public SpriteRenderer sprite;
    // void Start()
    // {
    //     startTime = Time.time;
    //     startTime = Time.deltaTime;
    // }
    // void Update()
    // {
    //     float t = (Time.time - startTime) / duration;
    //     sprite.color = new Color(1f, 1f, 1f, Mathf.SmoothStep(minimum, maximum, t));
    //     Debug.LogError("smoothStep: " + Mathf.SmoothStep(minimum, maximum, t));

    //     //float startTime = Time.deltaTime;
    //     //while (!isDirectingEnd)
    //     //{
    //     //    if (backgroundImage == null)
    //     //        yield break;

    //     //    float t = (Time.deltaTime - startTime) / fadeDuration;
    //     //    alpha = Mathf.SmoothStep(minFade, maxFade, t);
    //     //    backgroundImage.color = new Color(1f, 1f, 1f, alpha);
    //     //    if (alpha >= maxFade)
    //     //        isDirectingEnd = true;
    //     //    Debug.Log("alpha: " + alpha);
    //     //    Debug.Log("t: " + t);
    //     //    yield return null;
    //     //}
    // }
}
