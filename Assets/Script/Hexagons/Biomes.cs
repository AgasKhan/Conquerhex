using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "BaseData/Biomes", fileName = "Biomes")]
public class Biomes : ShowDetails
{
    public Tile[] tile;
    public int chanceEmptyOrEnemy=10;
    public Spawner spawner;
    public Pictionarys<GameObject, int> props= new Pictionarys<GameObject, int>();
}