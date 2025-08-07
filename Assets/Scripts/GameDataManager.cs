using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameDataManager 
{

    public List<int> heros;

    public int Money;
    public GameDataManager() { 
        heros = new List<int>();
        heros.Add(10001);
        heros.Add(10002);
        heros.Add(10003);

    }
}
