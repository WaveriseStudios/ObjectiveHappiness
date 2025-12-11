using UnityEngine;

public class BuildingSite : MonoBehaviour
{
    public BuildingType buildingType;
    public int masonsNeeded;
    public float constructionTime = 10f;

    private float constructionProgress = 0f;
    private BuildingManager buildingManager;

    public GameObject completedBuildingPrefab;

    void Start()
    {
        buildingManager = FindObjectOfType<BuildingManager>();
    }


    // Function to make the mason work on the building site
    public bool Contribute(Unit mason)
    {
        float effort = Time.deltaTime;
        constructionProgress += effort;

        if (constructionProgress >= constructionTime)
        {
            FinishBuilding();
            return true;
        }
        return false;
    }


    // Instantiate the finished building prefab
    private void FinishBuilding()
    {
        GameObject go = null;
        if (completedBuildingPrefab != null)
        {
            go = Instantiate(completedBuildingPrefab, transform.position, transform.rotation);
        }

        buildingManager.FinishConstruction(buildingType, go);

        Destroy(gameObject);
    }
}