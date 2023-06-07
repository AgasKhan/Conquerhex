using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "BaseData/Biomes", fileName = "Biomes")]
public class Biomes : ShowDetails
{
    public Tile[] tile;
    public int chanceEmptyWeight=100;
    public Pictionarys<GameObject, int> props= new Pictionarys<GameObject, int>();
}