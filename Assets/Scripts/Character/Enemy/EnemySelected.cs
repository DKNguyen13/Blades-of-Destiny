using UnityEngine;

public class EnemySelected : MonoBehaviour
{
    private int enemyIndex; // Gán số thứ tự cho enemy này
    private GameManager gameManager;

    private void Start()
    {
        // Tìm GameManager trong scene
        gameManager = FindObjectOfType<GameManager>();
    }

    private void OnMouseDown()
    {
        // Khi nhấn chuột vào enemy, gọi hàm ShowArrow của GameManager
        if (gameManager != null)
        {
            gameManager.ShowArrow(enemyIndex);
        }
    }

    public int EnemyIndex { set => enemyIndex = value; get => enemyIndex; }
}
