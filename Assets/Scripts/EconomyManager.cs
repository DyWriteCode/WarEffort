using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FactorySystem
{
    public class EconomyManager : BaseManager
    {
        public EconomyManager(float initialMoney) 
        {
            Money = initialMoney;
        }    

        public float Money = 0;

        public bool CanAfford(int amount)
        {
            return Money >= amount;
        }

        public void AddMoney(int amount)
        {
            Money += amount;
        }

        public bool SpendMoney(int amount)
        {
            if (!CanAfford(amount)) return false;
            Money -= amount;
            return true;
        }
    }
}
