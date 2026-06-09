using UnityEngine;

public class GuardVisionCone : MonoBehaviour
{
    [SerializeField] private int resolution = 30;
    [SerializeField] private int circleResolution = 30;

    // Bryce - "Trying to get the new shader to work with the vision cone logic already in place"
    [Header("Vision Cone Material")]
    [SerializeField] private Material hologramMaterial;
    [SerializeField] private string colorProperty = "_Color"; 

    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private Material coneMaterial;
    private Mesh combinedMesh;

    private float range, angle, circleRadius;

    void Awake()
    {
        GameObject vizObj = new GameObject("VisionMesh");
        vizObj.transform.SetParent(transform);
        vizObj.transform.localPosition = Vector3.zero;
        vizObj.transform.localRotation = Quaternion.identity;
        
        meshFilter = vizObj.AddComponent<MeshFilter>();
        meshRenderer = vizObj.AddComponent<MeshRenderer>();

        // Bryce - Changing Material for hologram purposes
        if (hologramMaterial != null)
        {
            coneMaterial = new Material(hologramMaterial);
        }
        else
        {
            coneMaterial = new Material(Shader.Find("Shader Graphs/HologramShader"));
        }

        meshRenderer.material = coneMaterial;

        combinedMesh = new Mesh();
        meshFilter.mesh = combinedMesh;
    }

    public void UpdateVision(float coneRange, float coneAngle, float circleRadius, Color stateColor, float suspicionMeter)
    {
        this.range = coneRange;
        this.angle = coneAngle;
        this.circleRadius = circleRadius;

        float t = Mathf.Clamp01(suspicionMeter / 100f);

        float alpha = Mathf.Lerp(0.25f, 0.8f, t);
        Color coneColor = new Color(stateColor.r, stateColor.g, stateColor.b, alpha);
        
        // Bryce - Testing changes to material properties
        if (coneMaterial.HasProperty(colorProperty))
        {
            coneMaterial.SetColor(colorProperty, coneColor);
        }

        if (coneMaterial.HasProperty("_FresnelPower"))
        {
            coneMaterial.SetFloat("_FresnelPower", Mathf.Lerp(2.5f, 0.75f, t));
        }

        if (coneMaterial.HasProperty("_VectorOffset"))
        {
            coneMaterial.SetFloat("_VectorOffset", Mathf.Lerp(0.05f, 0.2f, t));
        }
        if (coneMaterial.HasProperty("_ScanLineTiling"))
        {
            coneMaterial.SetVector("_ScanLineTiling", new Vector2(1f, -25f));
        }

        BuildCombinedMesh();
    }

    private void BuildCombinedMesh()
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

        int circleVerts = circleResolution + 2;
        Vector3[] circleVertices = new Vector3[circleVerts];
        int[] circleTriangles = new int[circleResolution * 3];

        circleVertices[0] = Vector3.up * 0.05f;

        for (int i = 0; i <= circleResolution; i++)
        {
            float a = (float)i / circleResolution * 360f * Mathf.Deg2Rad;
            circleVertices[i + 1] = new Vector3(Mathf.Cos(a) * circleRadius, 0.05f, Mathf.Sin(a) * circleRadius);
        }

        for (int i = 0; i < circleResolution; i++)
        {
            circleTriangles[i * 3 + 0] = 0;
            circleTriangles[i * 3 + 1] = i + 2;
            circleTriangles[i * 3 + 2] = i + 1;
        }

        combinedMesh.Clear();

        Vector3[] allVerts = new Vector3[coneVerts + circleVerts];
        coneVertices.CopyTo(allVerts, 0);
        circleVertices.CopyTo(allVerts, coneVerts);

        int[] allTris = new int[coneTriangles.Length + circleTriangles.Length];
        coneTriangles.CopyTo(allTris, 0);
        for (int i = 0; i < circleTriangles.Length; i++)
            allTris[coneTriangles.Length + i] = circleTriangles[i] + coneVerts;

        Vector2[] allUVs = new Vector2[allVerts.Length];

        for (int i = 0; i < allVerts.Length; i++)
        {
            Vector3 v = allVerts[i];

            allUVs[i] = new Vector2(
                v.x / range + 0.5f,
                v.z / range + 0.5f
            );
        }

        combinedMesh.vertices = allVerts;
        combinedMesh.triangles = allTris;
        combinedMesh.uv = allUVs;
        combinedMesh.RecalculateNormals();
            }
}