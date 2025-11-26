using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// Bar 애니메이션 기능 클래스
public class Sub_AnimBar : MonoBehaviour
{
    private Image valueImage;
    private Image animImage;

    [SerializeField] private float changeBarValueSpeed = 1f;
    [SerializeField] private float changeBarDelay = 0.2f;
    [SerializeField] private float changeBarAnimSpeed = 2f;

    public void Awake()
    {
        animImage = transform.GetChild(1).GetComponent<Image>();
        valueImage = transform.GetChild(2).GetComponent<Image>();
    }
    public void SetBar(int changeHp)
    {
        StartCoroutine(ValueBar(changeHp));
        StartCoroutine(AnimBar(changeHp));

        Debug.Log("SetBar!");
    }

    private IEnumerator ValueBar(int changeValue)
    {
        float startFill = valueImage.fillAmount;
        float targetFill = Mathf.Clamp01(startFill - (changeValue / 100f));

        float timer = 0f;

        while (!Mathf.Approximately(valueImage.fillAmount, targetFill))
        {
            timer += Time.deltaTime * changeBarValueSpeed;
            valueImage.fillAmount = Mathf.Lerp(startFill, targetFill, timer);
            yield return null;
        }
        valueImage.fillAmount = targetFill;
    }

    private IEnumerator AnimBar(int changeValue)
    {
        yield return new WaitForSeconds(changeBarDelay);

        float startFill = animImage.fillAmount;
        float targetFill = Mathf.Clamp01(startFill - (changeValue / 100f));

        float timer = 0f;

        while (!Mathf.Approximately(animImage.fillAmount, targetFill))
        {
            timer += Time.deltaTime * changeBarAnimSpeed;
            animImage.fillAmount = Mathf.Lerp(startFill, targetFill, timer);
            yield return null;
        }
        animImage.fillAmount = targetFill;
    }
}
