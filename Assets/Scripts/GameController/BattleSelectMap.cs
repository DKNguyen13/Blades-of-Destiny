using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;
using System.Collections;

public class BattleSelectMap : MonoBehaviour
{
    [Header("Map")]
    [SerializeField] private List<Button> buttonMapList;
    [SerializeField] private Sprite[] sprite;
    [SerializeField] Image imageHero;
    [SerializeField] private Image[] imageSkill;
    [SerializeField] private Sprite[] spriteSkill;
    [SerializeField] private TMPro.TextMeshProUGUI [] describeSkill;
    private int currentLvl = 0, hero;
    private Hero[] heroes;

    private void Start()
    {
        for (int i = 0; i < buttonMapList.Count; i++)
        {
            int index = i + 1;
            buttonMapList[i].onClick.AddListener(() => BattleMap(index));
        }
        heroes = LoadDataHero();
        currentLvl = PlayerPrefs.GetInt("CurrentMap");

        hero = heroes[0].name switch
        {
            "E_Water" => 0,
            "E_Fire" => 1,
            "E_Ground" => 2,
            _ => 0
        };
        imageHero.sprite = sprite[hero];
        StartCoroutine(ShowMap());
    }

    public IEnumerator ShowMap()
    {
        yield return null;
        for(int i = 0; i <= currentLvl; i++)
        {
            buttonMapList[i].gameObject.SetActive(true);
        }
        ShowSkillDescribe();
    }

    public void BattleMap(int lvl)
    {
        try
        {
            Debug.Log("Level "+ lvl);
            PlayerPrefs.SetInt("lvl", lvl);
            PlayerPrefs.SetInt("E_hero", hero);
            //PlayerPrefs.Save();//Luu vinh vien trong unity lien ngay khi goi
            SceneManager.LoadScene("Scene1");
        }
        catch(Exception ex)
        {
            Debug.LogException(ex);
        }
    }

    //Load default hero
    public Hero[] LoadDataHero()
    {
        string filePath = Path.Combine(Application.persistentDataPath, "gameDataHeroes.json");
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            HeroList heroList = JsonUtility.FromJson<HeroList>(json);
            if (heroList != null)
            {
                return heroList.heroes;
            }
            else
            {
                Debug.Log("Failed to load hero data");
                return null;
            }
        }
        else
        {
            Debug.Log("No hero data");
            return null ;
        }
    }
    public void ShowSkillDescribe()
    {
        if (hero == 0)
        {
            for(int i = 0; i < 4; i++)
            {
                imageSkill[i].sprite = spriteSkill[i];
            }
            describeSkill[0].text += " Healing";
            describeSkill[1].text += " Twin Blade Strike";
            describeSkill[2].text += " Aqua Burst Combo";
            describeSkill[3].text += " Icicle Salvation";
            describeSkill[4].text = "Restores 30% of your initial health. Remove all toxic statuses";
            describeSkill[5].text = "Use sword to attack target twice (dealing 130% damage each time).";
            describeSkill[6].text = "Perform 2 consecutive attacks then create a water push that knocks the target up (dealing 140% damage each time).";
            describeSkill[7].text = "Creates a water sphere and condenses it into ice arrows that attack, dealing 160% damage to the target. (Has a 40% chance to freeze).";
        }
        else if(hero == 1)
        {
            for(int i = 0; i <4; i++)
            {
                imageSkill[i].sprite = spriteSkill[i+4];
            }
            describeSkill[0].text += " Swift Slash";
            describeSkill[1].text += " Cyclone Slash";
            describeSkill[2].text += " Infernal Tempest";
            describeSkill[3].text += " Blazing Judgement";
            describeSkill[4].text = "Immediately swings sword horizontally, dealing 110% damage to target.";
            describeSkill[5].text = "Throw 3 consecutive punches and 1 spinning kick, each dealing 120% damage to the opponent (has a 10% chance to stun).";
            describeSkill[6].text = "Swings sword and creates a tornado that deals continuous damage and eventually creates a fireball that deals 80% damage to the target. (Has a 5% chance to burn the target).";
            describeSkill[7].text = "Pour all the flames of power and rage into the sword and swing it towards the opponent, dealing 160% damage (Has a 40% chance to burn the opponent).";
        }
        else if(hero==2)
        {
            for (int i = 0; i < 4; i++)
            {
                imageSkill[i].sprite = spriteSkill[i + 8];
            }
            describeSkill[0].text += " Charged Barrage";
            describeSkill[1].text += " Earthshaker Combo";
            describeSkill[2].text += " Stone Ascend";
            describeSkill[3].text += " Colossal Fist";
            describeSkill[4].text = "Charge up to unleash 3 consecutive punches that deal damage to enemies (each punch deals 120% damage).";
            describeSkill[5].text = "Slash down on the target, then spin continuously creating a continuous tornado dealing 70% damage to the target.";
            describeSkill[6].text = "Throw 3 punches in a row and dash up to knock up a moving boulder (dealing 130% damage per hit).";
            describeSkill[7].text = "Summons a giant stone hand to restrain the opponent and summons a punch from behind to attack, dealing 150% damage to the enemy.";
        }
    }
}
