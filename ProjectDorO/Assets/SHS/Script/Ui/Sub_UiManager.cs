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
    [Serializable]
    private class SkillSprites
    {
        public Sprite skill_1;
        public Sprite skill_2;
        public Sprite skill_3;
    }

    [Header("Tag")]
    [SerializeField] private SelectImage[] tagImages;

    [Header("Skill")]
    [SerializeField] private SelectImage[] skillImages;
    [SerializeField] private SkillSprites[] skillSprites;


    [Header("HP/MP Bar")]
    [SerializeField] private Sub_WorldHp worldHp;
    [SerializeField] private Sub_AnimBar mainHpBar;
    [SerializeField] private Sub_AnimBar[] playerHpBars = new Sub_AnimBar[5];

    public void SetSelectPlayer(int index)
    {
        skillImages[0].LayOutImage.transform.GetChild(0).GetComponent<Image>().sprite = skillSprites[index].skill_1;
        skillImages[1].LayOutImage.transform.GetChild(0).GetComponent<Image>().sprite = skillSprites[index].skill_2;
        skillImages[2].LayOutImage.transform.GetChild(0).GetComponent<Image>().sprite = skillSprites[index].skill_3;

        skillImages[0].OriginImage.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = skillSprites[index].skill_1;
        skillImages[1].OriginImage.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = skillSprites[index].skill_2;
        skillImages[2].OriginImage.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = skillSprites[index].skill_3;

        skillImages[0].ToggleImage.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = skillSprites[index].skill_1;
        skillImages[1].ToggleImage.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = skillSprites[index].skill_2;
        skillImages[2].ToggleImage.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = skillSprites[index].skill_3;

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

        worldHp.SetTarget(index);
    }

    public void SetSkill(int index, float cooltime, float maxCoolTime)
    {
        if (cooltime > 0f)
        {
            skillImages[index].OriginImage.gameObject.SetActive(false);
            skillImages[index].ToggleImage.gameObject.SetActive(true);
            skillImages[index].LayOutImage.gameObject.SetActive(true);
            skillImages[index].LayOutImage.fillAmount = 1 - (cooltime / maxCoolTime);
        }
        else
        {
            skillImages[index].OriginImage.gameObject.SetActive(true);
            skillImages[index].ToggleImage.gameObject.SetActive(false);
            skillImages[index].LayOutImage.gameObject.SetActive(false);
        }
    }
    public void SetHpBar(int hp, int maxHp)
    {
        mainHpBar.SetBar(hp, maxHp);
    }
    public void SetHpBar(int index, int hp, int maxHp)
    {
        playerHpBars[index].SetBar(hp, maxHp);
    }
}