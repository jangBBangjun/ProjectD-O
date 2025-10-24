using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Sub_SpawnEvent : MonoBehaviour
{
    [SerializeField] private ParticleSystem spawnParticleSystem;
    [SerializeField] private GameObject spawnObject;
    [SerializeField] private Transform spawnPoint;
    private bool isRunning = false;

    public void Update()
    {
        if (spawnParticleSystem.isPlaying == true && isRunning == false)
            StartCoroutine(Spawn());
    }
    private IEnumerator Spawn()
    {
        isRunning = true;
        yield return new WaitUntil(() => spawnParticleSystem.isPlaying == false);

        Instantiate(spawnObject, spawnPoint.position, spawnPoint.rotation);
        isRunning = false;
    }
}
