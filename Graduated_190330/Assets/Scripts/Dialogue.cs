using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum E_Dialogue {
    None,
    Opening,
    FirstLobbySceneTutorial,
    BattleSceneTutorial,
    SecondLobbySceneTutorial,
}

public class Dialogue : MonoBehaviour {
    [SerializeField] E_Dialogue dialogueType;
    public Dictionary<int, string> dialogueDic { get; private set; }

    [SerializeField] private Text uiText;
    [SerializeField] private Button btnText;

    private int remainTextCount;
    private int dialogueIndex = 0;
    public int DialogueDicIndex { get; private set; }
    public bool IsTypingEnd; //{get; private set;}
    public bool IsCutSceneDirecting;

    public IEnumerator Coroutine { get; private set; }

    void Awake () {
        IsTypingEnd = false;
    }

    // 오프닝 연출
    public void SetDialogueDic () {
        dialogueDic = new Dictionary<int, string> ();
        var openingDic = GameDataManager.Instance.OpeningSceneDialogueDataDic;
        foreach (var item in openingDic)
            dialogueDic.Add (item.Key, item.Value.Dialogue);

        DialogueDicIndex = 0;
    }

    // 튜토리얼용
    public void SetDialogueDic (E_TutorialType type) {
        if (dialogueDic == null)
            dialogueDic = new Dictionary<int, string> ();

        var tutorialDialogueDic = GameDataManager.Instance.TutorialDataDic[type];
        foreach (var item in tutorialDialogueDic)
            dialogueDic.Add (item.Key, item.Value.Dialogue);

        DialogueDicIndex = 0;
    }

    public void SetDialogueDic (string dialgoue) {
        if (dialogueDic == null)
            dialogueDic = new Dictionary<int, string> ();

        dialogueDic.Clear ();
        dialogueDic.Add (0, dialgoue);

        DialogueDicIndex = 0;
    }

    void Start () {
        // SetDialogueDic();
        uiText.text = string.Empty;
        btnText.onClick.AddListener (OnClickedText);
    }

    public void ShowDialogue (string dialogue) {
        IsTypingEnd = false;
        remainTextCount = dialogue.Length;
        Coroutine = Typing (dialogue);
        StartCoroutine (Coroutine);
    }

    public void ClearText () {
        if (uiText == null) {
            Debug.LogError ("uiText is null");
            return;
        }

        uiText.text = string.Empty;
    }

    IEnumerator Typing (string dialogue) {
        if (string.IsNullOrEmpty (dialogue)) {
            Debug.LogError ("There is no diaogue");
            yield break;
        }

        uiText.text = string.Empty;
        dialogueIndex = 0;
        while (remainTextCount > 0) {
            uiText.text += dialogue[dialogueIndex];
            dialogueIndex++;
            remainTextCount--;
            yield return new WaitForSeconds (0.1f);
        }
        OnTypingEnd ();
        Debug.LogError ("Typing ends");
    }

    void OnTypingEnd () {
        if (Coroutine != null) {
            Coroutine = null;
            DialogueDicIndex++;
            IsTypingEnd = true;
        }

    }

    void OnClickedText () {
        Debug.LogError ("dialogueDicIndex: " + DialogueDicIndex);
        if (IsCutSceneDirecting)
            return;

        if (Coroutine != null) // 타이핑이 진행중인 경우
        {
            Debug.LogError ("OnClickedTextm coroutine isn't null");
            StopCoroutine (Coroutine);
            uiText.text = dialogueDic[DialogueDicIndex];
            Coroutine = null;
            DialogueDicIndex++;
            IsTypingEnd = true;
        } else // 타이핑이 끝난 경우 or 진행 중이 아닌 경우
        {
            Debug.LogError ("OnClickedText, coroutine is null");
            if (DialogueDicIndex > dialogueDic.Count - 1) {
                // todo 대화가 끝난 후의 프로세스를 여기서 작업
                Debug.LogError ("Showing text is end!");
                return;
            }

            string dialogue = dialogueDic[DialogueDicIndex];
            ShowDialogue (dialogue);
        }
    }
}