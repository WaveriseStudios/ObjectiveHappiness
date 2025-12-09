// Fichier : Assets/Scripts/Controllers/CameraController.cs
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraController : MonoBehaviour
{
    public Transform baseCameraPosition;

    public float transitionSmoothSpeed = 5f; // Vitesse de lissage (ajustez dans l'Inspecteur)

    // Variables d'état pour le focus
    private bool isTransitioning = false;

    // Cibles de l'interpolation
    private Vector3 targetPosition;
    private Quaternion targetRotation;
    private Transform targetParent;

    [Header("Déplacement Horizontal")]
    [Tooltip("Vitesse de déplacement latéral de la caméra.")]
    public float panSpeed = 20f;

    [Tooltip("Zone morte (en pourcentage de l'écran, 0 à 0.5) au centre où le mouvement est ignoré.")]
    [Range(0f, 0.5f)]
    public float deadZonePercentage = 0.2f; // 20% du centre de l'écran est une zone morte

    [Tooltip("Vitesse à laquelle la caméra glisse lors du zoom.")]
    public float zoomSensitivity = 10f;

    [Header("Paramètres de Zoom (Hauteur et Angle)")]
    [Tooltip("Niveau de zoom actuel (0 = maximum zoomé, 1 = minimum zoomé).")]
    [Range(0f, 1f)]
    public float currentZoomLevel = 0.5f;

    [Tooltip("Hauteur minimale (zoom max).")]
    public float minHeight = 5f;

    [Tooltip("Hauteur maximale (zoom min).")]
    public float maxHeight = 30f;

    [Tooltip("Angle de pitch (rotation X) minimal (regarde plus horizontalement, zoomé).")]
    public float minAngle = 30f;

    [Tooltip("Angle de pitch (rotation X) maximal (regarde plus vers le sol, dézoomé).")]
    public float maxAngle = 75f;

    [Tooltip("Vitesse de lissage (Lerp) pour le zoom.")]
    public float smoothSpeed = 5f;

    [Header("Limites de la Carte (Optionnel)")]
    // Exemple de limites pour éviter que la caméra ne sorte de la carte
    public Vector2 mapBoundsX = new Vector2(-50f, 50f);
    public Vector2 mapBoundsZ = new Vector2(-50f, 50f);

    public bool isFocusing = false;


    void Update()
    {
        // Priorité à la transition si elle est active
        if (isTransitioning)
        {
            PerformTransition();
        }
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            // Le mouvement est bloqué, mais le zoom reste actif pour une meilleure UX (cf. point 3)
            HandleZoomAndPitch();
            return;
        }
        else
        {
            // Le mouvement par zones de l'écran ou le zoom ne s'activent que si la caméra n'est pas en transition
            // ET pas verrouillée sur une cible (si isFocusing est le flag pour le mode verrouillé)
            if (!isFocusing)
            {
                // Gère le mouvement horizontal (Panning)
                HandlePanMovement();

                // Gère le zoom et la rotation (Zoom/Pitch)
                HandleZoomAndPitch();
            }
        }
    }

    private void PerformTransition()
    {
        // 1. Interpolation de la Position (Lerp pour un mouvement linéaire)
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * transitionSmoothSpeed);

        // 2. Interpolation de la Rotation (Slerp pour une rotation sphérique douce)
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * transitionSmoothSpeed);

        // 3. Vérification de la fin de la transition
        // La transition est terminée si la position et la rotation sont très proches de la cible.
        if (Vector3.Distance(transform.position, targetPosition) < 0.01f &&
            Quaternion.Angle(transform.rotation, targetRotation) < 0.1f)
        {
            // Snap aux valeurs finales pour une précision parfaite
            transform.position = targetPosition;
            transform.rotation = targetRotation;

            // Appliquer le parent final (Verrouillage ou retour à la position de base)
            transform.SetParent(targetParent);

            // Arrêter la transition
            isTransitioning = false;
        }
    }
    private void HandlePanMovement()
    {
        Vector3 move = Vector3.zero;

        // Calcul des points de référence
        float screenCenterY = Screen.height * 0.5f;
        float screenCenterX = Screen.width * 0.5f;

        // Calcul de la taille de la zone morte en pixels (du centre)
        float deadZoneY = Screen.height * deadZonePercentage;
        float deadZoneX = Screen.width * deadZonePercentage;

        // --- Déplacement AVANT/ARRIÈRE (Axe Z) ---
        // Si la souris est au-dessus du centre + zone morte
        if (Input.mousePosition.y > screenCenterY + deadZoneY)
        {
            move.z = 1f; // Avancer
        }
        // Si la souris est en dessous du centre - zone morte
        else if (Input.mousePosition.y < screenCenterY - deadZoneY)
        {
            move.z = -1f; // Reculer
        }

        // --- Déplacement GAUCHE/DROITE (Axe X) ---
        // Si la souris est à gauche du centre - zone morte
        if (Input.mousePosition.x < screenCenterX - deadZoneX)
        {
            move.x = -1f; // Gauche
        }
        // Si la souris est à droite du centre + zone morte
        else if (Input.mousePosition.x > screenCenterX + deadZoneX)
        {
            move.x = 1f; // Droite
        }

        // Normaliser le vecteur de mouvement
        move.Normalize();

        // Applique le déplacement
        transform.position += move * panSpeed * Time.deltaTime;

        // Optionnel : Clamper la position pour rester dans les limites de la carte
        Vector3 clampedPosition = transform.position;
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, mapBoundsX.x, mapBoundsX.y);
        clampedPosition.z = Mathf.Clamp(clampedPosition.z, mapBoundsZ.x, mapBoundsZ.y);
        transform.position = clampedPosition;
    }
    private void HandleZoomAndPitch()
    {
        // Lecture de l'input de la molette de souris
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");

        // Mise à jour progressive du niveau de zoom actuel (clamped entre 0 et 1)
        // La molette vers l'avant (positif) diminue le niveau de zoom (dézoomer)
        // La molette vers l'arrière (négatif) augmente le niveau de zoom (zoomer)
        currentZoomLevel = Mathf.Clamp01(currentZoomLevel - scrollInput * zoomSensitivity * Time.deltaTime);

        // --- 1. APPLICATION DE LA HAUTEUR (Y) ---
        // Lerp entre la hauteur max (dézoomé) et la hauteur min (zoomé)
        float targetY = Mathf.Lerp(minHeight, maxHeight, currentZoomLevel);

        // Lissage de la position Y
        Vector3 newPos = transform.position;
        newPos.y = Mathf.Lerp(newPos.y, targetY, Time.deltaTime * smoothSpeed);
        transform.position = newPos;

        // --- 2. APPLICATION DE L'ANGLE (Pitch X) ---
        // Lerp entre l'angle max (dézoomé) et l'angle min (zoomé)
        float targetXAngle = Mathf.Lerp(maxAngle, minAngle, currentZoomLevel);

        // Lissage de la rotation X (Pitch)
        Quaternion targetRotation = Quaternion.Euler(targetXAngle, transform.eulerAngles.y, transform.eulerAngles.z);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * smoothSpeed);
    }

    public void Focus(GameObject go)
    {
        // 1. Définir les cibles finales
        Transform focusPoint = go.GetComponent<Unit>().focusPoint.transform;
        targetPosition = focusPoint.position;
        targetRotation = focusPoint.rotation;
        targetParent = go.transform; // Le parent final (l'unité)

        // 2. Lancer la transition
        isTransitioning = true;
        isFocusing = true;

    }

    public void ExitFocus()
    {
        // 1. Définir les cibles finales
        targetPosition = baseCameraPosition.position;
        targetRotation = baseCameraPosition.rotation;
        targetParent = baseCameraPosition; // Le parent final (la position de base)

        // 2. Retirer le parent (si nécessaire) pour que l'interpolation se fasse correctement
        if (transform.parent != null)
        {
            transform.SetParent(null);
        }

        // 3. Lancer la transition
        isTransitioning = true;
        isFocusing = false;
    }
}