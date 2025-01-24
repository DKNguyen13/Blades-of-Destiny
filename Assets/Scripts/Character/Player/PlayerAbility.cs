using UnityEngine;

public class PlayerAbility : MonoBehaviour
{
    [SerializeField] private PlayerAbilities ability;
    private GameManager gameManager;
    private string abiDescribe;
    private Player player;
    private void Awake()
    {
        player = GetComponent<Player>();
        gameManager = FindAnyObjectByType<GameManager>();
    }

    //Start
    private void Start()
    {
        SetAbility();
    }

    //Update
    private void Update()
    {
        
    }

    //Activate abilities
    public void ActivateAbilities()
    {
        if(ability == PlayerAbilities.ContinousRecovery)
        {
            bool hp = player.CurrentHealth < (int)(player.MaxHealth * 0.15f); //Hp is below 15%
            if(hp)
            {

            }
        }
    }

    //Set ability
    public void SetAbility()
    {
        if(player.ElementType == TypeElement.None)
        {
            ability =  PlayerAbilities.None;
            abiDescribe = "None";
        }
        else if(player.ElementType == TypeElement.Water)
        {
            ability = PlayerAbilities.ContinousRecovery;
            abiDescribe = "When hp is below 15%, self-healing ability will be activated (restore 5% hp and stamina each turn until full), hp recovery skill increases hp by 10%";
        }
        else if(player.ElementType == TypeElement.Fire)
        {
            ability = PlayerAbilities.Rage;
            abiDescribe = "When hp is below 10%, all stats of dmg, critical rate increase by 50%";
        }
        else if (player.ElementType == TypeElement.Leaf)
        {   
            ability = PlayerAbilities.Immunity;
            abiDescribe = "Immune to harmful statuses (except burn and freeze)";
        }
        else if(player.ElementType == TypeElement.Ground)
        {
            ability = PlayerAbilities.Indomitable;
            abiDescribe = "Has a 10% chance to be immune to damage and restore hp equal to the amount of damage dealt by the opponent";
        }
        else if(player.ElementType == TypeElement.Wind)
        {

        }
    }
}
