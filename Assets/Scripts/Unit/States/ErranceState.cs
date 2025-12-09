// Fichier : Assets/Scripts/Units/States/ErranceState.cs
using UnityEngine;

public class ErranceState : IUnitState
{
    private float wanderTimer = 0f;
    private float wanderInterval = 5f;

    public void OnEnter(Unit unit)
    {
        unit.Movement.StopMoving();
        wanderTimer = 0f;
        unit.Movement.MoveToRandomLocation(10f); // Se déplacer aléatoirement
    }

    public void OnExecute(Unit unit)
    {
        // 1. Vérifier si une maison est disponible (si l'unité est fatiguée)
        if (unit.isTired)
        {
            unit.StateMachine.ChangeState(new SeekingRestState());
            return;
        }

        // 2. Errance aléatoire
        if (unit.Movement.IsArrived())
        {
            wanderTimer += Time.deltaTime;
            if (wanderTimer >= wanderInterval)
            {
                unit.Movement.MoveToRandomLocation(10f);
                wanderTimer = 0f;
            }
        }
    }

    public void OnExit(Unit unit)
    {
        unit.Movement.StopMoving();
    }
}