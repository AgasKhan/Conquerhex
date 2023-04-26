using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "BaseData/plantilla", fileName = "Base Data")]
public class BaseData : SingletonScript<BaseData>
{
    public static int playerCoins = 0;
    public static string currentWeapon;


    public static Pictionarys<string, ItemBase> storeItems = new Pictionarys<string, ItemBase>();
    public static List<string> playerInventory = new List<string>();
}
