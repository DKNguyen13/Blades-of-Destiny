using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;

public class GameManager : MonoBehaviour
{

    [Header("GameObject Prefabs")]
    private List<GameObject> playerPrefab =  new();
    private List<GameObject> enemyPrefab = new();
    [SerializeField] private List<GameObject> playerPreTMP;
    [SerializeField] private List<GameObject> enemyPreTMP;
    private List<GameObject> players = new();
    private List<GameObject> enemies = new();

    // Arrow indicators for each enemy
    [Header("Enemy Arrows")]
    private List<GameObject> enemyArrows = new();

    [Header("Button")]
    private AnotherButton anotherButton;
    [SerializeField] private Button baseAttack, skill, defense, skill1, skill2, skill3, skill4;
    [SerializeField] private GameObject[] bG;

    [Header("Player status panel")]
    [SerializeField] private GameObject systemBattle;
    [SerializeField] private TextMeshProUGUI playerNameText, playerHpText, playerStaminaText, playerDmgText, playerDefenseText, playerLevelText, expTxt;

    private int[] originalDefense;
    //[Header("Spawn Positions")]
    private Vector3 playerStartPosition = new(-5f, -2f, 0f); // Bên trái màn hình
    private Vector3 enemyStartPosition = new(5f, -2f, 0f);   // Bên phải màn hình
    private float spacingCharacterMid = 1.5f, waitingTime = 1.25f, spacing = 1.5f;
    private Vector3 initialPosition, enemyTransform;
    private GameObject targetEnemy;

    // Turn-based settings
    private int enemySelect, playerSelect, currentPlayerIndex = 0, turnBase = 1,selectedLvl;
    private float distance = 2.2f;//Turn based attack
    private bool isPlayerTurn = true, isProcess = false, hasSaved = false, activeRecovery = false;

    //Awake
    private void Awake()
    {
        // Lấy giá trị index từ PlayerPrefs
        selectedLvl = PlayerPrefs.GetInt("lvl", 1); // Mặc định là 1 nếu không có giá trị
        EnemyPre(selectedLvl);
        int hero = PlayerPrefs.GetInt("E_hero");
        playerPrefab.Add(playerPreTMP[hero]);

        if (playerPrefab.Count > 3)
        {
            playerPrefab = playerPrefab.GetRange(0, 3); Debug.LogWarning("List has been trimmed to max size: " + 3);
        }
        if (enemyPrefab.Count > 3)
        {
            enemyPrefab = enemyPrefab.GetRange(0, 3); Debug.LogWarning("List has been trimmed to max size: " + 3);
        }
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
        StartCoroutine(UpdateStatusAfterFrame());
    }

    // Update is called once per frame
    void Update()
    {
        if (!CheckResults())
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
        if (CheckWin())
        {
            if (!hasSaved)
            {
                hasSaved = true;
                Save();
            }
            anotherButton.ShowWinPanel();  
        }
    }


