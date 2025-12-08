// Fichier : Assets/Scripts/Managers/BuildingManager.cs
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
    private int availableRestSlots = 0; // Capacité de repos totale

    // Fichier : Assets/Scripts/Managers/BuildingManager.cs (Nouvelles variables)
    // ...
    // Référence au prefab du Chantier (à assigner dans l'Inspecteur)
    public GameObject buildingSitePrefab;

    [Tooltip("Assignez ici les prefabs des bâtiments terminés.")]
    public List<GameObject> buildingPrefabs;

    public static event UnityAction<BuildingSite> OnNewBuildingSiteCreated;

    private List<Building> activeHouses = new List<Building>();
    private List<BuildingSite> activeSites = new List<BuildingSite>();
    // ...

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();

        foreach (BuildingType type in System.Enum.GetValues(typeof(BuildingType)))
        {
            buildingCounts.Add(type, 0);
        }
    }

    public bool IsSchoolBuilt() => buildingCounts[BuildingType.School] > 0;

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

        // 1. Déduire les ressources
        gameManager.AddResource(ResourceType.Wood, -cost.woodCost);
        gameManager.AddResource(ResourceType.Stone, -cost.stoneCost);

        // 2. Créer le BuildingSite
        GameObject siteObject = Instantiate(buildingSitePrefab, placementPosition, rotation);
        BuildingSite site = siteObject.GetComponent<BuildingSite>();

        if (site != null)
        {
            // 3. Initialiser le chantier
            site.buildingType = type;
            site.masonsNeeded = cost.masonsRequired;

            GameObject completedPrefab = buildingPrefabs.Find(p => p.GetComponent<Building>()?.type == type);
            site.completedBuildingPrefab = completedPrefab;

            OnNewBuildingSiteCreated?.Invoke(site); // Notifie les Maçons
        }

        Debug.Log($"Construction de {type} démarrée. Les maçons vont s'activer.");
    }

    public void FinishConstruction(BuildingType type, GameObject site)
    {
        buildingCounts[type]++;

        if (type == BuildingType.House)
        {
            availableRestSlots += 2; // 2 places par Maison
            activeHouses.Add(site.GetComponent<Building>());
        }
        else if (type == BuildingType.Museum)
        {
            gameManager.ChangeProsperity(5f);
        }
        else if (type == BuildingType.Library)
        {
            gameManager.ChangeProsperity(2f);
        }

        Debug.Log($"{type} construit!");
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
        // 1. Filtrer toutes les maisons actives par celles qui ont de la place
        Building availableHouse = activeHouses
            .Where(h => h.currentRestOccupancy < h.maxRestOccupancy)
            .OrderBy(h => Vector3.Distance(position, h.transform.position))
            .FirstOrDefault(); // Prend la plus proche disponible

        if (availableHouse != null)
        {
            // 2. Acquisition : La maison choisie prend maintenant la responsabilité du slot
            availableHouse.TryAcquireSlot();
            return availableHouse;
        }

        return null; // Aucune maison disponible
    }

    // NOUVELLE MÉTHODE : Libérer le slot sur la maison spécifique
    public void ReleaseRestSlot(Building house)
    {
        if (house != null)
        {
            house.ReleaseSlot();
            availableRestSlots++;
        }
    }
}