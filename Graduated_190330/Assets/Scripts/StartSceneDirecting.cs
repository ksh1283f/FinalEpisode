using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartSceneDirecting : MonoBehaviour
{
    private const float maxFade = 1f;
    private const float minFade = 0f;

    private const float fadeDuration = 3f;

    [SerializeField] GameObject title;
    [SerializeField] GameObject background;
    [SerializeField] GameObject titleEndPos;

    SpriteRenderer backgroundImage;
    SpriteRenderer titleImage;

    StartUI startUI;

    float startTime = 0f;

    IEnumerator Start()
    {
        DontDestroyOnLoad(this);
        if (background != null)
            backgroundImage = background.GetComponent<SpriteRenderer>();

        if (title != null)
            titleImage = title.GetComponent<SpriteRenderer>();

        startUI = UIManager.Instance.LoadUI(E_UIType.Start) as StartUI;
        startUI.OnClickedBtnStart = OnClickedStart;
        startUI.OnClickedBtnOption = OnClickedOption;
        startUI.OnClickedBtnExit = OnClickedExit;

        // 백그라운드 이미지 페이드인
        Color color = backgroundImage.color;
        startTime = 0f;
        color.a = Mathf.Lerp(minFade, maxFade, startTime);
        while (color.a < 1f)
        {
            startTime += Time.deltaTime / fadeDuration;
            color.a = Mathf.Lerp(minFade, maxFade, startTime);
            backgroundImage.color = color;

            yield return null;
        }

        // 타이틀이미지 내려오도록
        Vector3 titlePos = title.transform.position;
        startTime = 0f;
        while (titlePos.y > titleEndPos.transform.position.y)
        {
            startTime += Time.deltaTime / fadeDuration;
            titlePos.y = Mathf.Lerp(titlePos.y, titleEndPos.transform.position.y, startTime);
            title.transform.position = titlePos;

            yield return null;
        }

        Debug.Log("show Title");

        // 버튼 나오도록
        startUI.ShowBtns();
    }

    void OnClickedStart()
    {
        //SceneManager.LoadScene("Battle");

        // 씬이 바뀔땐 필히 close를 해주도록한다.
        //startUI.Close();
        //SceneManager.LoadScene("Lobby");
        //UserManager.Instance.UserSituation = E_UserSituation.LodingLobby;
        UserInfo userInfo = SaveDataManager.Instance.ReadUserInfoData();
        if(userInfo == null)    // 유저데이터가 없는 경우, 즉 처음 접속한 경우
        {
            StartCoroutine(OpeningCutSceneLoading());
            return;
        }

        StartCoroutine(LobbySceneLoad());
    }
    IEnumerator OpeningCutSceneLoading()
    {
        startUI.Close();
        UserManager.Instance.ao  = SceneManager.LoadSceneAsync("OpeningScene");
        while (!UserManager.Instance.ao.isDone)
            yield return null;
    }

    IEnumerator LobbySceneLoad()
    {
        startUI.Close();
        UserManager.Instance.ao  = SceneManager.LoadSceneAsync("NewLobby");
        while (!UserManager.Instance.ao.isDone)
            yield return null;

        UserManager.Instance.UserSituation = E_UserSituation.LoadingLobby;
        Destroy(gameObject);
    }

    void OnClickedOption()
    {

    }

    void OnClickedExit()
    {
        Application.Quit();
    }
}