// Fichier : Assets/Scripts/Controllers/CameraController.cs
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Déplacement Horizontal")]
    [Tooltip("Vitesse de déplacement latéral de la caméra (WASD/Flèches).")]
    public float panSpeed = 20f;

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


    void Update()
    {
        // Gère le mouvement horizontal (Panning)
        HandlePanMovement();

        // Gère le zoom et la rotation (Zoom/Pitch)
        HandleZoomAndPitch();
    }

    /// <summary>
    /// Gère le déplacement horizontal de la caméra (WASD/Flèches).
    /// </summary>
    private void HandlePanMovement()
    {
        // Input des axes pour le déplacement
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // Calcul du vecteur de déplacement dans le plan horizontal (Space.World)
        Vector3 move = new Vector3(x, 0, z) * panSpeed * Time.deltaTime;

        // Applique le déplacement
        transform.position += move;

        // Optionnel : Clamper la position pour rester dans les limites de la carte
        Vector3 clampedPosition = transform.position;
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, mapBoundsX.x, mapBoundsX.y);
        clampedPosition.z = Mathf.Clamp(clampedPosition.z, mapBoundsZ.x, mapBoundsZ.y);
        transform.position = clampedPosition;
    }

    /// <summary>
    /// Gère le zoom (Molette) en ajustant la hauteur et l'angle d'observation.
    /// </summary>
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
}