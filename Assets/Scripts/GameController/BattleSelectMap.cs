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
        string filePath = Application.persistentDataPath + "/gameDataHeroes.json";
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
}
