using UnityEngine;
using UnityEngine.UI;

public class ShowBattleButton : MonoBehaviour
{
    [SerializeField] private GameObject mapEnemyPanel;
    [SerializeField] private Button exitBtl_btn;

    public void ShowBattle()
    {
        exitBtl_btn.gameObject.SetActive(true);
        mapEnemyPanel.SetActive(true);
    }
}
