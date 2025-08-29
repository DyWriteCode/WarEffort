using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// public class TestCondition : MonoBehaviour
[Serializable]
public class TestCondition
{
    public enum Category { Weapons, Armor, Potions }
    public enum Rarity { Common, Rare, Epic, Legendary }

    public Category itemCategory;
    public Rarity itemRarity;

    // 仅当类别为武器且稀有度为史诗或传奇时显示
    [EnumConditionalHide("itemCategory", Category.Weapons)]
    public int weaponDamage;

    // 仅当类别为盔甲时显示
    [EnumConditionalHide("itemCategory", Category.Armor)]
    public int armorDefense;

    // 仅当类别为药水时显示
    [EnumConditionalHide("itemCategory", Category.Potions)]
    public float potionDuration;
}
