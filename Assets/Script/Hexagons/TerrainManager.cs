using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainManager : MonoBehaviour
{
    [System.Serializable]
    public class Paths
    {
        public bool toCenterPath = false;
        public int[] points= new int[6];
        public float width;
    }

    public TerrainData copyData;

    [SerializeField]
    Terrain terrain;
    
    [SerializeField]
    ComputeShader computeShader;

    [SerializeField]
    public Paths[] paths;

    ComputeBuffer inputPathBuffer;

    ComputeBuffer outputAlphaBuffer;

    ComputeBuffer outputDetailsBuffer;

    ComputeBuffer inputVertex;

    [SerializeField]
    float diference;

    [SerializeField]
    int scale;

    float[,] mapAlphaBuffer;
    float[,,] mapAlpha;

    int[] mapDetailsBuffer;
    int[,] grassMap;
    public int[,] detailsMap;

    TerrainData terrainData;

    int[,] vertexShader { get => HexagonsManager.vertexShader; set => HexagonsManager.vertexShader = value; }

    int[,] apotemaShader { get => HexagonsManager.apotemaShader; set => HexagonsManager.apotemaShader = value; }

    private void Awake()
    {
        if (copyData == null)
            return;
        terrainData = Instantiate(copyData);

        terrain.terrainData = terrainData;

        mapAlphaBuffer = new float[terrainData.alphamapWidth * terrainData.alphamapHeight, terrainData.alphamapLayers];
        mapAlpha = new float[terrainData.alphamapWidth, terrainData.alphamapHeight, terrainData.alphamapLayers];


        mapDetailsBuffer = new int[terrainData.detailResolution * terrainData.detailResolution];
        grassMap = new int[terrainData.detailResolution, terrainData.detailResolution];
        detailsMap = new int[terrainData.detailResolution, terrainData.detailResolution];
    }

    
    [ContextMenu("Generar")]
    public void Generate()
    {
        inputVertex = new ComputeBuffer(6, sizeof(int)*2);

        outputAlphaBuffer = new ComputeBuffer(terrainData.alphamapWidth * terrainData.alphamapHeight, sizeof(float) * terrainData.alphamapLayers);

        outputDetailsBuffer = new ComputeBuffer(terrainData.detailResolution * terrainData.detailResolution, sizeof(int));

        if(vertexShader==null)
        {
            var aux = HexagonsManager.LocalRadio();
            vertexShader = new int[6, 2];

            for (int i = 0; i < aux.GetLength(0); i++)
            {
                vertexShader[i, 0] = Mathf.CeilToInt((aux[i, 0] / 100) * terrainData.alphamapWidth + terrainData.alphamapWidth/2);
                vertexShader[i, 1] = Mathf.CeilToInt((aux[i, 1] / 100) * terrainData.alphamapHeight + terrainData.alphamapHeight/2);
            }
        }

        if (apotemaShader == null)
        {
            var aux = HexagonsManager.LocalApotema();
            apotemaShader = new int[6, 2];

            for (int i = 0; i < aux.GetLength(0); i++)
            {
                apotemaShader[i, 0] = Mathf.CeilToInt((aux[i, 0] / 100) * terrainData.alphamapWidth + terrainData.alphamapWidth / 2);
                apotemaShader[i, 1] = Mathf.CeilToInt((aux[i, 1] / 100) * terrainData.alphamapHeight + terrainData.alphamapHeight / 2);
            }
        }

        //computeShader.SetVector("toSave", vector);

        inputVertex.SetData(apotemaShader);
        computeShader.SetFloat("rng", Random.value*1000);
        computeShader.SetFloat("scale", scale);
        computeShader.SetInt("pathEntry", Random.Range(0,6));
        computeShader.SetInt("width", terrainData.alphamapWidth);
        computeShader.SetInt("height", terrainData.alphamapHeight);
        computeShader.SetInt("detailResolution", terrainData.detailResolution);

        computeShader.SetBuffer(0, "inputVertex", inputVertex);
        computeShader.SetBuffer(0, "outputAlphaBuffer", outputAlphaBuffer);
        computeShader.Dispatch(0, Mathf.CeilToInt(terrainData.alphamapWidth / 8f), Mathf.CeilToInt(terrainData.alphamapHeight / 8f), 1);


        for (int i = 0; i < paths.Length; i++)
        {
            inputPathBuffer = new ComputeBuffer(paths[i].points.Length, sizeof(int));

            inputPathBuffer.SetData(paths[i].points);
            //inputVertex.SetData(apotemaShader);

            computeShader.SetFloat("diference", (paths[i].width / terrainData.size.x)/2);
            computeShader.SetBool("toCenterPath", paths[i].toCenterPath);

            computeShader.SetBuffer(2, "inputVertex", inputVertex);
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


        //Debug.Log( $"{mapAlphaBuffer[0, 0]} {mapAlphaBuffer[0, 1]} {mapAlphaBuffer[0, 2]}");

        Convert();

        terrainData.SetAlphamaps(0, 0, mapAlpha);

        terrainData.SetDetailLayer(0, 0, 0, grassMap);

        outputAlphaBuffer.Dispose();

        outputDetailsBuffer.Dispose();

        inputVertex.Dispose();

        //Debug.Log($"{terrainData.detailResolution} {terrainData.alphamapResolution} {terrainData.alphamapWidth} {terrainData.size}");
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
            mapAlpha[x, y, 3] = mapAlphaBuffer[i, 3];
        }

        for (int i = 0; i < mapDetailsBuffer.Length; i++)
        {
            int x = i % grassMap.GetLength(0);
            int y = i / grassMap.GetLength(0);

            detailsMap[x, y] = mapDetailsBuffer[i];

            if (mapDetailsBuffer[i] == 1)
            {
                grassMap[x, y] = 10;
                //grassMap[x, y] = Random.Range(1, 10);
                continue;
            }

            grassMap[x, y] = 0;
        }
    }
}
