using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainManager : MonoBehaviour
{
    [System.Serializable]
    public struct Paths
    {
        public Vector3[] points;
        public float width;
    }

    [SerializeField]
    Terrain terrain;
    
    [SerializeField]
    ComputeShader computeShader;

    [SerializeField]
    Paths[] paths;

    ComputeBuffer inputPathBuffer;

    ComputeBuffer outputAlphaBuffer;

    ComputeBuffer outputDetailsBuffer;

    [SerializeField]
    float diference;

    [SerializeField]
    int scale;


    float[,] mapAlphaBuffer;
    float[,,] mapAlpha;


    int[] mapDetailsBuffer;
    int[,] detailsMap;

    TerrainData terrainData;


    private void Awake()
    {
        terrainData = Instantiate(terrain.terrainData);

        terrain.terrainData = terrainData;

        mapAlphaBuffer = new float[terrainData.alphamapWidth * terrainData.alphamapHeight,3];
        mapAlpha = new float[terrainData.alphamapWidth, terrainData.alphamapHeight, 3];


        mapDetailsBuffer = new int[terrainData.detailResolution * terrainData.detailResolution];
        detailsMap = new int[terrainData.detailResolution, terrainData.detailResolution];

    }

    private void OnEnable()
    {
        outputAlphaBuffer = new ComputeBuffer(terrainData.alphamapWidth * terrainData.alphamapHeight, sizeof(float) * 3);

        outputDetailsBuffer = new ComputeBuffer(terrainData.detailResolution * terrainData.detailResolution, sizeof(int));
        //computeShader.SetVector("toSave", vector);

        computeShader.SetFloat("rng", Random.value*1000);
        computeShader.SetFloat("scale", scale);
        computeShader.SetInt("width", terrainData.alphamapWidth);
        computeShader.SetInt("height", terrainData.alphamapHeight);
        computeShader.SetInt("detailResolution", terrainData.detailResolution);

        computeShader.SetBuffer(0, "outputAlphaBuffer", outputAlphaBuffer);
        computeShader.Dispatch(0, Mathf.CeilToInt(terrainData.alphamapWidth / 8f), Mathf.CeilToInt(terrainData.alphamapHeight / 8f), 1);


        for (int i = 0; i < paths.Length; i++)
        {
            inputPathBuffer = new ComputeBuffer(paths[i].points.Length, sizeof(float) * 3);

            inputPathBuffer.SetData(paths[i].points);
            computeShader.SetFloat("diference", (paths[i].width / terrainData.size.x)/2);
            computeShader.SetBuffer(2, "inputPathBuffer", inputPathBuffer);
            computeShader.SetBuffer(2, "outputAlphaBuffer", outputAlphaBuffer);
            
            computeShader.Dispatch(2, Mathf.CeilToInt(terrainData.alphamapWidth / 8f), Mathf.CeilToInt(terrainData.alphamapHeight / 8f), 1);

            inputPathBuffer.Dispose();
        }


        computeShader.SetBuffer(1, "outputDetailsBuffer", outputDetailsBuffer);
        computeShader.SetBuffer(1, "outputAlphaBuffer", outputAlphaBuffer);
        computeShader.Dispatch(1, Mathf.CeilToInt(terrainData.detailResolution / 8f), Mathf.CeilToInt(terrainData.detailResolution / 8f), 1);

        outputDetailsBuffer.GetData(mapDetailsBuffer);
        outputAlphaBuffer.GetData(mapAlphaBuffer);


        Debug.Log( $"{mapAlphaBuffer[0, 0]} {mapAlphaBuffer[0, 1]} {mapAlphaBuffer[0, 2]}");

        Convert();

        terrainData.SetAlphamaps(0, 0, mapAlpha);

        terrainData.SetDetailLayer(0, 0, 0, detailsMap);

        outputAlphaBuffer.Dispose();

        outputDetailsBuffer.Dispose();

        Debug.Log($"{terrainData.detailResolution} {terrainData.alphamapResolution} {terrainData.alphamapWidth} {terrainData.size}");
    }

    void Convert()
    {
        for (int i = 0; i < mapAlphaBuffer.GetLength(0); i++)
        {
            int x = i % mapAlpha.GetLength(0);
            int y = i / mapAlpha.GetLength(0);

            mapAlpha[x, y, 0] = mapAlphaBuffer[i, 0];
            mapAlpha[x, y, 1] = mapAlphaBuffer[i, 1];
            mapAlpha[x, y, 2] = mapAlphaBuffer[i, 2];
        }

        for (int i = 0; i < mapDetailsBuffer.Length; i++)
        {
            int x = i % detailsMap.GetLength(0);
            int y = i / detailsMap.GetLength(0);

            if (mapDetailsBuffer[i] == 0)
            {
                detailsMap[x, y] = 0;
                continue;
            }
            
            detailsMap[x, y] = Random.Range(1, 10);
        }
    }
}
