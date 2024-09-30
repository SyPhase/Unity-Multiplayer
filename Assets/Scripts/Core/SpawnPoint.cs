using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    static List<SpawnPoint> spawnPoints = new List<SpawnPoint>();

    float gizmoRadius = 1f;

    public static Vector3 GetRandomSpawnPos()
    {
        if (spawnPoints.Count == 0)
        {
            Debug.LogWarning("ERROR: No active SpawnPoints in scene.");
            return Vector3.zero;
        }

        return spawnPoints[Random.Range(0, spawnPoints.Count)].transform.position;
    }

    void OnEnable()
    {
        spawnPoints.Add(this);
    }

    void OnDisable()
    {
        spawnPoints.Remove(this);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(0.2f, 0.4f, 1f, 0.3f);
        Gizmos.DrawWireSphere(transform.position, gizmoRadius);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, gizmoRadius);
    }
}
