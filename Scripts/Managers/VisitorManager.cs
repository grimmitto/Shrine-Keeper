using UnityEngine;

public class VisitorManager : MonoBehaviour
{
    public static VisitorManager Instance;

    public GameObject visitorPrefab;
    public Transform[] spawnPoints;

    private int visitorLimit = 3;

    private void Awake()
    {
        Instance = this;
    }

    public void SetDailyVisitorLimit(int amount)
    {
        visitorLimit = amount;
    }

    public void SpawnVisitorsForDay()
    {
        int count = Mathf.Min(visitorLimit, spawnPoints.Length);

        for (int i = 0; i < count; i++)
        {
            Instantiate(visitorPrefab, spawnPoints[i].position, Quaternion.identity);
        }
    }
}
