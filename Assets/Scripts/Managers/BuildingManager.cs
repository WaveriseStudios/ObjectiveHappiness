using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class BuildingManager : MonoBehaviour
{
    [System.Serializable]
    public struct BuildingCost
    {
        public BuildingType type;
        public int woodCost;
        public int stoneCost;
        public int masonsRequired;
    }

    public List<BuildingCost> costs;

    private GameManager gameManager;
    private Dictionary<BuildingType, int> buildingCounts = new Dictionary<BuildingType, int>();
    private int availableRestSlots = 0;
    public GameObject buildingSitePrefab;

    public List<GameObject> buildingPrefabs;

    public static event UnityAction<BuildingSite> OnNewBuildingSiteCreated;

    private List<Building> activeHouses = new List<Building>();
    private List<Building> activeSchool = new List<Building>();

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();

        foreach (BuildingType type in System.Enum.GetValues(typeof(BuildingType)))
        {
            buildingCounts.Add(type, 0);
        }
    }

    public bool IsSchoolBuilt() => buildingCounts[BuildingType.School] > 0;

    public Building GetSchool()
    {
        System.Random rnd = new System.Random();
        return activeSchool[0];
    }

    public bool CanStartConstruction(BuildingType type)
    {
        BuildingCost cost = GetCost(type);
        return gameManager.CurrentWood >= cost.woodCost &&
               gameManager.CurrentStone >= cost.stoneCost;
    }

    public void StartConstruction(BuildingType type, Vector3 placementPosition, Quaternion rotation)
    {
        if (!CanStartConstruction(type)) return;

        BuildingCost cost = GetCost(type);


        // Reduce resources by the amount needed for the building
        gameManager.AddResource(ResourceType.Wood, -cost.woodCost);
        gameManager.AddResource(ResourceType.Stone, -cost.stoneCost);

        GameObject siteObject = Instantiate(buildingSitePrefab, placementPosition, rotation);
        BuildingSite site = siteObject.GetComponent<BuildingSite>();


        // Check if valid
        if (site != null)
        {
            site.buildingType = type;
            site.masonsNeeded = cost.masonsRequired;

            GameObject completedPrefab = buildingPrefabs.Find(p => p.GetComponent<Building>()?.type == type);
            site.completedBuildingPrefab = completedPrefab;

            OnNewBuildingSiteCreated?.Invoke(site);
        }
    }

    public void FinishConstruction(BuildingType type, GameObject site)
    {
        buildingCounts[type]++;

        // Not optimal ; see later for a fix


        if (type == BuildingType.House)
        {
            availableRestSlots += 2;
            activeHouses.Add(site.GetComponent<Building>());
            gameManager.ChangeProsperity(15f);
        }
        else if (type == BuildingType.Museum)
        {
            gameManager.ChangeProsperity(25f);
        }
        else if (type == BuildingType.Library)
        {
            gameManager.ChangeProsperity(20f);
        }
        else if(type == BuildingType.School)
        {
            activeSchool.Add(site.GetComponent<Building>());
        }
    }

    public BuildingCost GetCost(BuildingType type) => costs.First(c => c.type == type);

    public bool TryAcquireRestSlot()
    {
        if (availableRestSlots > 0)
        {
            availableRestSlots--;
            return true;
        }
        return false;
    }

    public Building FindAvailableHouseAndAcquireSlot(Vector3 position)
    {

        // Filter to search for the closest and available house
        Building availableHouse = activeHouses
            .Where(h => h.currentRestOccupancy < h.maxRestOccupancy)
            .OrderBy(h => Vector3.Distance(position, h.transform.position))
            .FirstOrDefault();

        if (availableHouse != null)
        {
            availableHouse.TryAcquireSlot();
            return availableHouse;
        }

        return null;
    }

    public void ReleaseRestSlot(Building house)
    {
        if (house != null)
        {
            house.ReleaseSlot();
            availableRestSlots++;
        }
    }
}