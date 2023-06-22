using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "BaseData/Biomes", fileName = "Biomes")]
public class Biomes : ShowDetails
{
    public Tile[] tile;
    public int chanceEmptyOrEnemy=10;
    [Tooltip("Representa cuantas casillas se salta para colocar objetos, en caso de ser 0 no se saltara ninguna")]
    public int inversaDensidad = 3;
    public Spawner spawner;
    public Pictionarys<GameObject, int> props= new Pictionarys<GameObject, int>();
}