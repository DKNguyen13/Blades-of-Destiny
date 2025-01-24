using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System;
using UnityEngine.SceneManagement;
using UnityEditor;

public class Menu : MonoBehaviour
{
    [Header("Menu")]
    //Main panel
    [SerializeField] private Button continue_btn;
    [SerializeField] private Button exit_btn;

    //Choose hero panel
    [SerializeField] private Button back_btn;
    [SerializeField] private Button next_btn;
    [SerializeField] private Button E_Water;
    [SerializeField] private Button E_Fire;
    [SerializeField] private Button E_Leaf;

    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject choseHeroPanel;
    [SerializeField] private GameObject informationPanel;
    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private TextMeshProUGUI playerLevelText;
    [SerializeField] private TextMeshProUGUI playerHpText;
    [SerializeField] private TextMeshProUGUI playerStaminaText;
    [SerializeField] private TextMeshProUGUI playerDmgText;
    [SerializeField] private TextMeshProUGUI playerDefenseText;
    [SerializeField] private TextMeshProUGUI playerCritcialRateText;
    [SerializeField] private TextMeshProUGUI playerCritcialDmgText;
    [SerializeField] private TextMeshProUGUI playerElement;
    [SerializeField] private TextMeshProUGUI playerAbility;

    //New game
    [SerializeField] private Button newGame_btn;
    [SerializeField] private Button yes_btn;
    [SerializeField] private Button no_btn;
    [SerializeField] private GameObject newG_pn;

    private string[] dataFile;
    private string filePath;
    private Hero[] heroList;
    string directoryPath;
    private int tmp = 0;

    // Start is called before the first frame update
    void Start()
    {
        directoryPath = Application.persistentDataPath;
        filePath = directoryPath + "/gameDataHeroes.json";
        Debug.Log("Đường dẫn lưu file: " + filePath);
        newGame_btn.onClick.AddListener(() => BackMenu(false));
        continue_btn.onClick.AddListener(ContinueGame);
        back_btn.onClick.AddListener(() => BackMenu(true));
        next_btn.onClick.AddListener(NextScene);
        yes_btn.onClick.AddListener(() => YesNoNewGame(true));
        no_btn.onClick.AddListener(() => YesNoNewGame(false));
        exit_btn.onClick.AddListener(ExitGame);
        E_Water.onClick.AddListener(() => OnButtonClicked(0));  // Index 0
        E_Fire.onClick.AddListener(() => OnButtonClicked(1));   // Index 1
        E_Leaf.onClick.AddListener(() => OnButtonClicked(2));   // Index 2
        informationPanel.SetActive(false);
        choseHeroPanel.SetActive(false);
        LoadHeroes();
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
                choseHeroPanel.SetActive(true);
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
            choseHeroPanel.SetActive(true);
        }
    }

    //New game
    public void NextScene()
    {
        Hero[] hero = { heroList[tmp] };
        HeroList heroes = new HeroList { heroes = hero };
        string json = JsonUtility.ToJson(heroes,true);
        File.WriteAllText(filePath, json);
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
            choseHeroPanel.SetActive(true);
        }
        else //No
        {
            newG_pn.SetActive(false);
        }
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


    /*
private bool ConfirmNewGame()
{
    return EditorUtility.DisplayDialog(
        "New Game",
        "Are you sure you want to start a new game? This will delete any existing progress.",
        "Yes",
        "No"
    );
}
*/
}
