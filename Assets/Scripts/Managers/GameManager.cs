// Fichier : Assets/Scripts/Managers/GameManager.cs
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{

    public int CurrentFood = 15;
    public int CurrentWood = 20;
    public int CurrentStone= 20;

    private float prosperityGauge = 0f;
    public float ProsperityGaugePercentage => prosperityGauge / maxProsperity * 100f;
    public Slider prosperitySlider;

    public float maxProsperity = 100f;

    [SerializeField] private List<Unit> population = new List<Unit>();
    public GameObject unitPrefab;


    // Lot of texts
    public TextMeshProUGUI woodText, stoneText, foodText, popText;

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

    private void Update()
    {
        foodText.text = CurrentFood.ToString();
        woodText.text = CurrentWood.ToString();
        stoneText.text = CurrentStone.ToString();
        popText.text = population.Count.ToString();
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
            ChangeProsperity(5f);
        }
    }


    // Day end, spawn etc
    private void OnDayStartHandler(int currentDay)
    {
        if (currentDay > 1)
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
                victim.Die("Hunger");
            }

            population.RemoveAll(u => victims.Contains(u));

            CheckGameOverCondition();
        }

        UpdateProsperityGauge();
    }


    // Resources ++
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
        prosperityGauge = prosperityGauge + change;
        prosperitySlider.value = prosperityGauge;
        CheckWinCondition();
    }

    private void UpdateProsperityGauge()
    {
        int unhappyCount = population.Count(u => u.isUnhappy);
        ChangeProsperity(-5);
    }


    // Win / lose
    private void CheckWinCondition()
    {
        if (prosperityGauge >= maxProsperity)
        {
            Debug.Log("Victory!");
            Time.timeScale = 0f;
        }
    }

    private void CheckGameOverCondition()
    {
        if (population.Count == 0)
        {
            Debug.Log("Defeat!");
            Time.timeScale = 0f;
        }
    }
}