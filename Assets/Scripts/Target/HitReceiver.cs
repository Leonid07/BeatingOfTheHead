using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class HitReceiver : MonoBehaviour
{
    private Mesh mesh;
    private Vector3[] originalVertices;
    private Vector3[] currentVertices;

    [Header("Bruise Settings")]
    public Texture2D bruiseTexture;
    public float bruiseSize = 0.1f;
    public Material targetMaterial;
    private RenderTexture bruiseRenderTexture;

    [Header("Sound Settings")]
    public AudioClip hitSound;

    [Header("Particle Settings")]
    public ParticleSystem hitParticlePrefab;
    public float particleDuration = 1f;

    void Start()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        originalVertices = mesh.vertices;
        currentVertices = (Vector3[])originalVertices.Clone();

        bruiseRenderTexture = new RenderTexture(512, 512, 0);
        bruiseRenderTexture.Create();
        targetMaterial.SetTexture("_BruiseTex", bruiseRenderTexture);
        ClearBruiseTexture();
    }

    public void ApplyHit(Collider targetCollider, Vector3 hitPoint, Vector3 normal, float power, float radius)
    {
        Vector3 localHit = transform.InverseTransformPoint(hitPoint);

        for (int i = 0; i < currentVertices.Length; i++)
        {
            if (Vector3.Distance(originalVertices[i], localHit) > radius)
                continue;

            Vector3 worldV = transform.TransformPoint(currentVertices[i]);
            Vector3 pen = worldV - hitPoint;
            float depth = Vector3.Dot(pen, normal);

            if (depth > 0)
            {
                float falloff = 1f - (depth / radius);
                float deformAmt = falloff * power;
                Vector3 offsetLocal = transform.worldToLocalMatrix.MultiplyVector(normal * deformAmt);
                currentVertices[i] += offsetLocal;
            }
        }

        mesh.vertices = currentVertices;
        mesh.RecalculateNormals();

        AddBruise(hitPoint);

        if (hitSound != null)
        {
            AudioSource.PlayClipAtPoint(hitSound, hitPoint);
        }

        if (hitParticlePrefab != null)
        {
            ParticleSystem particles = Instantiate(hitParticlePrefab, hitPoint, Quaternion.LookRotation(normal));
            particles.Play();
            Destroy(particles.gameObject, particleDuration);
        }
    }

    public void ResetToOriginalState()
    {
        currentVertices = (Vector3[])originalVertices.Clone();
        mesh.vertices = currentVertices;
        mesh.RecalculateNormals();
        ClearBruiseTexture();
    }

    private void AddBruise(Vector3 hitPoint)
    {
        if (bruiseTexture == null || bruiseRenderTexture == null)
            return;

        Vector2 uv = GetUVFromHitPoint(hitPoint);
        if (uv == Vector2.zero)
            return;

        RenderTexture.active = bruiseRenderTexture;
        GL.PushMatrix();
        GL.LoadPixelMatrix(0, bruiseRenderTexture.width, bruiseRenderTexture.height, 0);

        float bruisePixelSize = bruiseSize * bruiseRenderTexture.width;
        Rect bruiseRect = new Rect(
            uv.x * bruiseRenderTexture.width - bruisePixelSize * 0.5f,
            uv.y * bruiseRenderTexture.height - bruisePixelSize * 0.5f,
            bruisePixelSize,
            bruisePixelSize
        );

        Graphics.DrawTexture(bruiseRect, bruiseTexture);
        GL.PopMatrix();
        RenderTexture.active = null;
    }

    private void ClearBruiseTexture()
    {
        RenderTexture.active = bruiseRenderTexture;
        GL.Clear(true, true, Color.clear);
        RenderTexture.active = null;
    }

    private Vector2 GetUVFromHitPoint(Vector3 hitPoint)
    {
        Vector3 localHit = transform.InverseTransformPoint(hitPoint);
        int closestVertex = 0;
        float minDistance = float.MaxValue;

        for (int i = 0; i < currentVertices.Length; i++)
        {
            float distance = Vector3.Distance(currentVertices[i], localHit);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestVertex = i;
            }
        }

        return mesh.uv[closestVertex];
    }
}