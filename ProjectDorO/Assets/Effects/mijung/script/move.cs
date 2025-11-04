using UnityEngine;

public class FireballMovement : MonoBehaviour
{
    public float moveSpeed = 10f; // 이동 속도 설정
    private float lifeTime = 5f; // 화염구가 스스로 파괴될 시간 (옵션)
    private float timer;

    void Start()
    {
        // 화염구가 생성된 방향(앞 방향)으로 이미 회전되어 있다고 가정
        timer = lifeTime;
    }

    void Update()
    {
        // 1. 앞으로 이동
        // transform.forward는 현재 게임 오브젝트의 Z축 방향(앞)을 나타냅니다.
        transform.position += transform.forward * moveSpeed * Time.deltaTime;

        // 2. 수명 체크 (옵션: 너무 오래 남아있지 않도록)
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            Destroy(gameObject);
        }
    }
}