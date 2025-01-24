using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AnotherButton : MonoBehaviour
{
    [Header("Another button")]
    [SerializeField] private Button run;
    [SerializeField] private Button backMainPanel;
    [SerializeField] private Button skillOpenPanel;
    [SerializeField] private Button backMenu;

    [SerializeField] private GameObject skillPanel;
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject gameOverPanel;

    // Start is called before the first frame update
    void Start()
    {
        //On click
        run.onClick.AddListener(Run);
        backMainPanel.onClick.AddListener(CloseSkillPanel);
        skillOpenPanel.onClick.AddListener(OpenSkillPanel);
        backMenu.onClick.AddListener(Run);
    }

    //Run
    public void Run()
    {
        SceneManager.LoadScene("MenuScene");
    }

    //Open skill panel
    public void OpenSkillPanel()
    {
        skillPanel.SetActive(true);
        mainPanel.SetActive(false);
    }

    //Back main panel
    public void CloseSkillPanel()
    {
        mainPanel.SetActive(true);
        skillPanel.SetActive(false);
    }

    public void ShowGameOverPanel()
    {
        gameOverPanel.SetActive(true);
        Time.timeScale = 0f;
    }
}
