using UnityEngine;

/// <summary>
/// Trigger zum Wechseln der Musik-Bereiche in der Main Scene
/// Platziere diesen auf Collider (Trigger) in Beach, City und Forest Bereichen
/// </summary>
public class MusicAreaTrigger : MonoBehaviour
{
    [Header("Music Area Settings")]
    [Tooltip("Name des Bereichs: beach, city oder forest")]
    public string areaName = "beach";

    [Header("Optional: Visual Debug")]
    public bool showDebugGizmo = true;
    public Color gizmoColor = Color.yellow;

    private void OnTriggerEnter(Collider other)
    {
        // Prüfe ob der Player den Trigger betritt
        if (other.CompareTag("Player"))
        {
            // Wechsle Musik-Bereich
            if (MusicManager.Instance != null)
            {
                MusicManager.Instance.SetMainMusicArea(areaName);
                Debug.Log($"[MusicAreaTrigger] Player entered {areaName} area");
            }
            else
            {
                Debug.LogError("[MusicAreaTrigger] MusicManager not found!");
            }
        }
    }

    // Zeige den Trigger-Bereich im Editor
    private void OnDrawGizmos()
    {
        if (!showDebugGizmo) return;

        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            Gizmos.color = new Color(gizmoColor.r, gizmoColor.g, gizmoColor.b, 0.3f);

            if (col is BoxCollider)
            {
                BoxCollider box = (BoxCollider)col;
                Gizmos.matrix = transform.localToWorldMatrix;
                Gizmos.DrawCube(box.center, box.size);
            }
            else if (col is SphereCollider)
            {
                SphereCollider sphere = (SphereCollider)col;
                Gizmos.DrawSphere(transform.position + sphere.center, sphere.radius);
            }
        }
    }
}