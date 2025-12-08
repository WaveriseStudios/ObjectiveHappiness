// Fichier : Assets/Scripts/Managers/GameManager.cs
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class GameManager : MonoBehaviour
{
    // Propriétés pour l'accès public (utilisé par BuildingManager)
    public int CurrentFood { get; private set; } = 50;
    public int CurrentWood { get; private set; } = 0;
    public int CurrentStone { get; private set; } = 0;

    // Jauge de Prospérité
    private float prosperityGauge = 0f;
    public float ProsperityGaugePercentage => prosperityGauge / maxProsperity * 100f;

    public float maxProsperity = 100f;
    public int newUnitSpawnFrequency = 3;

    private List<Unit> population = new List<Unit>();
    public GameObject unitPrefab;

    void OnEnable()
    {
        TimeManager.OnDayEnd += EndOfDayLogic;
        TimeManager.OnDayStart += OnDayStartHandler;
    }

    void OnDisable()
    {
        TimeManager.OnDayEnd -= EndOfDayLogic;
        TimeManager.OnDayStart -= OnDayStartHandler;
    }

    void Start()
    {
        InitializeStartingPopulation();
    }

    private void InitializeStartingPopulation()
    {
        Job[] startingJobs = { Job.FoodGatherer, Job.Lumberjack, Job.Miner, Job.Mason, Job.Vagabond };
        foreach (Job job in startingJobs)
        {
            SpawnNewUnit(job);
        }
    }

    public void SpawnNewUnit(Job initialJob = Job.Vagabond)
    {
        GameObject newUnitObject = Instantiate(unitPrefab, Vector3.zero, Quaternion.identity);
        Unit newUnit = newUnitObject.GetComponent<Unit>();

        if (newUnit != null)
        {
            newUnit.currentJob = initialJob;
            newUnit.gameObject.name = $"{initialJob} Unit_{population.Count + 1}";
            population.Add(newUnit);
        }
    }

    private void OnDayStartHandler(int currentDay)
    {
        if (currentDay > 1 && currentDay % newUnitSpawnFrequency == 0)
        {
            SpawnNewUnit();
        }
    }

    private void EndOfDayLogic()
    {
        int foodRequired = population.Count;

        if (CurrentFood >= foodRequired)
        {
            CurrentFood -= foodRequired;
        }
        else
        {
            int foodDeficit = foodRequired - CurrentFood;
            int individualsToDie = foodDeficit;
            CurrentFood = 0;

            List<Unit> victims = population.OrderBy(x => Random.value).Take(individualsToDie).ToList();

            foreach (Unit victim in victims)
            {
                victim.Die("faim");
            }

            population.RemoveAll(u => victims.Contains(u));

            CheckGameOverCondition();
        }

        UpdateProsperityGauge();
    }

    public void AddResource(ResourceType type, int amount)
    {
        switch (type)
        {
            case ResourceType.Food:
                CurrentFood += amount;
                break;
            case ResourceType.Wood:
                CurrentWood += amount;
                break;
            case ResourceType.Stone:
                CurrentStone += amount;
                break;
        }
    }

    public void ChangeProsperity(float change)
    {
        prosperityGauge = Mathf.Clamp(prosperityGauge + change, 0f, maxProsperity);
        CheckWinCondition();
    }

    private void UpdateProsperityGauge()
    {
        int unhappyCount = population.Count(u => u.isUnhappy);
        ChangeProsperity(-unhappyCount * 0.1f);
    }

    private void CheckWinCondition()
    {
        if (prosperityGauge >= maxProsperity)
        {
            Debug.Log("VICTOIRE!");
            Time.timeScale = 0f;
        }
    }

    private void CheckGameOverCondition()
    {
        if (population.Count == 0)
        {
            Debug.Log("DÉFAITE!");
            Time.timeScale = 0f;
        }
    }
}