using UnityEngine;

public class TrashSpawner : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject[] trashPrefabs; // verschiedene Müllobjekte

    [Header("Spawn Settings")]
    public int spawnCount = 20; // wie viele Objekte sollen gespawnt werden
    public BoxCollider[] spawnAreas; // hier die BoxCollider reinziehen

    private void Start()
    {
        SpawnTrash();
    }

    void SpawnTrash()
    {
        if (trashPrefabs.Length == 0 || spawnAreas.Length == 0) return;

        for (int i = 0; i < spawnCount; i++)
        {
            // zufällige Zone auswählen
            BoxCollider area = spawnAreas[Random.Range(0, spawnAreas.Length)];

            // zufällige Position innerhalb der Box (Rotation berücksichtigt)
            Vector3 randomPos = GetRandomPointInBox(area);

            // Prefab wählen
            GameObject prefab = trashPrefabs[Random.Range(0, trashPrefabs.Length)];

            // spawnen
            Instantiate(prefab, randomPos, Quaternion.Euler(0, Random.Range(0, 360), 0));
        }
    }

    Vector3 GetRandomPointInBox(BoxCollider box)
    {
        // Lokale Zufallsposition relativ zum Collider
        Vector3 localPos = new Vector3(
            Random.Range(-box.size.x / 2f, box.size.x / 2f),
            Random.Range(-box.size.y / 2f, box.size.y / 2f),
            Random.Range(-box.size.z / 2f, box.size.z / 2f)
        );

        // Skaliert mit lossyScale
        localPos = Vector3.Scale(localPos, box.transform.lossyScale);

        // Mittelpunkt in Weltkoordinaten
        Vector3 worldCenter = box.transform.TransformPoint(box.center);

        // Lokale Position in Welt-Raum transformieren (Rotation wird berücksichtigt)
        return box.transform.TransformPoint(box.center + localPos);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0, 0.5f, 1, 0.25f);

        if (spawnAreas != null)
        {
            foreach (var box in spawnAreas)
            {
                if (box != null)
                {
                    Gizmos.matrix = box.transform.localToWorldMatrix;
                    Gizmos.DrawCube(box.center, box.size);
                }
            }
        }
    }
}
