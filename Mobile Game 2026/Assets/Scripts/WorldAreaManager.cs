using UnityEngine;

public class WorldAreaManager : MonoBehaviour
{
    public static WorldAreaManager Instance;

    [Header("References")]
    public Camera cam;
    public Material gridMaterial;

    [Header("Offsets")]
    public float playAreaPadding = 0f;     // usually 0 — exactly matches camera
    public float spawnAreaPadding = 10f;   // +10 outward on each side

    [Header("Computed (read-only)")]
    public float playWidth;
    public float playHeight;

    public float spawnWidth;
    public float spawnHeight;

    public Vector3 center; // camera-centered rect

    private void Awake()
    {
        Instance = this;

        if (cam == null)
            cam = Camera.main;

        RecalculateAreas();
    }

    private void LateUpdate()
    {
        // If camera zoom changed, update areas
        RecalculateAreas();
    }

    public void RecalculateAreas()
    {
        if (cam == null) return;

        // camera center
        center = cam.transform.position;

        // camera rect
        float camH = cam.orthographicSize * 2f;
        float camW = camH * cam.aspect;

        // PLAY AREA (camera rectangle)
        playWidth = camW + (playAreaPadding * 2f);
        playHeight = camH + (playAreaPadding * 2f);

        // SPAWN AREA (expanded rectangle)
        spawnWidth = playWidth + (spawnAreaPadding * 2f);
        spawnHeight = playHeight + (spawnAreaPadding * 2f);
    }

    private void Update()
    {
        if (gridMaterial != null && cam != null)
        {
            gridMaterial.SetFloat("_CameraOrthographicSize", cam.orthographicSize);

            Vector3 p = cam.transform.position;
            gridMaterial.SetVector("_CameraWorldPos", new Vector4(p.x, p.y, 0f, 0f));
        }
    }

    // -------------------------------------------------------
    // GIZMOS
    // -------------------------------------------------------
    private void OnDrawGizmos()
    {
        if (cam == null) cam = Camera.main;
        if (cam == null) return;

        RecalculateAreas();

        // PLAY AREA = lime green
        Gizmos.color = new Color(0.6f, 1f, 0.6f, 1f);
        Gizmos.DrawWireCube(center, new Vector3(playWidth, playHeight, 0));

        // SPAWN AREA = orange
        Gizmos.color = new Color(1f, 0.5f, 0f, 1f);
        Gizmos.DrawWireCube(center, new Vector3(spawnWidth, spawnHeight, 0));
    }
}
