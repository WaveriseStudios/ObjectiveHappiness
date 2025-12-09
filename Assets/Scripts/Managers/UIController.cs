// Fichier : Assets/Scripts/UI/UIController.cs
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;

public class UIController : MonoBehaviour
{
    private BuildingManager buildingManager;
    private BuildingType selectedBuilding;
    private bool isPlacingMode = false;
    private LayerMask placementLayerMask;
    public LayerMask playerLayerMask;
    public string ignoredLayerName = "Ignore Raycast"; // À configurer dans Unity

    public GameObject placementPreviewPrefab;
    private GameObject currentPreview;
    private GameObject currentSelectedPlayer;

    public Camera mainCamera;

    public float placementHeight = 0.5f;
    public float rotationSpeed = 90f;

    void Start()
    {
        buildingManager = FindObjectOfType<BuildingManager>();

        // Initialisation du masque de Layer pour ignorer le preview
        int ignoredLayer = LayerMask.NameToLayer(ignoredLayerName);
        if (ignoredLayer != -1)
        {
            placementLayerMask = ~(1 << ignoredLayer);
        }
        else
        {
            placementLayerMask = ~0; // Tout est inclus si le Layer n'existe pas
            Debug.LogError($"Layer '{ignoredLayerName}' non trouvé.");
        }
    }

    void Update()
    {
        if (isPlacingMode)
        {
            HandlePlacementInput();
        }
        else
        {
            HandleSelectionInput();
        }
    }

    // Fonction appelée par les boutons de l'UI (avec le nom de l'Enum en string)
    public void SelectBuildingToPlace(string buildingTypeName)
    {
        if (System.Enum.TryParse(buildingTypeName, out BuildingType type))
        {
            selectedBuilding = type;

            if (buildingManager.CanStartConstruction(selectedBuilding))
            {
                if (isPlacingMode)
                {
                    ExitPlacementMode();
                }

                isPlacingMode = true;

                // CRÉATION DE LA PRÉVISUALISATION
                if (placementPreviewPrefab != null)
                {
                    currentPreview = Instantiate(placementPreviewPrefab);
                }
            }
        }
    }

    private void HandlePlacementInput()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            if (currentPreview != null) currentPreview.SetActive(false);
            return;
        }

        if (currentPreview != null) currentPreview.SetActive(true);

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1000f, placementLayerMask))
        {
            Vector3 placementPosition = hit.point + Vector3.up * placementHeight;
            bool isValid = IsPlacementValid(hit);

            if (currentPreview != null)
            {
                currentPreview.transform.position = placementPosition;
                Renderer renderer = currentPreview.GetComponentInChildren<Renderer>();
                if (renderer != null)
                {
                    renderer.material.color = isValid ? Color.green : Color.red;
                }
            }

            if (Input.GetMouseButton(1))
            {
                if (currentPreview != null)
                {
                    currentPreview.transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
                }
            }

            if (Input.GetMouseButtonDown(0))
            {
                if (isValid)
                {
                    // Lancer la construction avec la rotation
                    Quaternion finalRotation = currentPreview.transform.rotation;
                    buildingManager.StartConstruction(selectedBuilding, placementPosition, finalRotation);

                    ExitPlacementMode();
                }
            }

            // Annuler le placement avec la touche Échap
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ExitPlacementMode();
            }
        }
    }

    private void HandleSelectionInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 1000f, playerLayerMask))
            {

                if(hit.collider.gameObject)
                {
                    currentSelectedPlayer = hit.collider.gameObject;
                    mainCamera.GetComponent<CameraController>().Focus(currentSelectedPlayer);
                }
            }
        }

        // Annuler le placement avec la touche Échap
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ExitSelectionMode();
        }
    }

    private void ExitPlacementMode()
    {
        isPlacingMode = false;

        // DÉTRUIRE L'OBJET DE PRÉVISUALISATION
        if (currentPreview != null)
        {
            Destroy(currentPreview);
            currentPreview = null;
        }
    }

    private void ExitSelectionMode()
    {
        if(mainCamera.GetComponent<CameraController>().isFocusing)
        {
            mainCamera.GetComponent<CameraController>().ExitFocus();
        }
    }

    private bool IsPlacementValid(RaycastHit hit)
    {
        if (!hit.collider.gameObject.CompareTag("TerrainPlaine"))
        {
            return false;
        }

        // Interdit sur les zones de récolte ou près d'un autre bâtiment
        Collider[] colliders = Physics.OverlapSphere(hit.point, 1.0f);
        foreach (Collider col in colliders)
        {
            if (col.GetComponent<ResourceNode>() != null || col.GetComponent<Building>() != null || col.GetComponent<BuildingSite>() != null)
            {
                return false;
            }
        }

        return true;
    }
}