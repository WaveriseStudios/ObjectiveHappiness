// Fichier : Assets/Scripts/Buildings/ResourceNode.cs (MISE À JOUR)
using UnityEngine;

public class ResourceNode : MonoBehaviour
{
    public ResourceType resourceType;
    public int baseGatherAmount = 1;
    [Tooltip("Temps en secondes entre chaque récolte de cette ressource.")]
    public float gatherInterval = 2f;

    private float timeSinceLastGather = 0f;

    // Cette fonction est maintenant un vérificateur/déclencheur
    public bool TryGather(Unit gatheringUnit)
    {
        // 1. Vérifie si l'intervalle de temps est écoulé
        timeSinceLastGather += Time.deltaTime;

        if (timeSinceLastGather >= gatherInterval)
        {
            timeSinceLastGather = 0f; // Réinitialiser le compteur

            int amountGathered = baseGatherAmount;

            // 2. Déclencher l'ajout de ressource
            GameManager gm = GameObject.FindObjectOfType<GameManager>();
            Debug.Log("nga");
            if (gm != null)
            {
                gm.AddResource(resourceType, amountGathered);
                Debug.Log($"{gatheringUnit.gameObject.name} a récolté {amountGathered} de {resourceType}. Total: {gm.CurrentFood} (exemple).");
            }
            return true;
        }
        return false;
    }
}