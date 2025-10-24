using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Sub_Event : MonoBehaviour
{
    private enum EventType { Spawn, Use }
    [SerializeField] private EventType type;

    [SerializeField] private ParticleSystem spawnParticleSystem;
    [SerializeField] private GameObject spawnObject;
    [SerializeField] private Transform spawnPoint;
    private bool isRunning = false;

    [SerializeField] private bool fireBall = false;
    public void Update()
    {
        switch (type)
        {
            case EventType.Spawn:
                if (spawnParticleSystem.isPlaying == true && isRunning == false)
                    StartCoroutine(Spawn());
                break;
            case EventType.Use:
                if (spawnParticleSystem.isPlaying == true && isRunning == false)
                    StartCoroutine(Use());
                break;
        }
        
    }
    private IEnumerator Spawn()
    {
        isRunning = true;
        yield return new WaitUntil(() => spawnParticleSystem.isPlaying == false);

        Instantiate(spawnObject, spawnPoint.position, spawnPoint.rotation);
        isRunning = false;
    }
    private IEnumerator Use()
    {
        isRunning = true;
        yield return new WaitUntil(() => spawnParticleSystem.isPlaying == false);

        spawnObject.transform.position = spawnPoint.position;
        spawnObject.transform.rotation = spawnPoint.rotation;

        if(fireBall == false)
            spawnObject.GetComponent<ParticleSystem>()?.Play();
        else
            spawnObject.GetComponent<Sub_FireBall>()?.Play();

            isRunning = false;
    }
}
