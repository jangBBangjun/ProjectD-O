using System;
using UnityEngine;
using UnityEngine.UI;


// UI 관리하는 기능을 실행하는 메니저 클래스
public class Sub_UiManager : MonoBehaviour
{
    [Serializable]
    private class SelectImage
    {
        public Image OriginImage;
        public Image ToggleImage;
        public Image LayOutImage = null;
    }

    [Header("Tag")]
    [SerializeField] private SelectImage[] tagImages;

    [Header("Skill")]
    [SerializeField] private SelectImage[] skillImages;

    [Header("HP/MP Bar")]
    [SerializeField] private Sub_WorldHpPool worldHpPool;
    [SerializeField] private Sub_AnimBar mainHpBar;
    [SerializeField] private Sub_AnimBar[] playerHpBars = new Sub_AnimBar[5];

    public void SetSelectPlayer(int index)
    {
        for (int i = 0; i < tagImages.Length; ++i)
        {
            if (index != i)
            {
                tagImages[i].OriginImage.gameObject.SetActive(true);
                tagImages[i].ToggleImage.gameObject.SetActive(false);
            }
            else
            {
                tagImages[i].OriginImage.gameObject.SetActive(false);
                tagImages[i].ToggleImage.gameObject.SetActive(true);
            }
        }

        worldHpPool.SetTarget(index);
    }

    public void SetSkill(int index, float cooltime, float maxCoolTime)
    {
        if (cooltime > 0f)
        {
            skillImages[index].OriginImage.gameObject.SetActive(false);
            skillImages[index].ToggleImage.gameObject.SetActive(true);
            skillImages[index].LayOutImage.gameObject.SetActive(true);
            skillImages[index].LayOutImage.fillAmount = cooltime / maxCoolTime;
        }
        else
        {
            skillImages[index].OriginImage.gameObject.SetActive(true);
            skillImages[index].ToggleImage.gameObject.SetActive(false);
            skillImages[index].LayOutImage.gameObject.SetActive(false);
        }
    }
    public void SetHpBar(int changeHp)
    {
        mainHpBar.SetBar(changeHp);
    }
    public void SetHpBar(int index, int changeHp)
    {
        playerHpBars[index].SetBar(changeHp);
    }
}