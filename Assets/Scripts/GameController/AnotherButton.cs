using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AnotherButton : MonoBehaviour
{
    [Header("Another button")]
    [SerializeField] private Button run;
    [SerializeField] private Button backMainPanel;
    [SerializeField] private Button skillOpenPanel;
    [SerializeField] private Button backMenu, backHome, playAgain;

    [SerializeField] private GameObject skillPanel, mainPanel, gameOverPanel, gameWinPanel, mainWinPanel;

    // Start is called before the first frame update
    void Start()
    {
        //On click
        run.onClick.AddListener(Run);
        backMainPanel.onClick.AddListener(CloseSkillPanel);
        skillOpenPanel.onClick.AddListener(OpenSkillPanel);
        backMenu.onClick.AddListener(Run);
        backHome.onClick.AddListener(() => Home(true));
        playAgain.onClick.AddListener(() => Home(false));
    }

    //Run
    public void Run()
    {
        SceneManager.LoadScene("HomeScene");
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

    public void ShowWinPanel()
    {
        gameWinPanel.SetActive(true);
        StartCoroutine(FadeIn(gameWinPanel, 1.5f)); // Chạy hiệu ứng trong 1.5 giây
        Time.timeScale = 0f;
    }
    
    public IEnumerator FadeIn(GameObject gameObject , float duration)
    {
        float time = 0;
        SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        Image image = gameObject.GetComponent<Image>();
        if(spriteRenderer != null)//SpriteRenderer 2D
        {
            Color color = spriteRenderer.color;
            color.a = 0f;
            spriteRenderer.color = color;
            while (time < duration)
            {
                time += Time.unscaledDeltaTime;
                color.a = Mathf.Lerp(0,1,time/duration);
                spriteRenderer.color = color;
                yield return null;
            }
        }
        else//UI
        {
            Color color = image.color;
            color.a = 0f;
            image.color = color;
            while(time < duration)
            {
                time += Time.unscaledDeltaTime;
                color.a = Mathf.Lerp(0,1, time/duration);
                image.color = color;
                yield return null;
            }
            mainWinPanel.SetActive(true) ;
        }
    }
    public void Home(bool x)
    {
        if (x)
        {
            SceneManager.LoadScene("HomeScene");
        }
        else
        {
            SceneManager.LoadScene("Scene1");
        }
    }
}
