using System;

[Serializable]
public class Hero
{
    public string name;
    public int level;
    public string element;
    public int hp;
    public int stamina;
    public int damage;
    public int defense;
    public int criticalDmg;
    public int criticalRate;
    public string ability;
}

[Serializable]
public class HeroList
{
    public Hero[] heroes;
}