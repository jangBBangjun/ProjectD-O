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

public class CharacterManager : MonoBehaviour
{
    [SerializeField] MiniMapManager miniMapManager;

    [Serializable]
    private class UnitPrefab
    {
        public UnitType unitType; // ���� Ÿ��
        public GameObject prefab; // ������
    }
    [Serializable]
    private class Unit
    {
        public UnitType unitType; // ���� Ÿ��
        public GameObject gameObject; // ���� ������Ʈ
    }

    [SerializeField] private UnitPrefab[] unitPrefabs; // ���� ������ �迭
    [SerializeField] private Transform unitParent; // ���� �θ� ������Ʈ
    [SerializeField] private List<Unit> unitList = new List<Unit>(); // ���� ����Ʈ

    [Serializable]
    public class PlayerObj
    {
        public int playerCode; // �÷��̾� �ڵ�
        public GameObject gameObject; // �÷��̾� ������Ʈ
    }

    [SerializeField] private List<PlayerObj> playerList = new List<PlayerObj>(); // �÷��̾� ����Ʈ

    public void PlayerBatch(int playerCount, Vector3 position)
    {
        if (GroundRayCast(position) == false)
            return;

        GameObject target = playerList[playerCount].gameObject;
        target.transform.position = position;

        miniMapManager.AddTarget(IconType.Player, target.transform);
        target.SetActive(true);
    }
    public Transform GetPlayer(int num)
    {
        if(num < playerList.Count)
        {
            if(playerList[num].gameObject.activeSelf == false)
            {
                Debug.LogWarning("�ش� �÷��̾�� ��Ȱ�� �����Դϴ�: " + num);
                return null;
            }

            return playerList[num].gameObject.transform;
        }

        Debug.LogWarning("�ش� �÷��̾� ��ȣ�� �����ϴ�: " + num);
        return null;
    }
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

        Debug.LogWarning("�ش� ���� Ÿ���� �������� �����ϴ�: " + unitType);
    }
    private bool GroundRayCast(Vector3 position)
    {
        RaycastHit hit;
        if (Physics.Raycast(position + Vector3.up * 5f, Vector3.down, out hit, 10f))
        {
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
            {
                return true;
            }
        }

        Debug.LogWarning("���� ���� ��ġ�� ����� ������� �ʽ��ϴ�: " + position);
        return false;
    }
}
