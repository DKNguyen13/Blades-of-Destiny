using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    [Header("Menu")]
    //Main panel
    [SerializeField] private Button continue_btn;
    [SerializeField] private Button exit_btn, close_btn;
    //Choose hero panel
    [SerializeField] private Button back_btn, next_btn;
    [SerializeField] private Button E_Water, E_Fire, E_Leaf;
    [SerializeField] private GameObject menuPanel, choseHeroPanel, informationPanel, inputName;
    [SerializeField] private TextMeshProUGUI playerNameText, playerLevelText, playerHpText, playerStaminaText, playerDmgText, playerDefenseText, playerCritcialRateText, playerCritcialDmgText, playerElement, playerAbility;
    [SerializeField] private TMP_InputField inputField;
    //New game
    [SerializeField] private Button newGame_btn, yes_btn, no_btn,okName_btn;
    [SerializeField] private GameObject newG_pn;

    private string[] dataFile;
    private string filePath, directoryPath;
    private Hero[] heroList;
    private int tmp = 0;
    private string playerName;
    // Start is called before the first frame update
    void Start()
    {
        directoryPath = Application.persistentDataPath;
        filePath = directoryPath + "/gameDataHeroes.json";
        Debug.Log("Đường dẫn lưu file: " + filePath);
        OnClick();
        informationPanel.SetActive(false);
        choseHeroPanel.SetActive(false);
        LoadHeroes();
        Time.timeScale = 1f;
    }
    
    //Choose hero
    private void OnButtonClicked(int index)
    {
        informationPanel.SetActive(true);
        tmp = index;
        try
        {
            Hero hero = heroList[index];

            playerNameText.text = hero.name;
            playerHpText.text = "HP: " + hero.hp;
            playerElement.text = "Element: " + hero.element;
            playerStaminaText.text = "Stamina: " + hero.stamina;
            playerDmgText.text = "Damage: " + hero.damage;
            playerDefenseText.text = "Defense: " + hero.defense;
            playerCritcialRateText.text = "Critical Rate: " + hero.criticalRate + "%";
            playerCritcialDmgText.text = "Critical Dmg: " + hero.criticalDmg + "%";
            playerLevelText.text = "Level: " + hero.level;
            playerAbility.text = hero.ability;
        }
        catch(Exception ex)
        {
            Debug.LogException(ex);
        }
    }

    //Back menu
    public void BackMenu(bool back)
    {
        if(back)
        {
            menuPanel.SetActive(true);
            choseHeroPanel.SetActive(false);
            informationPanel.SetActive(false);
            newG_pn.SetActive(false);
            inputName.SetActive(false);
        }
        else
        {
            dataFile = Directory.GetFiles(directoryPath);
            if(dataFile.Length > 0)
            {
                newG_pn.SetActive(true);
            }
            else
            {
                menuPanel.SetActive(false);
                inputName.SetActive(true) ;
            }
        }
    }

    //Continue game
    public void ContinueGame()
    {
        dataFile = Directory.GetFiles(directoryPath);
        if(dataFile.Length > 0)
        {
            SceneManager.LoadScene("HomeScene");
        }
        else
        {
            inputName.SetActive(true);
        }
    }

    //New game
    public void NextScene()
    {
        Hero[] hero = { heroList[tmp] };
        HeroList heroes = new () { heroes = hero };
        string json = JsonUtility.ToJson(heroes,true);
        File.WriteAllText(filePath, json);
        PlayerPrefs.SetString("playerName",playerName);
        Debug.Log("Hero list saved successfully!");
        SceneManager.LoadScene("HomeScene");
    }
    public void YesNoNewGame(bool x)
    {
        if (x)  //Yes
        {
            foreach (string file in dataFile)
            {
                File.Delete(file);
            }
            menuPanel.SetActive(false);
            inputName.SetActive(true);
        }
        else //No
        {
            newG_pn.SetActive(false);
        }
    }

    public void InputName()
    {
        playerName = inputField.text;
        choseHeroPanel.SetActive(true);
        inputName.SetActive(false);
    }

    //Load hero json
    public void LoadHeroes()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath,"heroes.json");
        /*
        filePath = filePath.Replace("\\", "/");
        Debug.Log("File path: " + filePath);
        Debug.Log("StreamingAssets directory exists: " + Directory.Exists(Application.streamingAssetsPath));
        */
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            // Giải mã JSON thành đối tượng HeroList
            HeroList heroListWrapper = JsonUtility.FromJson<HeroList>(json);

            // Gán mảng heroes từ heroListWrapper vào danh sách heroList
            heroList = heroListWrapper.heroes;
        }
        else
        {
            Debug.LogError("Heroes JSON file not found!");
        }
    }

    public void OnClick()
    {
        newGame_btn.onClick.AddListener(() => BackMenu(false));
        continue_btn.onClick.AddListener(ContinueGame);
        back_btn.onClick.AddListener(() => BackMenu(true));
        close_btn.onClick.AddListener(() => BackMenu(true));
        next_btn.onClick.AddListener(NextScene);
        yes_btn.onClick.AddListener(() => YesNoNewGame(true));
        no_btn.onClick.AddListener(() => YesNoNewGame(false));
        exit_btn.onClick.AddListener(ExitGame);
        E_Water.onClick.AddListener(() => OnButtonClicked(0));  // Index 0
        E_Fire.onClick.AddListener(() => OnButtonClicked(1));   // Index 1
        E_Leaf.onClick.AddListener(() => OnButtonClicked(2));   // Index 2
        okName_btn.onClick.AddListener(InputName);
    }

    //Exit game
    public void ExitGame()
    {
        // Đóng game khi nhấn nút exit
        Application.Quit();

        // Nếu đang trong Unity Editor, bạn có thể dừng play mode bằng cách gọi:
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
