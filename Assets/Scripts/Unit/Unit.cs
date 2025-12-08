// Fichier : Assets/Scripts/Units/Unit.cs
using UnityEngine;

[RequireComponent(typeof(UnitStateMachine), typeof(UnitMovement))]
public class Unit : MonoBehaviour
{
    // Références aux composants
    public UnitStateMachine StateMachine { get; private set; }
    public UnitMovement Movement { get; private set; }
    private BuildingManager buildingManager;
    public GameObject currentHouse;

    // Données de l'individu
    public Job currentJob = Job.Vagabond;
    public int age = 0;
    public bool isTired = false;
    public bool isUnhappy = false;

    public int maxAge = 80;

    void Awake()
    {
        StateMachine = GetComponent<UnitStateMachine>();
        Movement = GetComponent<UnitMovement>();
        buildingManager = FindObjectOfType<BuildingManager>();
    }

    void OnEnable()
    {
        TimeManager.OnDayEnd += OnDayEndHandler;
        BuildingManager.OnNewBuildingSiteCreated += OnNewBuildingSiteCreatedHandler;
    }

    void OnDisable()
    {
        TimeManager.OnDayEnd -= OnDayEndHandler;
        BuildingManager.OnNewBuildingSiteCreated -= OnNewBuildingSiteCreatedHandler;
    }

    // NOUVEAU GESTIONNAIRE D'ÉVÉNEMENT : Réaction au nouveau chantier
    private void OnNewBuildingSiteCreatedHandler(BuildingSite site)
    {
        // 1. SEULS les maçons doivent réagir
        if (currentJob == Job.Mason)
        {
            // 2. Vérifier s'ils sont déjà en WorkingState (pour éviter de les réinitialiser inutilement)
            if (StateMachine.currentState.GetType() != typeof(WorkingState))
            {
                // Les maçons interrompent leur errance ou leur repos (si non en sommeil profond)
                // et sont forcés de passer en WorkingState.
                StateMachine.ChangeState(new WorkingState());
                Debug.Log($"{gameObject.name} (Maçon) est appelé à travailler sur le nouveau chantier.");
            }
            else
            {
                // Si le maçon était déjà au travail, il se contente de mettre à jour sa cible lors du prochain OnExecute
                // (La logique dans WorkingState le gère automatiquement en trouvant le chantier actif).
            }
        }
    }

    private void OnDayEndHandler()
    {
        age++;

        if (age > maxAge)
        {
            Die("vieillesse");
            return;
        }

        // Si l'individu a travaillé/étudié, il est fatigué (logique simplifiée)
        if (currentJob != Job.Vagabond && !isTired)
        {
            isTired = true;
            isUnhappy = true;
            // L'unité doit chercher un repos (via sa machine à états)
            StateMachine.ChangeState(new SeekingRestState());
        }
    }

    public void Die(string cause)
    {
        Debug.Log($"{gameObject.name} est mort de {cause}.");
        // Le GameManager met à jour la population
        Destroy(gameObject);
    }

    // Appelée par l'UI du joueur
    public void SetJob(Job newJob)
    {
        if (!buildingManager.IsSchoolBuilt())
        {
            Debug.LogWarning("Impossible d'apprendre un nouveau métier : aucune École n'est construite.");
            return;
        }

        if (currentJob == newJob) return;

        // Transition vers l'état d'apprentissage
        StateMachine.ChangeState(new SchoolingState(newJob));
    }

    public void Rest()
    {
        isTired = false;
        isUnhappy = false;
        // La place dans la maison doit être libérée
        buildingManager.ReleaseRestSlot(currentHouse.GetComponent<Building>());

        // Transition vers le travail ou l'errance
        if (currentJob != Job.Vagabond)
        {
            StateMachine.ChangeState(new WorkingState());
        }
        else
        {
            StateMachine.ChangeState(new ErranceState());
        }
    }


}