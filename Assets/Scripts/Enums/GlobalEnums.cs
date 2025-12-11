using UnityEngine;

public enum Job
{
    Vagabond,
    FoodGatherer,
    Lumberjack,
    Miner,
    Mason
}

public enum ResourceType
{
    Food,
    Wood,
    Stone
}

public enum BuildingType
{
    House,
    School,
    Farm,
    Library,
    Museum
}

[System.Serializable]
public struct SkinToJob
{
    public Job job;
    public GameObject associatedModel;

    // Skin associated to job
}