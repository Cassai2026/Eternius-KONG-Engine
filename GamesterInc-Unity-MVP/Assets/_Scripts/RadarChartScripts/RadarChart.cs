using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class RadarChart : MonoBehaviour
{
    [SerializeField] private TMP_Text statText;
    [SerializeField] private Material radarMaterial;
    [SerializeField] private CanvasRenderer radarMeshCanvasRenderer;
    [SerializeField] float radarChartSize = 42f;
    [SerializeField] private float raderAnimSpeed;
    [SerializeField] float duration = 4;

    private Stats stats;
    private TMP_Text[] statTexts = new TMP_Text[0];
    private IEnumerator scaleRoutine = null;
    
    public void SetStats(Stats stats)
    {
        this.stats = stats;
        SetupText();
        UpdateStatsList();
        if (scaleRoutine is not null)
        {
            StopCoroutine(scaleRoutine);
        }
        scaleRoutine = LerpStats();
        StartCoroutine(scaleRoutine);
    }

    private void UpdateStatsList()
    {
        Mesh mesh = new Mesh();
        int statAmount = stats.GetStatsLength();

        if (statAmount < 3)
        {
            Debug.LogError("STAT AMOUNT NEEDS TO BE LARGER THEN 3");
            return;
        }

        Vector3[] vertices = new Vector3[statAmount + 1];
        Vector2[] uv = new Vector2[statAmount + 1];
        int[] triangles = new int[3 * statAmount];

        float angleIncrement = -360f / statAmount;
        
        vertices[0] = Vector3.zero;

        for (int i = 0; i < statAmount; i++)
        {
            Vector3 statVertex = Quaternion.Euler(0,0, angleIncrement * i) * Vector3.up * (radarChartSize * stats.GetStatAmountNormalized(i));
            int statIndex = i+1;
            vertices[statIndex] = statVertex;

            triangles[3 * i] = 0;
            triangles[3 * i + 1] = statIndex;
            triangles[3 * i + 2] = statIndex + 1;
        }
        
        triangles[triangles.Length-1] = 1;
        
        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
        
        radarMeshCanvasRenderer.SetMesh(mesh);
        radarMeshCanvasRenderer.SetMaterial(radarMaterial, null);
    }

    private IEnumerator LerpStats()
    {
        radarMeshCanvasRenderer.transform.localScale = Vector3.zero;
        float timePassed = 0f;
        float radarChartLerp = 0;
        while (radarChartLerp < radarChartSize)
        {
            timePassed += raderAnimSpeed * Time.deltaTime;
            radarChartLerp = Mathf.Sin((1 / (duration * 2)) * Mathf.PI * timePassed ) * radarChartSize + 0.2f;
            radarMeshCanvasRenderer.transform.localScale = Vector3.one * (radarChartLerp / radarChartSize);
            yield return null;
        }
    }

    private void SetupText()
    {
        int statAmount = stats.GetStatsLength();
        float angleIncrement = -360f / statAmount;
        statText.text = $"{stats.GetStatName(0)}";
        statText.transform.localPosition = Vector3.up * (radarChartSize + 5);

        if (statTexts.Length > 0)
        {
            for (int i = statTexts.Length - 1; i >= 0; i--)
            {
                if (statTexts[i] is not null)
                    Destroy(statTexts[i].gameObject);
            }
        }
        statTexts = new TMP_Text [statAmount];

        for (int i = 0; i < statAmount; i++)
        {
            TMP_Text text = Instantiate(statText, transform);
            text.transform.localPosition = Quaternion.Euler(0,0, angleIncrement * i) * Vector3.up * (radarChartSize + 15);
            
            text.text = $"{stats.GetStatName(i)}";
            statTexts[i] = text;
        }
    }
}
