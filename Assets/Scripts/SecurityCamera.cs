using UnityEngine;

public class SecurityCamera : MonoBehaviour
{
    [SerializeField] private int resolution = 30;
    [SerializeField] private float range = 10f;
    [SerializeField] private float angle = 45f;
    [SerializeField] private float suspicionFillRate = 25f;
    [SerializeField] private float suspicionDrainRate = 15f;
    [SerializeField] private float trackSpeed = 90f;

    // Color refs
    [SerializeField] private Color defaultColor = Color.green;
    [SerializeField] private Color trackingColor = Color.yellow;
    [SerializeField] private Color alertColor = Color.red;

    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private Material coneMaterial;

    private Transform player;
    private Quaternion baseRotation;
    private float suspicionMeter = 0f;
    void Awake()
    {
        GameObject vizObj = new GameObject("VisionMesh");
        vizObj.transform.SetParent(transform);
        vizObj.transform.localPosition = Vector3.zero;
        vizObj.transform.localRotation = Quaternion.identity;
        meshFilter = vizObj.AddComponent<MeshFilter>();
        meshRenderer = vizObj.AddComponent<MeshRenderer>();

        coneMaterial = new Material(Shader.Find("Legacy Shaders/Transparent/Diffuse"));
        coneMaterial.color = new Color(0f, 1f, 0f, 0.2f);
        meshRenderer.material = coneMaterial;

        player = GameObject.FindWithTag("Player").transform;
        baseRotation = transform.rotation;

        UpdateVision(defaultColor, suspicionMeter);
    }
    void Update()
    {
        bool canSee = CanSeePlayer();

        if (canSee)
        {
            suspicionMeter = Mathf.Clamp(suspicionMeter + suspicionFillRate * Time.deltaTime, 0f, 100f);
            TrackPlayer();
        }
        else
        {
            suspicionMeter = Mathf.Clamp(suspicionMeter - suspicionDrainRate * Time.deltaTime, 0f, 100f);
            ReturnToDefault();
        }

        if (suspicionMeter >= 100f)
            player.GetComponent<PlayerController>().TriggerLose();

        Color color = canSee ? (suspicionMeter >= 100f ? alertColor : trackingColor) : defaultColor;
        UpdateVision(color, suspicionMeter);
    }
    private bool CanSeePlayer()
    {
        Vector3 toPlayer = player.position - transform.position;
        float distance = toPlayer.magnitude;

        if (distance > range) return false;
        if (Vector3.Angle(transform.forward, toPlayer) > angle / 2f) return false;

        Ray ray = new Ray(transform.position + Vector3.up * 0.5f, toPlayer.normalized);
        if (Physics.Raycast(ray, out RaycastHit hit, range))
            return hit.collider.CompareTag("Player");

        return false;
    }
    private void TrackPlayer()
    {
        Vector3 dir = player.position - transform.position;
        dir.y = 0;
        if (dir == Vector3.zero) return;
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            Quaternion.LookRotation(dir),
            trackSpeed * Time.deltaTime
        );
    }

    private void ReturnToDefault()
    {
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            baseRotation,
            trackSpeed * Time.deltaTime
        );
    }
    private void UpdateVision(Color stateColor, float suspicion)
    {
        float alpha = Mathf.Lerp(0.1f, 0.5f, suspicionMeter / 100f);
        coneMaterial.color = new Color(stateColor.r, stateColor.g, stateColor.b, alpha);
        BuildMesh();
    }

    private void BuildMesh()
    {
        int coneVerts = resolution + 2;
        Vector3[] coneVertices = new Vector3[coneVerts];
        int[] coneTriangles = new int[resolution * 3];

        coneVertices[0] = Vector3.up * 0.05f;
        float halfAngle = angle / 2f;

        for (int i = 0; i <= resolution; i++)
        {
            float t = (float)i / resolution;
            float a = Mathf.Lerp(-halfAngle, halfAngle, t) * Mathf.Deg2Rad;
            coneVertices[i + 1] = new Vector3(Mathf.Sin(a) * range, 0.05f, Mathf.Cos(a) * range);
        }

        for (int i = 0; i < resolution; i++)
        {
            coneTriangles[i * 3 + 0] = 0;
            coneTriangles[i * 3 + 1] = i + 1;
            coneTriangles[i * 3 + 2] = i + 2;
        }
        Mesh mesh = new Mesh();
        mesh.vertices = coneVertices;
        mesh.triangles = coneTriangles;
        mesh.RecalculateNormals();
        meshFilter.mesh = mesh;
    }
}