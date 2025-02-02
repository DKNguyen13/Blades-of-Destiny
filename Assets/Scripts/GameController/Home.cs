using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Home : MonoBehaviour
{
    [SerializeField] private Button mainmenu_btn, battle_btn, exitBtl_btn, hero_btn, closeHero_btn;
    [SerializeField] private GameObject battlePanel, menuPanel, mapEnemyPanel, heroPanel;

    [Header("State's hero")]
    [SerializeField] private Animator battlePanelAnimator;
    [SerializeField] private TextMeshProUGUI point_txt, hp_txt, stamina_txt, level_txt, dmg_txt, defense_txt, cR_txt, cDmg_txt;
    [SerializeField] private AudioClip sumer;
    [SerializeField] private List<Button> buttonList = new();
    private AudioClip tiecTraSao;
    private AudioSource au;
    private HeroList herolist;
    private int point = 0, hp, stamina, level = 1, damage, defense, criticalRate, criticalDmg;
    private GameData gameData;
    private bool isUpdate = false;

    public void Awake()
    {
        au = GetComponent<AudioSource>();
        battlePanelAnimator = battlePanelAnimator.GetComponent<Animator>();
        gameData = LoadData();
        tiecTraSao = au.clip;
    }

    // Start is called before the first frame update
    void Start()
    {
        LoadHeroData();
        OnClick();
        Time.timeScale = 1f;
    }

    public void BackMenu()
    {
        SceneManager.LoadScene("MenuScene");
    }    

    public IEnumerator BattleScene()
    {
        PlayerSound(sumer);
        menuPanel.SetActive(false);
        battlePanel.SetActive(true);
        AnimatorClipInfo[] clipInfo = battlePanelAnimator.GetCurrentAnimatorClipInfo(0);
        float animationDuration = clipInfo[0].clip.length;

        yield return new WaitForSeconds(animationDuration + 0.2f);
        exitBtl_btn.gameObject.SetActive(true);
        mapEnemyPanel.SetActive(true);
    }

    public IEnumerator CloseBattleMap()
    {
        mapEnemyPanel.SetActive(false);
        exitBtl_btn.gameObject.SetActive(false);
        battlePanelAnimator.Play("Disappear");

        // Đợi frame tiếp theo để chắc chắn animation bắt đầu play
        yield return null;

        // Lấy thông tin state của animation hiện tại trong layer 0
        AnimatorStateInfo stateInfo = battlePanelAnimator.GetCurrentAnimatorStateInfo(0);

        // Kiểm tra nếu animation "Disappear" đang được phát
        if (stateInfo.IsName("Disappear"))
        {
            float animationDuration = stateInfo.length; // Thời gian chạy animation
            yield return new WaitForSeconds(animationDuration); // Đợi animation hoàn thành
        }
        else
        {
            Debug.LogWarning("Animation 'Disappear' không được phát.");
            yield return new WaitForSeconds(0.2f); // Thời gian fallback
        }
        menuPanel.SetActive(true);
        battlePanel.SetActive(false);
        PlayerSound(tiecTraSao);
    }

    public void HeroPanel(bool x)
    {
        heroPanel.SetActive(x);
        menuPanel.SetActive(!x);
        if (!x && isUpdate)
        {
            isUpdate = false;
            SaveData();
        }
    }
    private void PlayerSound(AudioClip a)
    {
        au.clip = a;
        au.Play();
    }

    public void LoadHeroData()
    {
        string filePath = Application.persistentDataPath + "/gameDataHeroes.json";
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            herolist = JsonUtility.FromJson<HeroList>(json);
            /*
            foreach(Hero hero in herolist.heroes)
            {

            }
            */
            Hero hero = herolist.heroes[0];
            hp = hero.hp;
            stamina = hero.stamina;
            damage = hero.damage;
            defense = hero.defense;
            criticalDmg = hero.criticalDmg;
            criticalRate = hero.criticalRate;
            SaveStatePlayerPrebs();
            UpdateUI();
        }
        else
        {
            SceneManager.LoadScene("MenuScene");
            return;
        }
    }

    //Load gameData.json
    public GameData LoadData()
    {
        string filePath = Application.persistentDataPath + "/gameData.json";
        if (File.Exists(filePath))
        {
            //Load game data
            string json = File.ReadAllText(filePath);
            gameData = JsonUtility.FromJson<GameData>(json);
            level = gameData.level;
            point = gameData.point;
            return gameData;
        }
        else
        {
            GameData gameData = new()
            {
                playerName = PlayerPrefs.GetString("playerName")
            };
            PlayerPrefs.DeleteKey("playerName");
            string json = JsonUtility.ToJson(gameData);
            File.WriteAllText(filePath, json);
            return gameData;
        }

    }

    private void UpdateUI()
    {
        point_txt.text = $"Point: {point}";
        hp_txt.text = $"Hp: {hp}";
        stamina_txt.text = $"Sta: {stamina}";
        level_txt.text = $"Level: {level}";
        dmg_txt.text = $"Dmg: {damage}";
        defense_txt.text = $"Def: {defense}";
        cDmg_txt.text = $"CDmg: {criticalDmg} %";
        cR_txt.text = $"CR: {criticalRate} %";
    }

    public void OnClick()
    {
        mainmenu_btn.onClick.AddListener(BackMenu);
        battle_btn.onClick.AddListener(() => StartCoroutine(BattleScene()));
        exitBtl_btn.onClick.AddListener(() => StartCoroutine(CloseBattleMap()));
        closeHero_btn.onClick.AddListener(() => HeroPanel(false));
        hero_btn.onClick.AddListener(() => HeroPanel(true));
        for(int i = 0; i< buttonList.Count; i++)
        {
            int index = i;
            buttonList[i].onClick.AddListener(() => UpdateState(index));
            if(point == 0)
            {
                buttonList[i].gameObject.SetActive(false);
            }
        }
    }

    public void UpdateState(int index)
    {
        point -= 1;
        isUpdate = true;
        if (index == 0)
        {
            hp += (int)(hp * 0.2f);
            Debug.Log("HP la: "+hp);
            hp_txt.text = $"Hp: {hp}";
        }
        else if(index == 1)
        {
            stamina += (int)(stamina * 0.2f);
            stamina_txt.text = "Sta: " + stamina.ToString();
        }
        else if (index == 2)
        {
            damage += (int)(damage * 0.2f);
            dmg_txt.text = $"Dmg: {damage}";
        }
        else if (index == 3)
        {
            defense += 3;
            defense_txt.text = $"Def: {defense}";
        }
        else if (index == 4)
        {
            criticalDmg += 1;
            cDmg_txt.text = $"CDmg: {criticalDmg}";
        }
        else if (index == 5)
        {
            criticalRate += 1;
            cR_txt.text = $"CR: {criticalRate}";
        }
        point_txt.text = $"Point: {point}";
        SaveStatePlayerPrebs();
        if (point == 0)
        {
            foreach (Button i in buttonList)
            {
                i.gameObject.SetActive(false);
            }
        }
    }

    //Save data
    public void SaveData()
    {
        string filePath1 = Application.persistentDataPath + "/gameDataHeroes.json";
        string filePath2 = Application.persistentDataPath + "/gameData.json";
        if (File.Exists(filePath1) && File.Exists(filePath2))
        {
            gameData.point = point;
            if(herolist.heroes[0].level < gameData.level)
            {
                herolist.heroes[0].level = gameData.level;
            }
            herolist.heroes[0].hp = hp;
            herolist.heroes[0].stamina = stamina;
            herolist.heroes[0].defense = defense;
            herolist.heroes[0].damage = damage;
            herolist.heroes[0].criticalDmg = criticalDmg;
            herolist.heroes[0].criticalRate = criticalRate;
            string json1 = JsonUtility.ToJson(herolist,true);
            string json2 = JsonUtility.ToJson(gameData,true);
            File.WriteAllText(filePath1, json1);
            File.WriteAllText(filePath2, json2);
            Debug.Log("Dữ liệu đã được lưu thành công!");
        }
        else
        {
            SceneManager.LoadScene("MenuScene");
            return;
        }
    }
    public void SaveStatePlayerPrebs()
    {
        PlayerPrefs.SetInt("Hero_Level", level);
        PlayerPrefs.SetInt("Hero_HP", hp);
        PlayerPrefs.SetInt("Hero_Stamina", stamina);
        PlayerPrefs.SetInt("Hero_Defense", defense);
        PlayerPrefs.SetInt("Hero_Damage", damage);
        PlayerPrefs.SetInt("Hero_CriticalDmg", criticalDmg);
        PlayerPrefs.SetInt("Hero_CriticalRate", criticalRate);
        PlayerPrefs.SetInt("CurrentMap", gameData.currentMap);
        //PlayerPrefs.Save();
    }
}
