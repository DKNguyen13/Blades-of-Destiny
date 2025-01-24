using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{

    [Header("GameObject Prefabs")]
    [SerializeField] private List<GameObject> playerPrefab;
    //[SerializeField] private List<GameObject> playerPreTMP;
    [SerializeField] private List<GameObject> enemyPrefab;

    // Arrow indicators for each enemy
    [Header("Enemy Arrows")]
    [SerializeField] private List<GameObject> enemyArrows = new List<GameObject>();

    [Header("Button")]
    [SerializeField] private Button baseAttack;
    [SerializeField] private Button skill;
    [SerializeField] private Button defense;
    [SerializeField] private Button skill1;
    [SerializeField] private Button skill2;
    [SerializeField] private Button skill3;
    [SerializeField] private Button skill4;

    [Header("Player status panel")]
    [SerializeField] private GameObject systemBattle;
    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private TextMeshProUGUI playerHpText;
    [SerializeField] private TextMeshProUGUI playerStaminaText;
    [SerializeField] private TextMeshProUGUI playerDmgText;
    [SerializeField] private TextMeshProUGUI playerDefenseText;
    [SerializeField] private TextMeshProUGUI playerLevelText;

    //[Header("Spawn Positions")]
    private Vector3 playerStartPosition = new Vector3(-5f, -2f, 0f); // Bên trái màn hình
    private Vector3 enemyStartPosition = new Vector3(5f, -2f, 0f);   // Bên phải màn hình
    private float spacing = 1.5f; // Distance between characters
    private float spacingCharacterMid = 1.5f;
    private float waitingTime = 1.25f;

    //List player and enemy
    private List<GameObject> players = new List<GameObject>();
    private List<GameObject> enemies = new List<GameObject>();

    // Turn-based settings
    private int maxCharacter = 3;
    private int enemySelect;
    private int playerSelect;
    private int currentPlayerIndex = 0;
    private int turnBase = 1;
    private float distance = 2.2f;//Turn based attack
    private AnotherButton anotherButton;
    private Vector3 enemyTransform;

    //Check
    private bool isPlayerTurn = true;
    private bool isProcess = false;

    //Awake
    private void Awake()
    {
        if (playerPrefab.Count > maxCharacter)
        {
            playerPrefab = playerPrefab.GetRange(0, maxCharacter);
            Debug.LogWarning("List has been trimmed to max size: " + maxCharacter);
        }
        if (enemyPrefab.Count > maxCharacter)
        {
            enemyPrefab = enemyPrefab.GetRange(0, maxCharacter);
            Debug.LogWarning("List has been trimmed to max size: " + maxCharacter);
        }
        /*
        // Lấy giá trị index từ PlayerPrefs
        int selectedElementIndex = PlayerPrefs.GetInt("SelectedElementIndex", 4); // Mặc định là 0 nếu không có giá trị
        if (selectedElementIndex != 3)
        {
            playerPrefab.Add(playerPreTMP[selectedElementIndex]);
        }
        else
        {
            playerPrefab.AddRange(playerPreTMP);
        }
        */
        Time.timeScale = 1f;
        anotherButton = GetComponent<AnotherButton>();
    }

    // Start is called before the first frame update
    void Start()
    {
        //Spawn character
        SpawnCharacters(playerPrefab, players, playerStartPosition, true);
        SpawnCharacters(enemyPrefab, enemies, enemyStartPosition, false);

        enemySelect = (enemies.Count / 2);//Select the target enemy to attack
        playerSelect = (players.Count) / 2;//Select the target player to attack

        FindArrow();//Find Arrow

        //Button
        baseAttack.onClick.AddListener(Attack);
        defense.onClick.AddListener(Defense);
        skill1.onClick.AddListener(Skill1);
        skill2.onClick.AddListener(Skill2);
        skill3.onClick.AddListener(Skill3);
        skill4.onClick.AddListener(Skill4);
    }

    // Update is called once per frame
    void Update()
    {
        if (!TeamPlayerLose())
        {
            UpdateCurrentPlayerIndex();
            if (CheckEnemise())
            {
                enemies.RemoveAll(en => en == null);
                FindArrow();
            }
            PlayerStatusPanel();
        }
        else
        {
            Debug.Log("Lose");
            anotherButton.ShowGameOverPanel();
        }
        Debug.Log(currentPlayerIndex);
    }

    //Attack 
    public void Attack()
    {
        if (isPlayerTurn && !isProcess)
        {
            isProcess = true;
            StartCoroutine(MoveAndAttackSinglePlayer(players[currentPlayerIndex],0));// Chạy coroutine để di chuyển và tấn công player hiện tại
        }
    }

    public GameObject E_Leaf()
    {
        foreach( var player in players )
        {
            Player plB = player.GetComponent<Player>();
            if(plB.ElementType == TypeElement.Leaf)
            {
                return player;
            }
        }
        return null;
    }

    //Defense
    public void Defense()
    {
        if(isPlayerTurn && !isProcess)
        {
            isProcess = true;
            Player plBase = players[currentPlayerIndex].GetComponent<Player>();
            plBase.state = PlayerState.Defense;
            currentPlayerIndex++;
            isProcess = false;
            if (currentPlayerIndex >= players.Count)
            {
                currentPlayerIndex = 0;
                PlayerTurn(false);
            }
        }
    }

    //Skill 1
    public void Skill1()
    {
        if (isPlayerTurn && !isProcess)
        {
            Debug.Log("p "+currentPlayerIndex);
            isProcess = true;
            UpdateCurrentPlayerIndex();
            Player plBase = players[currentPlayerIndex].GetComponent<Player>();
            plBase.state = PlayerState.Skill1;
            plBase.BaseAttack();
            StartCoroutine(ExecuteSkill(plBase));
        }
    }

    private IEnumerator ExecuteSkill(Player plBase)
    {
        int x = 1;
        if (plBase.ElementType == TypeElement.Water)
        {
            x = 4;
            currentPlayerIndex++;
        }
        else if (plBase.ElementType == TypeElement.Leaf)
        {
            E_Leaf_Arrow arrow_E = players[currentPlayerIndex].GetComponent<E_Leaf_Arrow>();
            arrow_E.EnemyPosition = enemyTransform;
            x = 4;
            PerformAttack(players[currentPlayerIndex], enemies[enemySelect]);
            currentPlayerIndex++;
        }
        else
        {
            yield return StartCoroutine(MoveAndAttackSinglePlayer(players[currentPlayerIndex], 1)); // Chờ MoveAndAttackSinglePlayer hoàn thành
        }
        yield return StartCoroutine(EndSkill(x)); // Chờ EndSkill sau khi các bước trên hoàn tất
    }

    //Skill 2
    public void Skill2()
    {
        if (isPlayerTurn && !isProcess)
        {
            isProcess = true;
            Player plBase = players[currentPlayerIndex].GetComponent<Player>();
            plBase.state = PlayerState.Skill2;
            plBase.BaseAttack();
            StartCoroutine(MoveAndAttackSinglePlayer(players[currentPlayerIndex],2));// Chạy coroutine để di chuyển và tấn công player hiện tại
            StartCoroutine(EndSkill(2));
        }
    }
    
    //Skill 3
    public void Skill3()
    {
        if (isPlayerTurn && !isProcess)
        {
            isProcess = true;
            Player plBase = players[currentPlayerIndex].GetComponent<Player>();
            plBase.state = PlayerState.Skill3;
            plBase.BaseAttack();
            StartCoroutine(MoveAndAttackSinglePlayer(players[currentPlayerIndex], 3));// Chạy coroutine để di chuyển và tấn công player hiện tại
            StartCoroutine(EndSkill(3));
        }
    }
    
    //Skill 4
    public void Skill4()
    {
        if (isPlayerTurn && !isProcess)
        {
            isProcess = true;
            Player plBase = players[currentPlayerIndex].GetComponent<Player>();
            plBase.state = PlayerState.Skill4;
            plBase.BaseAttack();
            StartCoroutine(MoveAndAttackSinglePlayer(players[currentPlayerIndex], 4));// Chạy coroutine để di chuyển và tấn công player hiện tại
            StartCoroutine(EndSkill(4));
        }
    }

    //End skill
    IEnumerator EndSkill(int x)
    {
        float tmp = waitingTime;
        if(x == 4)
        {
            yield return new WaitForSeconds(1f);
        }
        else if(x == 1)
        {
            tmp -= 0.5f;
        }
        anotherButton.CloseSkillPanel();
        yield return new WaitForSeconds(tmp);
        isProcess = false;
        if (currentPlayerIndex >= players.Count) { currentPlayerIndex = 0; PlayerTurn(false); }
    }

    //Perform attack
    public void PerformAttack(GameObject player, GameObject enemy)
    {
        Player playerBase = player.GetComponent<Player>();
        Enemy enemyBase = enemy.GetComponent<Enemy>();

        if (isPlayerTurn)
        {
            float multiplier = playerBase.state switch
            {
                PlayerState.Skill1 => 1.1f,//Increase 10%
                PlayerState.Skill2 => 1.2f,//Increase 20%
                PlayerState.Skill3 => 1.3f,//Increase 30%
                PlayerState.Skill4 => 1.4f,//Increase 40%
                _ => 1f
            };
            playerBase.BaseAttack();//Player attack
            if(playerBase.state == PlayerState.None)
            {
                enemyBase.TakeDmg(playerBase.Damage, playerBase.CriticalDmg, playerBase.CriticalRate);//Enemy take damage
            }
            else
            {
                enemyBase.TakeDmg((int)(playerBase.Damage * multiplier), playerBase.CriticalDmg, playerBase.CriticalRate);//Enemy take damage
            }
        }
        else
        {           
            enemyBase.BaseAttack();//Enemy attack
            playerBase.TakeDmg(enemyBase.Damage, enemyBase.CriticalDmg, enemyBase.CriticalRate);//Player take damage
        }
    }

    // Coroutine để xử lý di chuyển và tấn công của một player
    private IEnumerator MoveAndAttackSinglePlayer(GameObject player, int skill)
    {
        Vector3 initialPosition = player.transform.position;        // Lưu vị trí ban đầu của player
        GameObject targetEnemy = enemies[enemySelect];  // Chọn enemy được chỉ định     
        player.transform.position = targetEnemy.transform.position + new Vector3(-distance, 0.2f, 0);// Dịch chuyển Player tới Enemy
        PerformAttack(player, targetEnemy);// Thực hiện tấn công
        float tmp = skill switch
        {
            1 => 0.35f,//Skill 1: +0.35f
            2 => 0.65f,//Skill 2: +0.6f
            3 => 0.8f,//Skill 3: +0.8f
            4 => 0.85f,//Skill 4: +0.85f
            _ => 0f
        };
        yield return new WaitForSeconds(waitingTime + tmp);// Đợi một chút trước khi quay lại vị trí ban đầu
        player.transform.position = initialPosition;    // Quay lại vị trí ban đầu
        currentPlayerIndex++;    // Chuyển sang player tiếp theo
        isProcess = false;  // Kết thúc tiến trình

        if(currentPlayerIndex >= players.Count)
        {
            Debug.Log("Hết lượt của Player. Đến lượt Enemy.");
            PlayerTurn(false);
        }
    }

    //Player Status panel
    public void PlayerStatusPanel()
    {
        if (isPlayerTurn)
        {
            if(currentPlayerIndex < players.Count)
            {
                Player playerBase = players[currentPlayerIndex].GetComponent<Player>();
                if (playerBase.state != PlayerState.Death)
                {
                    playerNameText.text = playerBase.NameCharacter;
                    playerHpText.text = "HP: " + playerBase.CurrentHealth.ToString() + "/" + playerBase.MaxHealth.ToString();
                    playerStaminaText.text = "Stamina: " + playerBase.CurrentStamina.ToString() + "/" + playerBase.MaxStamina.ToString();
                    playerDmgText.text = "Dmg: " + playerBase.Damage.ToString();
                    playerDefenseText.text = "Defense: " + playerBase.Defense.ToString();
                    playerLevelText.text = "Level: " + playerBase.Level.ToString();
                }
            }
        }
    }

    //Update current player index
    public void UpdateCurrentPlayerIndex()
    {
        if(currentPlayerIndex < players.Count)
        {
            for (int i = 0; i < players.Count; i++)
            {
                Player playerBase = players[currentPlayerIndex].GetComponent<Player>();
                if (playerBase.state == PlayerState.Death)
                {
                    if (currentPlayerIndex < players.Count)
                    {
                        currentPlayerIndex++;
                    }
                    else
                    {
                        currentPlayerIndex = 0;
                        Debug.Log("Lose");
                        anotherButton.ShowGameOverPanel();
                    }
                }
                else
                {
                    break;
                }

            }
        }
        else
        {
            currentPlayerIndex = 0;
        }
    }

    // Hàm kết thúc lượt của player
    private void PlayerTurn(bool turn)
    {
        if(!turn)
        {
            systemBattle.SetActive(false);
            isPlayerTurn = false;
            currentPlayerIndex = 0; // Reset lượt tấn công của player
            StartCoroutine(EnemyTurnSequence());
        }
        else
        {
            systemBattle.SetActive(true);
            isPlayerTurn = true;
            turnBase += 1;
            Debug.Log("Turn " + turnBase);
            currentPlayerIndex = 0;
            for(int i = 0; i < players.Count; i++)
            {
                Player pBase = players[i].GetComponent<Player>();
                if (pBase.state != PlayerState.None && pBase.state != PlayerState.Death)
                {
                    pBase.state = PlayerState.None;
                    pBase.PlayerDefense();
                }
            }
        }
    }

    // Coroutine để xử lý lượt của các enemy lần lượt
    private IEnumerator EnemyTurnSequence()
    {
        yield return new WaitForSeconds(0.7f);
        for (int i = 0; i < enemies.Count; i++)
        {
            yield return new WaitForSeconds(0.5f);
            // Xử lý lượt của enemy
            yield return StartCoroutine(EnemyTurn(enemies[i]));

        }
        yield return new WaitForSeconds(1f);
        Debug.Log("Lượt của Enemy kết thúc.");
        PlayerTurn(true);
    }

    //Enemy turn
    private IEnumerator EnemyTurn(GameObject enemy)
    {
        Vector3 initialPos = enemy.transform.position;
        GameObject attackedPlayer = GetLowestPlayerHp();

        if (attackedPlayer != null)
        {
            enemy.transform.position = attackedPlayer.transform.position + new Vector3(distance, 0, 0);
            PerformAttack(attackedPlayer, enemy);
            yield return new WaitForSeconds(waitingTime);
            enemy.transform.position = initialPos;
        }
    }

    //Find player's hp is the lowest
    public GameObject GetLowestPlayerHp()
    {
        int lowestHP = int.MaxValue;
        playerSelect = 0;
        GameObject playerSelected = null;

        for(int i = 0; i< players.Count; i++)
        {
            Player hp = players[i].GetComponent<Player>();
            if (hp.state != PlayerState.Death)
            {
                if (hp.CurrentHealth < lowestHP)
                {
                    lowestHP = hp.CurrentHealth;
                    playerSelect = i;
                    playerSelected = players[playerSelect];
                }
            }
        }
        return playerSelected;
    }


    //Check enemies null
    public bool CheckEnemise()
    {
        for(int i = 0;i< enemies.Count;i++)
        {
            if( enemies[i] == null ) return true;
        }
        return false;
    }

    //Check win
    public bool TeamPlayerLose()
    {
        if(enemies.Count == 0) return false;
        return players.TrueForAll(player => player.GetComponent<Player>().state == PlayerState.Death);
    }

    //Spawn character
    public void SpawnCharacters(List<GameObject> prefabs, List<GameObject> characterList, Vector3 startPosition, bool isPlayerTeam)
    {
        int quantity = prefabs.Count;
        if (quantity==0)
        {
            Debug.LogError("Prefab list is empty!");
            return;
        }

        if (quantity == 1)
        {
            Vector3 spawnPosition = startPosition + new Vector3(0f, 2.2f, 0);

            GameObject character = Instantiate(prefabs[0], spawnPosition, Quaternion.identity);

            if (!isPlayerTeam)
            {
                character.transform.localScale = new Vector3(-1 * character.transform.localScale.x, character.transform.localScale.y, character.transform.localScale.z);
            }

            // Gán index cho mỗi enemy
            EnemySelected interaction = character.AddComponent<EnemySelected>();
            interaction.EnemyIndex = 0;

            // Thêm vào danh sách
            characterList.Add(character);
        }
        else if (quantity == 2)
        {
            for (int i = 0; i < 2; i++)
            {
                Vector3 spawnPosition = startPosition + new Vector3 (0f,1.3f + i*spacing,0);
                GameObject character = Instantiate(prefabs[i], spawnPosition, Quaternion.identity);

                if (!isPlayerTeam)
                {
                    character.transform.localScale = new Vector3(-1 * character.transform.localScale.x, character.transform.localScale.y, character.transform.localScale.z);
                }

                // Gán index cho mỗi enemy
                EnemySelected interaction = character.AddComponent<EnemySelected>();
                interaction.EnemyIndex = i;

                // Thêm vào danh sách
                characterList.Add(character);
            }
        }
        else if(quantity == 3)
        {
            for (int i = 0; i < quantity; i++)
            {
                Vector3 spawnPosition;

                if (i == 1) // Nếu là nhân vật ở giữa
                {
                    if (isPlayerTeam)
                    {
                        // Player ở giữa, dịch sang phải
                        spawnPosition = startPosition + new Vector3(spacingCharacterMid, i * spacing, 0);
                    }
                    else
                    {
                        // Enemy ở giữa, dịch sang trái
                        spawnPosition = startPosition + new Vector3(-spacingCharacterMid, i * spacing, 0);
                    }
                }
                else
                {
                    // Các nhân vật còn lại thẳng hàng
                    spawnPosition = startPosition + new Vector3(0, i * spacing, 0);
                }

                // Instantiate prefab
                GameObject character = Instantiate(prefabs[i], spawnPosition, Quaternion.identity);

                // Nếu là enemy, quay mặt đối diện
                if (!isPlayerTeam)
                {
                    character.transform.localScale = new Vector3(-1 * character.transform.localScale.x, character.transform.localScale.y, character.transform.localScale.z);
                }

                // Gán index cho mỗi enemy
                EnemySelected interaction = character.AddComponent<EnemySelected>();
                interaction.EnemyIndex = i;

                // Thêm vào danh sách
                characterList.Add(character);

            }
        }

    }

    //Find arrow
    public void FindArrow()
    {
        //Select the target enemy to attack
        enemySelect = (enemies.Count / 2);
        int d = -1;
        foreach (var enemy in enemies)
        {
            d++;
            GameObject arrows = enemy.transform.Find("arrow").gameObject;
            if (arrows != null)
            {
                if (d != enemySelect)
                {
                    arrows.SetActive(false);
                }
                else if (d == enemySelect)
                {
                    enemyTransform = enemy.transform.position + new Vector3(-0.95f, 0.2f, 0); ;
                    arrows.SetActive(true);
                }
                enemyArrows.Add(arrows);
            }
        }
    }

    //Show Arrow
    public void ShowArrow(int enemyIndex)
    {
        HideAllArrows();
        // Hiển thị mũi tên của enemy được chọn
        if (enemyIndex >= 0 && enemyIndex < enemyArrows.Count && enemyArrows[enemyIndex] != null)
        {
            enemySelect = enemyIndex;
            enemyArrows[enemyIndex].SetActive(true);
            enemyTransform = enemies[enemyIndex].transform.position + new Vector3(-0.95f, 0.2f, 0); ;

        }
    }

    //Hide all arrow
    public void HideAllArrows()
    {
        // Ẩn tất cả các mũi tên
        foreach (var arrow in enemyArrows)
        {
            if (arrow != null)
            {
                arrow.SetActive(false);
            }
        }

    }
}
