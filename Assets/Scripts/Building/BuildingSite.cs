// Fichier : Assets/Scripts/Buildings/BuildingSite.cs
using UnityEngine;

public class BuildingSite : MonoBehaviour
{
    public BuildingType buildingType;
    public int masonsNeeded; // Nombre de maçons requis (pour la complexité)
    public float constructionTime = 10f; // Temps total requis (effort)

    private float constructionProgress = 0f; // Progrès actuel (0 à 100)
    private BuildingManager buildingManager;

    // Le prefab du bâtiment terminé à instancier
    public GameObject completedBuildingPrefab;

    void Start()
    {
        buildingManager = FindObjectOfType<BuildingManager>();

        // Optionnel : L'effort total peut être calculé en fonction du nombre de maçons requis * le temps.
    }

    // Fonction appelée par un Maçon (WorkingState)
    public bool Contribute(Unit mason)
    {
        // 1. Ajouter le progrès
        float effort = Time.deltaTime; // Chaque frame où le maçon est là, il travaille
        constructionProgress += effort;

        Debug.Log($"Chantier {buildingType} : {Mathf.Round(constructionProgress / constructionTime * 100)}%");

        // 2. Vérifier la fin de la construction
        if (constructionProgress >= constructionTime)
        {
            FinishBuilding();
            return true;
        }
        return false;
    }

    private void FinishBuilding()
    {
        GameObject go = null;
        // 1. Instancier le bâtiment terminé
        if (completedBuildingPrefab != null)
        {
            go = Instantiate(completedBuildingPrefab, transform.position, transform.rotation);
        }

        // 2. Informer le BuildingManager
        buildingManager.FinishConstruction(buildingType, go);

        // 3. Détruire le chantier
        Destroy(gameObject);

        Debug.Log($"{buildingType} est terminé !");
    }
}