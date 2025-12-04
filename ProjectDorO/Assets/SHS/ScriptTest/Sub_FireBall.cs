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
                // Raycast로 groundLayer에서 바닥 탐지
                Vector3 origin = spawnPoint.position; // 조금 위에서 아래로 쏘기
                Ray ray = new Ray(origin, Vector3.down);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, 10f, groundLayer)) // groundLayer만 감지
                {
                    Vector3 spawnPos = hit.point + Vector3.up * 0.01f; // 바닥 위 0.01f 위치
                    Transform tr = Instantiate(areaPrefab, spawnPos, transform.rotation).transform;
                    tr.SetParent(areaParent);
                }
                else
                {
                    // 바닥을 감지하지 못한 경우 예외 처리
                    Debug.LogWarning("바닥을 감지하지 못했습니다.");
                }
            }

            step++;
            yield return new WaitForSeconds(spawnCoolTime);
        }

        ps.Stop();
        yield return null;
    }
}
