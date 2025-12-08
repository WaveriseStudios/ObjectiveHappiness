// Fichier : Assets/Scripts/GlobalEnums.cs
// Contient les énumérations utilisées par tout le projet.

public enum Job
{
    Vagabond,               // Ne se fatigue jamais
    FoodGatherer,           // Récolteur de nourriture
    Lumberjack,             // Bûcheron
    Miner,                  // Mineur
    Mason                   // Maçon (pour la construction)
}

public enum ResourceType
{
    Food,
    Wood,
    Stone
}

public enum BuildingType
{
    House,      // Maison (Repos)
    School,     // École (Apprentissage de métiers)
    Farm,       // Ferme (Production de nourriture)
    Library,    // Librairie (Prospérité)
    Museum      // Musée (Prospérité)
}