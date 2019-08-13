using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DamageFloatObject : MonoBehaviour, IFloatObject
{
    private float fadeDuration = 5f;
    private E_DamageType damageType = E_DamageType.None;
    [SerializeField] TextMeshProUGUI damageText;
    
    public void Release()
    {
        Destroy(gameObject);
    }

    public void Directing()
    {
        StartCoroutine(ShowDamageFont());
    }

    IEnumerator ShowDamageFont()
    {
        // x좌표는 일정범위에서 랜덤으로 계산
        // 현재 위치 -> 조금 위쪽 위치
        float targetYPos = damageText.transform.localPosition.y + 1f;
        float startTime = 0f;
        damageText.transform.localPosition = new Vector3(damageText.transform.localPosition.x
                                                , Mathf.Lerp(damageText.transform.localPosition.y, targetYPos, startTime)
                                                , damageText.transform.localPosition.z);
        while ((targetYPos - damageText.transform.localPosition.y) > 0f)
        {
            startTime += Time.deltaTime / fadeDuration;
            damageText.transform.localPosition = new Vector3(damageText.transform.localPosition.x
                                                , Mathf.Lerp(damageText.transform.localPosition.y, targetYPos, startTime)
                                                , damageText.transform.localPosition.z);

            yield return null;
        }
        yield return new WaitForSeconds(1f);
        Release();
    }

    public void SetData(string data)
    {
        if(damageText == null)
        {
            Debug.LogError("damageText is null");
            return;
        }

        damageText.text = data;
    }

    public void SetDamageType(E_DamageType type)
    {
        damageType = type;
    }
}
