// Fichier : Assets/Scripts/Units/States/RestingState.cs
using UnityEngine;

public class RestingState : IUnitState
{
    private float timeResting = 0f;
    private float restDuration = 15f;
    private GameObject house;

    public RestingState(GameObject dest) { house = dest; }



    public void OnEnter(Unit unit)
    {
        timeResting = 0f;
        unit.currentHouse = house;
        Vector3 targetLocation = house.transform.position;
        unit.Movement.MoveTo(targetLocation);
        Debug.Log($"{unit.gameObject.name} va se reposer.");
    }

    public void OnExecute(Unit unit)
    {
        if (!unit.isArrivedToDestination) return;

        timeResting += Time.deltaTime;

        if (timeResting >= restDuration)
        {
            Debug.Log($"{unit.gameObject.name} s'est reposé");
            unit.Rest(); // Transition vers Working/Errance
        }
    }

    public void OnExit(Unit unit)
    {
        // Rien de spécifique ici, la place est libérée dans unit.Rest()
    }
}