    //Perform attack
    public void PerformAttack(GameObject player, GameObject enemy)
    {
        if(enemy == null)
        {
            return;
        }
        Player playerBase = player.GetComponent<Player>();
        Enemy enemyBase = enemy.GetComponent<Enemy>();

        if (isPlayerTurn)
        {
            float multiplier = 1f;
            if (playerBase.ElementType == TypeElement.Fire)
            {
                multiplier = playerBase.state switch
                {
                    PlayerState.Skill1 => 1.2f,//Increase 20%
                    PlayerState.Skill2 => 0.7f,//Decrease 30%
                    PlayerState.Skill3 => 0.8f,//Decrease 20%
                    PlayerState.SpecialSkill => 1.5f,//Increase 50%
                    _ => 1f
                };
            }
            else if (playerBase.ElementType == TypeElement.Water)
            {
                multiplier = playerBase.state switch
                {
                    PlayerState.Skill2 => 1.3f,//Increase 30%
                    PlayerState.Skill3 => 1.4f,//INcrease 40%
                    PlayerState.SpecialSkill => 1.6f,//Increase 60%
                    _ => 1f
                };
            }
            else if (playerBase.ElementType == TypeElement.Ground)
            {
                multiplier = playerBase.state switch
                {
                    PlayerState.Skill1 => 1.1f, //Increase 20%
                    PlayerState.Skill2 => 1.2f,//Increase 25%
                    PlayerState.Skill3 => 1.3f,//Increase 30%
                    PlayerState.SpecialSkill => 1.5f,//Increase 60%
                    _ => 1f
                };
            }
            else
            {
                multiplier = playerBase.state switch
                {
                    PlayerState.Skill1 => 1.2f,//Increase 20%
                    PlayerState.Skill2 => 1.15f,//Increase 15%
                    PlayerState.Skill3 => 1.2f,//Increase 20%
                    PlayerState.SpecialSkill => 1.6f,//Increase 60%
                    _ => 1f
                };
            }
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

    public void AttackAndMove()
    {
        StartCoroutine(WaitAndMoveBack());
    }

    public void PlayerAttack()
    {
        PerformAttack(players[currentPlayerIndex], targetEnemy);
    }
    //Attack 
    public void Attack()
    {
        if (isPlayerTurn && !isProcess)
        {
            isProcess = true;
            Player plBase = players[currentPlayerIndex].GetComponent<Player>();
            plBase.state = PlayerState.BaseAttack;
            Debug.Log("Base attack");
            StartCoroutine(MoveAndAttackSinglePlayer(players[currentPlayerIndex]));// Chạy coroutine để di chuyển và tấn công player hiện tại
        }
    }

    //Defense
    public void Defense()
    {
        if (isPlayerTurn && !isProcess)
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
            Player plBase = players[currentPlayerIndex].GetComponent<Player>();
            plBase.state = PlayerState.Skill1;
            //int amount = (int)(plBase.MaxStamina * 0.2f);
            int amount = 50;
            if (plBase.CheckConditionStamina(amount))
            {
                isProcess = true;
                if (plBase.ElementType != TypeElement.Water)
                {
                    StartCoroutine(MoveAndAttackSinglePlayer(players[currentPlayerIndex]));
                }
                plBase.UseSkill(amount);
            }
            else
            {
                FindAnyObjectByType<FadeOutText>().ShowMessage("Not enough stamina to use the skill (50).");
            }
        }
    }

    //Skill 2
    public void Skill2()
    {
        if (isPlayerTurn && !isProcess)
        {
            Player plBase = players[currentPlayerIndex].GetComponent<Player>();
            plBase.state = PlayerState.Skill2;
            //int amount = (int)(plBase.MaxStamina * 0.5f);
            int amount = 100;
            if (plBase.CheckConditionStamina(amount))
            {
                isProcess = true;
                StartCoroutine(MoveAndAttackSinglePlayer(players[currentPlayerIndex]));
                plBase.UseSkill(amount);
            }
            else
            {
                FindAnyObjectByType<FadeOutText>().ShowMessage("Not enough stamina to use the skill (100).");
            }
        }
    }

    //Skill 3
    public void Skill3()
    {
        if (isPlayerTurn && !isProcess)
        {
            Player plBase = players[currentPlayerIndex].GetComponent<Player>();
            plBase.state = PlayerState.Skill3;
            //int amount = (int)(plBase.MaxStamina * 0.7f);
            int amount = 150;
            if (plBase.CheckConditionStamina(amount))
            {
                isProcess = true;
                StartCoroutine(MoveAndAttackSinglePlayer(players[currentPlayerIndex]));
                plBase.UseSkill(amount);
            }
            else
            {
                FindAnyObjectByType<FadeOutText>().ShowMessage("Not enough stamina to use the skill (150).");
            }
        }
    }

    //Skill 4
    public void Skill4()
    {
        if (isPlayerTurn && !isProcess)
        {
            Player plBase = players[currentPlayerIndex].GetComponent<Player>();
            plBase.state = PlayerState.SpecialSkill;
            int amount = 200;
            if (plBase.CheckConditionStamina(amount))
            {
                isProcess = true;
                StartCoroutine(MoveAndAttackSinglePlayer(players[currentPlayerIndex]));
                plBase.UseSkill(amount);
            }
            else
            {
                FindAnyObjectByType<FadeOutText>().ShowMessage("Not enough stamina to use the skill (200).");
            }
        }
    }

    //End skill
    public void EndPlayerSkill()
    {
        StartCoroutine(EndSkill());
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
                        break;
                    }
                    else
                    {
                        currentPlayerIndex = 0;
                        EndSkill();
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

    // Player Turn
    private void PlayerTurn(bool turn)
    {
        if (turn)
        {
            systemBattle.SetActive(true);
            isPlayerTurn = true;
            turnBase += 1;
            Debug.Log("Turn " + turnBase);
            currentPlayerIndex = 0;
            for (int i = 0; i < players.Count; i++)
            {
                Player pBase = players[i].GetComponent<Player>();
                if (pBase.state != PlayerState.None && pBase.state != PlayerState.Death)
                {
                    if(pBase.state == PlayerState.Defense)
                    {
                        if (pBase.Defense > 1)
                        {
                            pBase.Defense = Mathf.RoundToInt(originalDefense[i] / 1.5f);
                        }
                        else
                        {
                            pBase.Defense = 0;
                        }
                        originalDefense[i] = pBase.Defense;
                    }
                    pBase.state = PlayerState.None;
                }
                if(pBase.Ability == PlayerAbilities.ContinousRecovery)
                {
                    if(pBase.CurrentHealth <= (int)(pBase.MaxHealth * 0.15f))
                    {
                        activeRecovery = true;
                    }
                    if (activeRecovery)
                    {
                        pBase.CurrentHealth += (int)(pBase.MaxHealth * 0.1f);
                        pBase.CurrentStamina += (int)(pBase.MaxStamina * 0.1f);
                        if (pBase.CurrentHealth > pBase.MaxHealth)
                        {
                            pBase.CurrentHealth = pBase.MaxHealth;
                        }
                    }
                }
            }
        }
        else
        {
            systemBattle.SetActive(false);
            isPlayerTurn = false;
            currentPlayerIndex = 0; // Reset lượt tấn công của player
            StartCoroutine(EnemyTurnSequence());
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
    public void EnemyPre(int x)
    {
        bG[Random.Range(0, bG.Length)].SetActive(true);
        if (x == 1)
        {
            int tmp = Random.Range(0, 3);
            enemyPrefab.Add(enemyPreTMP[tmp]);
        }
        else if(x == 2)
        {
            for (int i = 0; i < 2; i++)
            {
                int tmp = Random.Range(0, 3);
                enemyPrefab.Add(enemyPreTMP[tmp]);
            }
        }
        else if(x == 3)
        {
            int quantityEnemy = Random.Range(2, 4);
            for (int i = 0; i < quantityEnemy; i++)
            {
                int tmp = Random.Range(0, 3);
                enemyPrefab.Add(enemyPreTMP[tmp]);
            }
        }        
        else if(x == 4)
        {
            for (int i = 0; i < 3; i++)
            {
                if (i == 1)
                {
                    enemyPrefab.Add(enemyPreTMP[3]);
                }
                else
                {
                    int tmp = Random.Range(0, 3);
                    enemyPrefab.Add(enemyPreTMP[tmp]);
                }
            }

        }
        else if (x == 5)
        {
            int quantityEnemy = Random.Range(2, 4);
            for (int i = 0; i < quantityEnemy; i++)
            {
                int tmp = Random.Range(4, 6);
                enemyPrefab.Add(enemyPreTMP[tmp]);
            }
        }
        else if(x == 6)
        {
            for (int i = 0; i < 3; i++)
            {
                if (i == 1)
                {
                    enemyPrefab.Add(enemyPreTMP[6]);
                }
                else
                {
                    int tmp = Random.Range(4, 6);
                    enemyPrefab.Add(enemyPreTMP[tmp]);
                }
            }
        }
        else if(x == 7)
        {
            int quantityEnemy = Random.Range(2, 4);
            for (int i = 0; i < quantityEnemy; i++)
            {
                enemyPrefab.Add(enemyPreTMP[7]);
            }
        }
        else if(x == 8)
        {
            for (int i = 0; i < 3; i++)
            {
                if (i == 1)
                {
                    enemyPrefab.Add(enemyPreTMP[8]);
                }
                else
                {
                    enemyPrefab.Add(enemyPreTMP[7]);
                }
            }
        }
        else if(x == 9)
        {
            int quantityEnemy = Random.Range(1, 4);
            for (int i = 0; i < quantityEnemy; i++)
            {
                enemyPrefab.Add(enemyPreTMP[9]);
            }
        }
        else if(x == 10)
        {
            int quantityEnemy = Random.Range(2, 4);
            for (int i = 0; i < quantityEnemy; i++)
            {
                int tmp = Random.Range(9, 11);
                enemyPrefab.Add(enemyPreTMP[tmp]);
            }
        }
        else if(x == 11)
        {
            for (int i = 0; i < 3; i++)
            {
                if (i == 1)
                {
                    enemyPrefab.Add(enemyPreTMP[11]);
                }
                else
                {
                    int tmp = Random.Range(9, 11);
                    enemyPrefab.Add(enemyPreTMP[tmp]);
                }
            }
        }
        else if (x == 12)
        {
            for (int i = 0; i < 3; i++)
            {
                if (i == 1)
                {
                    enemyPrefab.Add(enemyPreTMP[12]);
                }
                else
                {
                    int tmp = Random.Range(0, 12);
                    enemyPrefab.Add(enemyPreTMP[tmp]);
                }
            }
        }
        else if (x == 13)
        {
            for (int i = 0; i < 3; i++)
            {
                if (i == 1)
                {
                    enemyPrefab.Add(enemyPreTMP[13]);
                }
                else
                {
                    int tmp = Random.Range(0, 13);
                    enemyPrefab.Add(enemyPreTMP[tmp]);
                }
            }
        }
    }


    //Arrow hero
    public GameObject E_Leaf()
    {
        foreach (var player in players)
        {
            Player plB = player.GetComponent<Player>();
            if (plB.ElementType == TypeElement.Leaf)
            {
                return player;
            }
        }
        return null;
    }
    //Find, show, hide selected enemy arrow
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
    public void ShowArrow(int enemyIndex)
    {
        HideAllArrows();
        // Hiển thị mũi tên của enemy được chọn
        if (enemyIndex >= 0 && enemyIndex < enemyArrows.Count && enemyArrows[enemyIndex] != null)
        {
            enemySelect = enemyIndex;
            enemyArrows[enemyIndex].SetActive(true);
            enemyTransform = enemies[enemyIndex].transform.position + new Vector3(-0.95f, 0.2f, 0);
        }
    }
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



    //Check win or lose
    public bool CheckResults()
    {
        return players.TrueForAll(player => player.GetComponent<Player>().state == PlayerState.Death);
    }
    public bool CheckWin()
    {
        if (enemies.Count == 0) return true;
        return false;
    }
    public int ExpReward(int playerLevel)
    {
        int baseExp = ExpToLevelUp(playerLevel) / 10;
        // EXP ngẫu nhiên trong khoảng 70% - 130% của baseExp
        int minExp = (int)(baseExp * 0.7f);
        int maxExp = (int)(baseExp * 1.3f);
        int expBonusBoss = 0;
        if(selectedLvl == 4 || selectedLvl == 6|| selectedLvl==8 || selectedLvl ==11|| selectedLvl==12|| selectedLvl == 13){
            expBonusBoss = Random.Range(baseExp / 4, baseExp/2);
        }
        else if(selectedLvl >= playerLevel)
        {
            expBonusBoss = Random.Range(baseExp/10, baseExp/5);
        }
        return Random.Range(minExp, maxExp) + expBonusBoss;
    }
    public int ExpToLevelUp(int playerLevel)
    {
        return (int)(100 * Mathf.Pow(playerLevel,1.5f));//Sử dụng hàm lưu thừa y = a^n
    }

    //Save data
    public void Save()
    {
        string filePath = Path.Combine(Application.persistentDataPath, "gameData.json");
        string jsonWrite;
        GameData gameData;
        if (!File.Exists(filePath))
        {
            gameData = new GameData();
            string json = JsonUtility.ToJson(gameData, true);
            File.WriteAllText(filePath, json);
        }
        else
        {
            string json = File.ReadAllText(filePath);
            gameData = JsonUtility.FromJson<GameData>(json);
        }
        if (selectedLvl > gameData.currentMap)
        {
            gameData.currentMap = selectedLvl;
        }
        int playerLevel = gameData.level;
        int expUp = ExpToLevelUp(playerLevel);
        int expRewadStage = ExpReward(playerLevel);
        gameData.experiece += expRewadStage;

        if (gameData.experiece >= expUp)
        {
            gameData.experiece -= expUp;
            gameData.level++;
            gameData.point += 3;
            expTxt.text = "Level: "+gameData.level+ " (Level Up)" + " \nExp: " + gameData.experiece + " / " + expUp + " (+ " + expRewadStage + " exp)";
        }
        else
        {
            expTxt.text = "Level: " + gameData.level + " \nExp: " + gameData.experiece + " / " + expUp + " (+ " + expRewadStage + " exp)";
        }
        jsonWrite = JsonUtility.ToJson(gameData, true);
        File.WriteAllText(filePath, jsonWrite);
    }



    //IEmurator
    IEnumerator UpdateStatusAfterFrame()
    {
        yield return null; // Chờ 1 frame
        PlayerStatusPanel(); // Cập nhật UI sau khi dữ liệu đã sẵn sàng
        originalDefense = new int[players.Count];
        for (int i = 0; i < players.Count; i++)
        {
            originalDefense[i] = players[i].GetComponent<Player>().Defense;
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
    public IEnumerator WaitAndMoveBack()
    {
        yield return new WaitForSeconds(0.5f);
        anotherButton.CloseSkillPanel();
        players[currentPlayerIndex].transform.position = initialPosition;
        currentPlayerIndex++;    // Chuyển sang player tiếp theo
        isProcess = false;  // Kết thúc tiến trình
        if (currentPlayerIndex >= players.Count)
        {
            Debug.Log("Hết lượt của Player. Đến lượt Enemy.");
            PlayerTurn(false);
        }
    }

    // Attack system (tele attack)
    private IEnumerator MoveAndAttackSinglePlayer(GameObject player)
    {
        initialPosition = player.transform.position;        // Lưu vị trí ban đầu của player
        targetEnemy = enemies[enemySelect];  // Chọn enemy được chỉ định     
        player.transform.position = targetEnemy.transform.position + new Vector3(-distance, 0.2f, 0);// Dịch chuyển Player tới Enemy

        // Thực hiện tấn công
        yield return null;

        Player pl = player.GetComponent<Player>();
        if (pl.state == PlayerState.BaseAttack)
        {
            pl.BaseAttack();
        }
    }
    IEnumerator EndSkill()
    {
        yield return new WaitForSeconds(0.3f);
        anotherButton.CloseSkillPanel();
        isProcess = false;
        currentPlayerIndex++;
        if (currentPlayerIndex >= players.Count)
        {
            Debug.Log("Hết lượt của Player. Đến lượt Enemy.");
            PlayerTurn(false);
        }
    }
}
