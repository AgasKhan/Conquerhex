using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

[CreateAssetMenu(menuName = "BaseData/Biomes", fileName = "Biomes")]
public class Biomes : ShowDetails
{
    [System.Serializable]
    public struct LayersOfProps
    {
        public int chanceEmptyOrEnemy;
        [Tooltip("Representa cuantas casillas se salta para colocar objetos, en caso de ser 0 no se saltara ninguna")]
        public int inversaDensidad;
        public Spawner spawner;
        public Pictionarys<GameObject, int> props;
    }

    public enum LayersNames
    {
        Trees,
        Enemies,
        Paths
    }

    public Tile[] tile;
    public TerrainData terrainBiome;

    [Header("Orden de las layers:\n\tPrimero: Trees\n\tSegundo: Enemies\n\tTercero: Paths\n\nPara el sistema viejo toma: \n\tchanceEmptyOrEnemy: Enemies\n\tinversaDensidad: trees\n\tspawner: Enemies")]
    [Space]
    public LayersOfProps[] layersOfProps = new LayersOfProps[3];

    [Tooltip("Quedara obsoleto cuando se traspase de sistema, en su lugar trabajar con LayersOfProps")]
    public Pictionarys<GameObject, int> props
    {
        get
        {
            if(_props==null)
                _props = layersOfProps.SelectMany((lOfProps) => lOfProps.props).ToPictionarys();
            return layersOfProps.SelectMany((lOfProps) => lOfProps.props).ToPictionarys();
        }
    }

    Pictionarys<GameObject, int> _props;

    [ContextMenu("Cargar assets de la carpeta")]
    void LoadAssets()
    {
        string path = BaseData.pathProps + "Common" + "/";

        for (int i = 0; i < layersOfProps.Length; i++)
        {
            string pathFolder = path + System.Enum.GetNames(typeof(LayersNames))[i] + "/";

            Debug.Log(pathFolder);

            foreach (var item in LoadSystem.LoadAssets<GameObject>(pathFolder))
            {
                if (!layersOfProps[i].props.ContainsKey(item))
                    layersOfProps[i].props.Add(item, 10);
            }
        }

        path = BaseData.pathProps + nameDisplay + "/";

        for (int i = 0; i < layersOfProps.Length; i++)
        {
            string pathFolder = path + System.Enum.GetNames(typeof(LayersNames))[i] + "/";

            Debug.Log(pathFolder);

            foreach (var item in LoadSystem.LoadAssets<GameObject>(pathFolder))
            {
                if (!layersOfProps[i].props.ContainsKey(item))
                    layersOfProps[i].props.Add(item, 10);
            }
        }
    }

}

