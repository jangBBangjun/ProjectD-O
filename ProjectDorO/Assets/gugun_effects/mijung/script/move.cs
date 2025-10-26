using UnityEngine;

public class FireballMovement : MonoBehaviour
{
    public float moveSpeed = 10f; // �̵� �ӵ� ����
    private float lifeTime = 5f; // ȭ������ ������ �ı��� �ð� (�ɼ�)
    private float timer;

    void Start()
    {
        // ȭ������ ������ ����(�� ����)���� �̹� ȸ���Ǿ� �ִٰ� ����
        timer = lifeTime;
    }

    void Update()
    {
        // 1. ������ �̵�
        // transform.forward�� ���� ���� ������Ʈ�� Z�� ����(��)�� ��Ÿ���ϴ�.
        transform.position += transform.forward * moveSpeed * Time.deltaTime;

        // 2. ���� üũ (�ɼ�: �ʹ� ���� �������� �ʵ���)
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            Destroy(gameObject);
        }
    }
}