using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    [Header("Menu")]
    [SerializeField] private Button button1vs1;
    [SerializeField] private Button button3vs3;
    [SerializeField] private Button back;
    [SerializeField] private Button next;
    [SerializeField] private Button exit;
    [SerializeField] private Button E_Water;
    [SerializeField] private Button E_Fire;
    [SerializeField] private Button E_Leaf;
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject choseHeroPanel;

    // Start is called before the first frame update
    void Start()
    {
        back.onClick.AddListener(() => BackMenu(true));
        button1vs1.onClick.AddListener(()=> BackMenu(false));
        button3vs3.onClick.AddListener(() => OnButtonClicked(3));
        next.onClick.AddListener(NextScene);
        exit.onClick.AddListener(ExitGame);
        E_Water.onClick.AddListener(() => OnButtonClicked(0));  // Index 0
        E_Fire.onClick.AddListener(() => OnButtonClicked(1));   // Index 1
        E_Leaf.onClick.AddListener(() => OnButtonClicked(2));   // Index 2
    }

    private void OnButtonClicked(int index)
    {
        // Lưu index vào PlayerPrefs
        PlayerPrefs.SetInt("SelectedElementIndex", index);
        PlayerPrefs.Save();  // Lưu lại giá trị

        // Chuyển sang scene tiếp theo
        SceneManager.LoadScene("Scene1");
    }

    public void BackMenu(bool back)
    {
        if(back)
        {
            menuPanel.SetActive(true);
            choseHeroPanel.SetActive(false);
        }
        else
        {
            menuPanel.SetActive(false);
            choseHeroPanel.SetActive(true);
        }
    }

    public void NextScene()
    {
        SceneManager.LoadScene("Scene1");
    }

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
