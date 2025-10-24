using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class Sub_FireBall : MonoBehaviour
{
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private GameObject areaPrefab;
    private ParticleSystem ps;

    [SerializeField] private LayerMask passLayer;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform areaParent;
    [SerializeField] private float spawnCoolTime = 1f;
    [SerializeField] private float speed;
    [SerializeField] private int maxStep = 20;

    private void Awake()
    {
        ps = GetComponent<ParticleSystem>();
    }
    public void Play()
    {
        ps.Play();
        StopAllCoroutines();
        StartCoroutine(Move());
        StartCoroutine(AreaSpawn());
    }
    private IEnumerator Move()
    {
        while (true)
        {
            transform.Translate(Vector3.forward * speed * Time.deltaTime);

            yield return null;
        }
    }
    private IEnumerator AreaSpawn()
    {
        int step = 0;

        while (step <= maxStep)
        {
            if (areaParent.childCount > step && areaParent.GetChild(step) != null && areaParent.GetChild(step).gameObject.activeSelf == false)
            {
                areaParent.GetChild(step).gameObject.SetActive(true);
            }
            else
            {
                // Raycast�� groundLayer���� �ٴ� Ž��
                Vector3 origin = spawnPoint.position; // ���� ������ �Ʒ��� ���
                Ray ray = new Ray(origin, Vector3.down);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, 10f, groundLayer)) // groundLayer�� ����
                {
                    Vector3 spawnPos = hit.point + Vector3.up * 0.01f; // �ٴ� �� 0.01f ��ġ
                    Transform tr = Instantiate(areaPrefab, spawnPos, transform.rotation).transform;
                    tr.SetParent(areaParent);
                }
                else
                {
                    // �ٴ��� �������� ���� ��� ���� ó��
                    Debug.LogWarning("�ٴ��� �������� ���߽��ϴ�.");
                }
            }

            step++;
            yield return new WaitForSeconds(spawnCoolTime);
        }

        ps.Stop();
        yield return null;
    }
}
