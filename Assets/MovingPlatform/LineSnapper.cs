using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LineSnapper : MonoBehaviour
{
    [SerializeField]
    private Transform startPoint; // Premier objet (départ de la ligne)

    [SerializeField]
    public Transform endPoint; // Deuxième objet (fin de la ligne)

    private LineRenderer lineRenderer;

    private void OnValidate()
    {
        InitializeLineRenderer();
        UpdateLineRenderer();
    }

    private void Awake()
    {
        InitializeLineRenderer();
    }

    private void Update()
    {
        UpdateLineRenderer();
    }

    private void InitializeLineRenderer()
    {
        // Récupère le Line Renderer attaché à l'objet
        lineRenderer = GetComponent<LineRenderer>();

        // Définit le nombre de positions (2 pour une ligne simple)
        lineRenderer.positionCount = 2;
    }

    private void UpdateLineRenderer()
    {
        // Met à jour les positions du Line Renderer à chaque frame
        if (startPoint != null && endPoint != null)
        {
            lineRenderer.SetPosition(0, startPoint.position); // Position de départ (startPoint)
            lineRenderer.SetPosition(1, endPoint.position); // Position de fin (endPoint)
        }
    }
}
