// Fichier : Assets/Scripts/Managers/TimeManager.cs
using UnityEngine;
using UnityEngine.Events;

public class TimeManager : MonoBehaviour
{
    // Paramètres
    public float dayDurationInSeconds = 60f; // Durée d'une journée (Y unités de temps)

    // NOUVEAU : Fréquence de tick
    [Tooltip("Fréquence à laquelle le temps de jeu avance (une fois toutes les X secondes non-scalées).")]
    public float tickFrequency = 0.1f; // 10 ticks par seconde (1 / 0.1f)

    // Événements
    public static event UnityAction OnDayEnd;
    public static event UnityAction<int> OnDayStart;
    public static event UnityAction OnGameTick; // NOUVEAU : Événement pour les actions qui doivent se produire à chaque tick

    // Variables de contrôle
    private float timeSinceLastTick = 0f; // Temps non-scalé écoulé depuis le dernier tick
    private float timeSinceDayStart = 0f;
    private int currentDay = 1;
    private float timeScaleFactor = 1f; // Multiplicateur (x1, x2, x3)

    void Start()
    {
        // Assurez-vous que Time.timeScale est à 1 au démarrage
        Time.timeScale = 1f;
        SetTimeScale(1f); // Initialise timeScaleFactor

        // Démarre le jour 1
        OnDayStart?.Invoke(currentDay);
    }

    void Update()
    {
        // --- 1. Gestion du Tick Personnalisé (Utilise le temps non-scalé) ---
        // Time.unscaledDeltaTime continue d'avancer même si Time.timeScale = 0
        timeSinceLastTick += Time.unscaledDeltaTime;

        if (timeSinceLastTick >= tickFrequency)
        {
            // Calcule le nombre de ticks qui devraient avoir eu lieu
            int ticksToProcess = Mathf.FloorToInt(timeSinceLastTick / tickFrequency);
            timeSinceLastTick -= ticksToProcess * tickFrequency; // Remettre à jour le temps restant

            // Avance le temps de jeu
            float gameTimeAdvanced = (ticksToProcess * tickFrequency) * timeScaleFactor;
            timeSinceDayStart += gameTimeAdvanced;

            // Déclenche l'événement de tick pour toutes les unités/logiques qui en ont besoin
            OnGameTick?.Invoke();

            // --- 2. Vérification de fin de journée ---
            if (timeSinceDayStart >= dayDurationInSeconds)
            {
                EndDay();
            }
        }
    }

    // ... (Reste du code pour EndDay, TogglePause et SetTimeScale) ...

    private void EndDay()
    {
        OnDayEnd?.Invoke();
        timeSinceDayStart = 0f;
        currentDay++;
        OnDayStart?.Invoke(currentDay);

        Debug.Log($"Jour {currentDay} commencé.");
    }

    public void TogglePause()
    {
        // La pause n'affecte pas l'avancement du temps de jeu ici (Update utilise Time.unscaledDeltaTime).
        // Elle sert uniquement à contrôler Time.timeScale (pour les animations, physics, etc.)

        if (Time.timeScale > 0f) // Mettre en pause
        {
            Time.timeScale = 0f;
            Debug.Log("Jeu en PAUSE (Time.timeScale = 0)");
        }
        else // Reprendre
        {
            Time.timeScale = timeScaleFactor;
            Debug.Log($"Jeu repris (Time.timeScale = {timeScaleFactor})");
        }
    }

    public void SetTimeScale(float scale)
    {
        if (scale == 1f || scale == 2f || scale == 3f)
        {
            timeScaleFactor = scale;
            if (Time.timeScale > 0f) // Applique uniquement si le jeu n'est pas en pause réelle
            {
                Time.timeScale = scale;
            }
            Debug.Log($"Vitesse de jeu définie sur x{scale}");
        }
    }
}