using UnityEngine;
using System.Collections;

public class Heatmap : MonoBehaviour
{
    public Vector4[] positions;
    public Vector4[] properties;

    public int count = 10;

    public float posRange = 50f;
    public float intensityRange = 10f;
    public float radiusRange = 10f;

    private Material material;

    void Start()
    {
        positions = new Vector4[count];
        properties = new Vector4[count];

        material = VisualizationController.Instance.HeatmapMaterial;
    }


    private void OnEnable()
    {
        for (int i = 0; i < positions.Length; i++)
        {
            positions[i] = new Vector4(Random.Range(-posRange,posRange), 0, Random.Range(-posRange,posRange), 0);
            properties[i] = new Vector4(Random.Range(0f, radiusRange), Random.Range(0f, intensityRange), 0, 0);
        }

        InvokeRepeating("UpdateData", 0f, 1f );
    }

    void UpdateData()
    {
        for (int i = 0; i < positions.Length; i++)
            positions[i] += new Vector4(Random.Range(-posRange, posRange), 0, Random.Range(-posRange, posRange), 0) * Time.deltaTime;

        material.SetInt("_Points_Length", count);
        material.SetVectorArray("_Points", positions);
        material.SetVectorArray("_Properties", properties);
    }

    private void OnDisable()
    {
        CancelInvoke("UpdateData");
    }
}
