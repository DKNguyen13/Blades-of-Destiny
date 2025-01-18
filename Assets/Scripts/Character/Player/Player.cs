using UnityEngine;

public class Player : CharacterBase
{
    //Player status
    [Header("Elements")]
    [SerializeField] private TypeElement type = TypeElement.None;
    public PlayerState state;

    private int playerExperience;

    //Animator
    private Animator animator;

    //System hp, stamina
    private GameObject healthSystem;
    private GameObject hpBox;
    private GameObject staminaSystem;
    private GameObject staminaBox;

    private int originalDefense;
    private Color originalColor;// Lưu màu gốc của thanh HP

    private void Start()
    {
        
        animator = GetComponent<Animator>();//Animator
        InitializeElementStats();//Init player
        originalDefense = Defense; //Store original defense

        //Health system and health box
        healthSystem = transform.Find("System/Health System").gameObject;   
        hpBox = healthSystem.transform.Find("Hp box").gameObject;
        
        //Stamina system and stamina box
        staminaSystem = transform.Find("System/Stamina system").gameObject;
        staminaBox = staminaSystem.transform.Find("Stamina box").gameObject;

        SpriteRenderer hpBoxRenderer = hpBox.GetComponent<SpriteRenderer>();// Lưu lại màu gốc của thanh HP khi bắt đầu
        if (hpBoxRenderer != null)
        {
            originalColor = hpBoxRenderer.color; // Màu gốc
        }
    }

    //Update
    private void Update()
    {
        UpdateStaminaBar();
        UpdateHpBar();
    }

    // Constructor hoặc Init riêng cho Player
    public void InitPlayer(int maxHealth, int stamina, int damage, int defense, int criticalDmg, int criticalRate, int level, int playerExperience)
    {
        // Gọi hàm Init từ lớp cha
        Init(maxHealth, stamina , damage, defense, criticalDmg, criticalRate, level);

        this.playerExperience = playerExperience;
    }

    //Initialize element stats
    public void InitializeElementStats()
    {
        //Khởi tạo level 1
        if (type == TypeElement.Water)
        {
            InitPlayer(200, 200, 50, 50, 0, 0, 1,0);
        }
        else if(type == TypeElement.Ground){
            InitPlayer(230, 100, 20, 100, 0, 0, 1, 0);
        }
        else if (type == TypeElement.Fire)
        {
            InitPlayer(180, 200, 100, 0, 15, 5, 1, 0);
        }
        else if (type == TypeElement.Leaf)
        {
            InitPlayer(200, 200, 65, 35, 0, 0, 1, 0);
        }
        else if (type == TypeElement.Wind)
        {
            InitPlayer(200, 200, 70, 10, 10, 10, 1, 0);
        }
        else
        {
            InitPlayer(150, 150, 50, 50, 50, 50, 0, 0);
        }
    }

    // level up
    public void LevelUp(int exp)
    {
        playerExperience += exp; // Cộng kinh nghiệm
        while (playerExperience >= Level * 300)
        {
            playerExperience -= Level * 300;
            Level++;
            //Hp + damage tăng xử lý sau
        }
    }
    
    //Defense
    public void PlayerDefense()
    {
        if(state == PlayerState.Defense)
        {
            Defense = (int)(originalDefense * 1.5f);
            if(Defense > 0)
            {
                animator.SetTrigger("defense");
            }
            else
            {
                animator.SetTrigger("takehit");
            }
        }
        else
        {
            if(Defense > 1)
            {
                Defense = Mathf.RoundToInt(originalDefense / 1.5f);
            }
            else
            {
                Defense = 0;
            }
            originalDefense = Defense;
        }
    }

    //Base attack
    public void BaseAttack()
    {
        if (state == PlayerState.Skill1)
        {
            if (type == TypeElement.Water)
            {
                RestoreHealth((int)(MaxHealth * 0.25f));
            }
            animator.SetTrigger("skill1");
        }
        else if (state == PlayerState.Skill2)
        {
            animator.SetTrigger("skill2");
        }
        else if (state == PlayerState.Skill3)
        {
            animator.SetTrigger("skill3");
        }
        else if (state == PlayerState.Skill4)
        {
            animator.SetTrigger("specialSkill");
        }
        else
        {
            base.RestoreStamina((int)(MaxStamina*0.25f));
            UpdateStaminaBar();
            animator.SetTrigger("atk1");
        }
    }

    public bool TwinBatte()
    {
        return true;
    }

    //Take dmg
    public override void TakeDmg(int dmg, int criticalDmg, int criticalRate)
    {
        base.TakeDmg(dmg, criticalDmg, criticalRate);
        if (isDead())
        {
            state = PlayerState.Death;
            animator.SetTrigger("death");
            CurrentStamina = 0;
        }
        else if (state == PlayerState.Defense)
        {
            PlayerDefense();
        }
        else
        {
            animator.SetTrigger("takehit");
        }
    }

    //Stamina bar
    public void UpdateStaminaBar()
    {
        float staminaPercentage = (float)CurrentStamina/(float)MaxStamina;
        staminaBox.transform.localScale = new Vector3(staminaPercentage, 1f);
    }

    //Health bar
    public void UpdateHpBar()
    {
        float healthPercentage = (float)CurrentHealth / (float)MaxHealth;
        hpBox.transform.localScale = new Vector3(healthPercentage, 1f);

        // Kiểm tra nếu tỷ lệ HP dưới 30%
        if (healthPercentage < 0.3f)
        {
            // Tạo hiệu ứng Ping Pong màu
            float pingPongValue = Mathf.PingPong(Time.time * 2f, 1f); // Thay đổi tốc độ nhấp nháy bằng cách thay đổi hệ số nhân với Time.time
            /*
            Color darkGreen = new Color(0.0f, 0.5f, 0.0f); // RGB(0, 128, 0)
            Color mysticGreen = new Color(0.1f, 0.3f, 0.2f); // RGB(26, 77, 51)
            */
            Color barColor = new Color(0.0f, 0.6f, 0.2f); // RGB(0, 153, 51)
            Color pingPongColor = Color.Lerp(barColor, Color.blue, pingPongValue);
            SetColor(pingPongColor); // Áp dụng màu cho thanh HP
        }
        else
        {
            // Nếu HP trên 30%, gắn lại màu gốc
            SetColor(originalColor);
        }
    }

    //Use special skill
    public void UseSpeciallSkill(int amount, string ani)
    {
        if (CurrentStamina >= amount)
        {
            CurrentStamina -= amount;
            animator.SetTrigger(ani);
        }
        else
        {
            Debug.Log("not enough stamina");
        }
    }

    //Set color
    public void SetColor(Color color)
    {
        // Đặt màu cho SpriteRenderer của thanh HP
        SpriteRenderer hpBoxRenderer = hpBox.GetComponent<SpriteRenderer>();
        if (hpBoxRenderer != null)
        {
            hpBoxRenderer.color = color;
        }
    }

    //Getter, Setter
    public int PlayerExperience { get => playerExperience; set => playerExperience = value;}
    public TypeElement ElementType { get => type; set => type = value; }
}
