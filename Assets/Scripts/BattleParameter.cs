using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


[System.Serializable]
public class BattleParameterBase
{
    [Min(1)] public int HP;
    [Min(1)] public int MaxHP;

    [Min(1)] public int MP;
    [Min(1)] public int MaxMP;

    [Min(1)] public int power;
    [Min(1)] public int technique;

    [Min(1)] public int prayer;
    [Min(1)] public int agility;

    [Min(1)] public int luck;

    [Min(1)] public int Attackpower;
    [Min(1)] public int Defensepower;

    [Min(1)] public int Level;
    [Min(0)] public int Exp;
    [Min(0)] public int Money;

    public Weapon AttackWeapon;
    public Weapon DefenseWeapon;

    public bool IsLimitItemCount { get => Items.Count >= 4; }
    public List<Item> Items; //ãŒÀ4ŒÂ‚Ü‚Å‚ð‘z’è‚µ‚Ä‘¼‚Ì‚à‚Ì‚ðì¬‚µ‚Ä‚¢‚éB

    public int AttackPower { get => power + (AttackWeapon != null ? AttackWeapon.Power : 0); }
    public int DefensePower { get => power + (DefenseWeapon != null ? DefenseWeapon.Power : 0); }

    public bool IsNowDefense { get; set; } = false;

    [Min(1)] public int LimitHP = 500;
    [Min(1)] public int LimitMP = 300;
    [Min(1)] public int Limitpower = 200;
    [Min(1)] public int Limittechnique = 100;
    [Min(1)] public int Limitprayer = 200;
    [Min(1)] public int Limitagility = 100;
    [Min(1)] public int Limitluck = 100;


    public void AdjustParamWithLevel()
    {
        Level = Exp / 100;
        MaxHP = (int)(LimitHP * Level / 100f);
        MaxMP = (int)(LimitMP * Level / 100f);
        power = (int)(Limitpower * Level / 100f);
        technique = (int)(Limittechnique * Level / 100f);
        prayer = (int)(Limitprayer * Level / 100f);
        agility = (int)(Limitagility * Level / 100f);
        luck = (int)(Limitluck * Level / 100f);
    }

    public virtual void CopyTo(BattleParameterBase dest)
    {
        dest.HP = HP;
        dest.MaxHP = HP < MaxHP ? MaxHP : HP;
        dest.MP = MP;
        dest.MaxMP = MP < MaxMP ? MaxMP : MP;
        dest.power = power;
        dest.technique = technique;
        dest.prayer = prayer;
        dest.agility = agility;
        dest.luck = luck;
        dest.Level = Level;
        dest.Exp = Exp;
        dest.Money = Money;

        dest.AttackWeapon = AttackWeapon;
        dest.DefenseWeapon = DefenseWeapon;

        dest.Items = new List<Item>(Items.ToArray());

        dest.LimitHP = LimitHP;
        dest.LimitMP = LimitMP;
        dest.Limitpower = Limitpower;
        dest.Limittechnique = Limittechnique;
        dest.Limitprayer = Limitprayer;
        dest.Limitagility = Limitagility;
        dest.Limitluck = Limitluck;
    }

    public class AttackResult
    {
        public int LeaveHP;
        public int Damage;
    }
    public virtual bool AttackTo(BattleParameterBase target, out AttackResult result)
    {
        result = new AttackResult();

        result.Damage = Mathf.Max(0, AttackPower - target.DefensePower);
        if (target.IsNowDefense)
        {
            result.Damage /= 2;
        }
        target.HP -= result.Damage;
        result.LeaveHP = target.HP;
        return target.HP <= 0;
    }

    public bool GetExp(int exp)
    {
        Exp += exp;

        if (Exp >= (Level + 1) * 100)
        {
            AdjustParamWithLevel();
            return true;
        }

        return false;
    }

}


[System.Serializable]
public class BattleParameterBaseSaveData
{
    public string paramJson;
    public int attackWeaponIndex;
    public int defenseWeaponIndex;
    public int[] itemsIndex;

    public BattleParameterBaseSaveData(BattleParameterBase param, ItemList itemList)
    {
        paramJson = JsonUtility.ToJson(param);
        attackWeaponIndex = itemList.FindIndex(param.AttackWeapon);
        defenseWeaponIndex = itemList.FindIndex(param.DefenseWeapon);
        itemsIndex = new int[param.Items.Count];
        for (var i = 0; i < itemsIndex.Length; ++i)
        {
            itemsIndex[i] = itemList.FindIndex(param.Items[i]);
        }
    }

    public BattleParameterBase Load(ItemList itemList)
    {
        var inst = JsonUtility.FromJson<BattleParameterBase>(paramJson);

        inst.AttackWeapon = itemList[attackWeaponIndex] as Weapon;
        inst.DefenseWeapon = itemList[defenseWeaponIndex] as Weapon;
        inst.Items = new List<Item>(itemsIndex.Length);
        for (var i = 0; i < itemsIndex.Length; ++i)
        {
            inst.Items.Add(itemList[itemsIndex[i]]);
        }
        return inst;
    }
}

[CreateAssetMenu(menuName = "BattleParameter")]
public class BattleParameter : ScriptableObject
{
    public BattleParameterBase Data;
}
