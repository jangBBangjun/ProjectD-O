using System;
using System.Collections.Generic;
using UnityEngine;

public enum UnitType
{
    None,
    Knight,
    Shield,
    Archer
}
public class Sub_EntityManager : MonoBehaviour
{
    //------------------- 필수 참조 ------------------ //

    private MiniMapManager miniMapManager;
    private Sub_UiManager uiManager;
    [SerializeField] private TestCam CameratestCam;

    private void Awake()
    {
        miniMapManager = GetComponent<MiniMapManager>();
        uiManager = GetComponent<Sub_UiManager>();
    }

    #region [Player]-

    [Serializable]
    public class PlayerObj
    {
        public int playerCode; // 플레이어 코드
        public GameObject gameObject; // 플레이어 오브젝트
    }

    [Header("플레이어")]
    [SerializeField] int usePlayerNum = 0; // 현재 사용 중인 플레이어 번호
    [SerializeField] private List<PlayerObj> playerList = new List<PlayerObj>(); // 플레이어 리스트

    public void PlayerBatch(int playerCount, Vector3 position)
    {
        if (GroundRayCast(position) == false)
            return;

        GameObject target = playerList[playerCount].gameObject;
        target.transform.position = position;

        miniMapManager.AddTarget(IconType.Player, target.transform);
        target.SetActive(true);
    }
    public Transform PlayerGet(int num)
    {
        if (num < playerList.Count)
        {
            if (playerList[num].gameObject.activeSelf == false)
            {
                Debug.LogWarning("해당 플레이어는 비활성 상태입니다: " + num);
                return null;
            }

            return playerList[num].gameObject.transform;
        }

        Debug.LogWarning("해당 플레이어 번호가 없습니다: " + num);
        return null;
    }
    public void PlayerSeclect(int changeNum)
    {
        usePlayerNum = changeNum;

        Transform player = null;
        player = PlayerGet(usePlayerNum);

        if (player != null)
        {
            miniMapManager.SetTarget(player);
            CameratestCam.SetTarget(player);
            uiManager.SetSelectPlayer(changeNum);
        }
    }
    public void PlayerChangeHp(int index, int value)
    {
        if (index >= playerList.Count)
        {
            Debug.LogWarning("해당 플레이어 번호가 없습니다: " + index);
            return;
        }

        uiManager.SetHpBar(index, value);

        if (index == usePlayerNum)
            uiManager.SetHpBar(value);
    }
    public void PlayerSkillRender(int skillNum, float skillCooldown, float maxSkillCooldown)
    {
        uiManager.SetSkill(skillNum, skillCooldown, maxSkillCooldown);
    }

    #endregion

    #region [Unit]-

    [Serializable]
    private class UnitPrefab
    {
        public UnitType unitType; // 유닛 타입
        public GameObject prefab; // 프리팹
    }
    [Serializable]
    private class Unit
    {
        public UnitType unitType; // 유닛 타입
        public GameObject gameObject; // 유닛 오브젝트
    }

    [Header("유닛")]
    [SerializeField] private UnitPrefab[] unitPrefabs; // 유닛 프리팹 배열
    [SerializeField] private Transform unitParent; // 유닛 부모 오브젝트
    [SerializeField] private List<Unit> unitList = new List<Unit>(); // 유닛 리스트

    public void UnitSpawn(UnitType unitType, Vector3 position)
    {
        if (GroundRayCast(position) == false)
            return;

        foreach (Unit unit in unitList)
        {
            if(unit.unitType == unitType && unit.gameObject.activeSelf == false)
            {
                unit.gameObject.SetActive(true);
                unit.gameObject.transform.position = position;
                miniMapManager.AddTarget(IconType.Unit, unit.gameObject.transform);
                return;
            }
        }

        foreach (UnitPrefab unitPrefab in unitPrefabs)
        {
            if (unitPrefab.unitType == unitType)
            {
                GameObject unit = Instantiate(unitPrefab.prefab, position, Quaternion.identity, unitParent);
                miniMapManager.AddTarget(IconType.Unit, unit.gameObject.transform);
                return;
            }
        }

        Debug.LogWarning("해당 유닛 타입의 프리팹이 없습니다: " + unitType);
    }
    #endregion

    #region [Utils]-
    private bool GroundRayCast(Vector3 position)
    {
        RaycastHit hit;
        if (Physics.Raycast(position + Vector3.up * 1f, Vector3.down, out hit, 5f))
        {
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
            {
                return true;
            }
        }

        Debug.LogWarning("유닛 생성 위치가 지면과 닿아있지 않습니다: " + position);
        return false;
    }
    #endregion
}