// Fichier : Assets/Scripts/Units/UnitStateMachine.cs
using UnityEngine;

public class UnitStateMachine : MonoBehaviour
{
    private Unit unit;
    public IUnitState currentState;

    // Fichier : Assets/Scripts/Units/UnitStateMachine.cs
    void Start()
    {
        unit = GetComponent<Unit>();

        // Ancien code (trop général, force l'Errance si ce n'est pas Vagabond)
        if (unit.currentJob == Job.Vagabond)
        {
            ChangeState(new ErranceState());
        }
        else
        {
            // Les individus de départ commencent par travailler
            ChangeState(new WorkingState());
        }
    }

    void Update()
    {
        if (currentState != null)
        {
            currentState.OnExecute(unit);
        }
    }

    public void ChangeState(IUnitState newState)
    {
        if (currentState != null)
        {
            currentState.OnExit(unit);
        }

        currentState = newState;

        if (currentState != null)
        {
            currentState.OnEnter(unit);
        }

        // Debug.Log($"{unit.gameObject.name} -> {newState.GetType().Name}");
    }
}