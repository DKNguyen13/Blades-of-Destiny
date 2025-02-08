using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class SaveSystem : MonoBehaviour
{
    private string filePath;
    public Button saveButton;
    public Button saveButton1;
    public Button saveButton2;

    // Start is called before the first frame update
    void Start()
    {
        filePath = Path.Combine(Application.persistentDataPath, "gameData.json");
        Debug.Log("Đường dẫn lưu file: " + filePath);  // Xem đường dẫn file
        saveButton.onClick.AddListener(OnSaveButtonClicked);  // Đăng ký sự kiện cho nút
        saveButton1.onClick.AddListener(OnSaveButtonClicked1);  // Đăng ký sự kiện cho nút
        saveButton2.onClick.AddListener(OnSaveButtonClicked2);  // Đăng ký sự kiện cho nút

        // Tải dữ liệu nếu có khi bắt đầu
        //Load();
    }

    public void Save(GameData gameData)
    {
        string json = JsonUtility.ToJson(gameData,true);
        File.WriteAllText(filePath, json);
        Debug.Log("Dữ liệu đã được lưu tại: " + filePath);
    }

    public GameData Load()
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath); // Đọc dữ liệu từ file
            GameData data = JsonUtility.FromJson<GameData>(json); // Chuyển đổi JSON thành đối tượng GameData
            Debug.Log("Dữ liệu đã được tải.");
            return data;
        }
        else
        {
            Debug.LogWarning("File không tồn tại!");
            return null;
        }
    }

    // Lưu dữ liệu khi nút được nhấn
    public void OnSaveButtonClicked()
    {
        // Tạo một đối tượng GameData mới hoặc load dữ liệu hiện tại
        GameData gameData = Load() ?? new GameData();
        Debug.Log("on click active");
        // Thêm level mới vào danh sách
        //gameData.levelDataGames.Add("1.1");  // Thêm level "1.1" vào danh sách

        // Lưu lại dữ liệu vào file JSON
        Save(gameData);
    }
    public void OnSaveButtonClicked1()
    {
        Debug.Log("on click active1");

    }    public void OnSaveButtonClicked2()
    {
        Debug.Log("on click active2");

    }
}
