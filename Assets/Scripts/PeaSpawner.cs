using System.Collections;
using UnityEngine;

public class PeaSpawner : MonoBehaviour
{
    [SerializeField] GameObject pea;
    [SerializeField] int spawnCount = 50;
    [SerializeField] int batchSize = 25;
    [SerializeField] float interval = 0.2f;
    [SerializeField] float spacing = 1.5f;
    [SerializeField] int gridWidth = 5;
    [SerializeField] float jitter = 0.3f;

    private void Start() { StartCoroutine(SpawnPeas()); }

    IEnumerator SpawnPeas()
    {
        int spawned = 0;

        while (spawned < spawnCount)
        {
            for (int i = 0; i < batchSize; i++)
            {
                int x = i % gridWidth;
                int z = i / gridWidth;

                Vector3 gridPos = transform.position + new Vector3(x * spacing, 0, z * spacing);
                Vector3 randomOffset = new Vector3(Random.Range(-jitter, jitter),Random.Range(0f, jitter),Random.Range(-jitter, jitter));
                Vector3 spawnPos = gridPos + randomOffset;
                GameObject obj = Instantiate(pea, spawnPos, Quaternion.identity);
                obj.transform.parent = transform;
                spawned++;
            }

            yield return new WaitForSeconds(interval);
        }
    }
}
