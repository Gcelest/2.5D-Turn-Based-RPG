using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;

public class BattleSystem : MonoBehaviour
{

    [SerializeField] private enum BattleState { Start, Selection, Battle, Won, Lost, Run }

    [Header("Battle State")]
    [SerializeField] private BattleState battleState;

    [Header("Spawn Points")]
    [SerializeField] private Transform[] partySpawnPoints;
    [SerializeField] private Transform[] enemySpawnPoints;

    [Header("Battlers")]
    [SerializeField] private List<BattleEntities> allBattlers = new List<BattleEntities>();
    [SerializeField] private List<BattleEntities> enemyBattlers = new List<BattleEntities>();
    [SerializeField] private List<BattleEntities> playerBattlers = new List<BattleEntities>();
    private int currentTurnIndex = 0;

    [Header("UI")]
    [SerializeField] private GameObject[] enemySelectionButtons;
    [SerializeField] private GameObject battleMenu;
    [SerializeField] private GameObject enemySelectionMenu;
    [SerializeField] private TextMeshProUGUI actionText;
    [SerializeField] private GameObject bottomTextPopUp;
    [SerializeField] private TextMeshProUGUI bottomText;


    private PartyManager partyManager;
    private EnemyManager enemyManager;
    private int currentPlayer;

    private const string ACTION_MESSAGE = "'s Action:";
    private const string WIN_MESSAGE = "You won the battle";
    private const string LOSE_MESSAGE = "Game Over";
    private const string OVERWORLD_SCENE = "OverworldScene";
    private const string SUCCESS_ESCAPE = "Party ran away";
    private const string FAILED_ESCAPE = "Party failed to escape";

    private const int TURN_DURATION = 2;
    private const int RUN_CHANCE = 50;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        partyManager = GameObject.FindFirstObjectByType<PartyManager>();
        enemyManager = GameObject.FindFirstObjectByType<EnemyManager>();
        StartBattle();
        CreatePartyEntities();
        CreateEnemyEntities();
        ShowBattleMenu();
        DetermineBattleOrder();
    }

    private void StartBattle()
    {
        allBattlers.Clear();

        // Add players and enemies to the battle
        AddPlayersToBattle();
        AddEnemiesToBattle();

        // Sort all battlers based on initiative
        DetermineBattleOrder();

        // Start the first turn
        currentTurnIndex = 0;
        ProceedToNextTurn();
    }

    private void AddPlayersToBattle()
    {
        foreach (var player in playerBattlers)
        {
            allBattlers.Add(player);
        }
    }

    private void AddEnemiesToBattle()
    {
        foreach (var enemy in enemyBattlers)
        {
            allBattlers.Add(enemy);
        }
    }

    private IEnumerator BattleRoutine()
{
    battleState = BattleState.Battle;
    bottomTextPopUp.SetActive(true);

    while (battleState == BattleState.Battle)
    {
        if (allBattlers.Count == 0)
            yield break;

        BattleEntities currentEntity = allBattlers[0];

        if (currentEntity.CurrHealth > 0)
        {
            if (currentEntity.IsPlayer)
            {
                currentPlayer = playerBattlers.IndexOf(currentEntity);
                ShowBattleMenu();
                yield break; // Wait for player's input
            }
            else
            {
                yield return StartCoroutine(EnemyTurnRoutine(currentEntity));
                yield return new WaitForSeconds(0.5f);
            }
        }

        RemoveDeadBattlers();

        // Requeue if alive
        if (currentEntity.CurrHealth > 0 && allBattlers.Contains(currentEntity))
        {
            allBattlers.RemoveAt(0);
            allBattlers.Add(currentEntity);
        }
        else if (allBattlers.Contains(currentEntity))
        {
            allBattlers.Remove(currentEntity);
        }

        // End conditions
        if (playerBattlers.Count <= 0)
        {
            battleState = BattleState.Lost;
            bottomText.text = LOSE_MESSAGE;
            yield return new WaitForSeconds(TURN_DURATION);
            SceneManager.LoadScene(OVERWORLD_SCENE);
            yield break;
        }

        if (enemyBattlers.Count <= 0)
        {
            battleState = BattleState.Won;
            bottomText.text = WIN_MESSAGE;
            yield return new WaitForSeconds(TURN_DURATION);
            SceneManager.LoadScene(OVERWORLD_SCENE);
            yield break;
        }

        yield return new WaitForSeconds(0.5f);
    }
}


    private IEnumerator EnemyTurnRoutine(BattleEntities enemy)
    {
        enemy.SetTarget(GetRandomPartyMember()); // select a random party member
        yield return StartCoroutine(AttackRoutine(allBattlers.IndexOf(enemy))); // enemy attacks
        RemoveDeadBattlers();
    }


    private IEnumerator AttackRoutine(int i)
    {
        if (allBattlers[i].IsPlayer == true)//players turn
        {
            BattleEntities currAttacker = allBattlers[i];
            if (allBattlers[currAttacker.Target].CurrHealth <= 0)
            {
                currAttacker.SetTarget(GetRandomEnemyMember());
            }
            BattleEntities currTarget = allBattlers[currAttacker.Target];
            AttackAction(currAttacker, currTarget);// attack selected enemy(attack action)
            yield return new WaitForSeconds(TURN_DURATION);// wait few seconds

            if (currTarget.CurrHealth <= 0)
            {
                bottomText.text = string.Format("{0} defeated {1}", currAttacker.Name, currTarget.Name);
                yield return new WaitForSeconds(TURN_DURATION);
                enemyBattlers.Remove(currTarget);// kill the enemy

                if (enemyBattlers.Count <= 0)
                {
                    battleState = BattleState.Won;
                    bottomText.text = WIN_MESSAGE;
                    yield return new WaitForSeconds(TURN_DURATION);
                    SceneManager.LoadScene(OVERWORLD_SCENE);
                    //Debug.Log("Return to Overworld");
                }
                // if no enemies remain
                // won
            }
        }

        //enemy turn
        if (i < allBattlers.Count && allBattlers[i].IsPlayer == false)
        {
            BattleEntities currAttacker = allBattlers[i];
            currAttacker.SetTarget(GetRandomPartyMember());// get random party member (target)
            BattleEntities currTarget = allBattlers[currAttacker.Target];

            AttackAction(currAttacker, currTarget); //attack selected party member (attack action)
            yield return new WaitForSeconds(TURN_DURATION);// wait few seconds

            if (currTarget.CurrHealth <= 0)
            {   // kill the party member
                bottomText.text = string.Format("{0} defeated {1}", currAttacker.Name, currTarget.Name);
                yield return new WaitForSeconds(TURN_DURATION);
                playerBattlers.Remove(currTarget);
                //allBattlers.Remove(currTarget);

                if (playerBattlers.Count <= 0)// if no party remain
                {
                    battleState = BattleState.Lost;
                    yield return new WaitForSeconds(TURN_DURATION);
                    Debug.Log(LOSE_MESSAGE);// lost
                }
            }

        }

    }

    private IEnumerator RunRoutine()
    {
        if (battleState == BattleState.Battle)
        {
            if (Random.Range(1, 101) >= RUN_CHANCE)
            {
                //ran away

                bottomText.text = SUCCESS_ESCAPE;//set our bottom text to tell us we ran
                battleState = BattleState.Run;//set our battle state to run
                allBattlers.Clear();// clear our all battlers list
                yield return new WaitForSeconds(TURN_DURATION);//wait for few sec
                SceneManager.LoadScene(OVERWORLD_SCENE);// load the overworld scene
                yield break;
            }
            else
            {
                //failed to run
                bottomText.text = FAILED_ESCAPE;//set our bottom text to say we failed
                yield return new WaitForSeconds(TURN_DURATION);//wait for few sec
            }
        }
    }

    private void RemoveDeadBattlers()
    {
        for (int i = 0; i < allBattlers.Count; i++)
        {
            if (allBattlers[i].CurrHealth <= 0)
            {
                allBattlers.RemoveAt(i);
            }
        }
    }

    private void CreatePartyEntities()
    {
        //get current party
        List<PartyMember> currentParty = new List<PartyMember>();
        currentParty = partyManager.GetAliveParty();

        for (int i = 0; i < currentParty.Count; i++)
        {
            BattleEntities tempEntity = new BattleEntities();

            tempEntity.SetEntityValues(currentParty[i].MemberName, currentParty[i].CurrHealth, currentParty[i].MaxHealth,
            currentParty[i].Initiative, currentParty[i].Strength, currentParty[i].Level, true);
            //spawning the visuals
            //set visuals starting values and assign it to the battle entity

            BattleVisuals tempBattleVisuals = Instantiate(currentParty[i].MemberBattleVisualPrefab,
            partySpawnPoints[i].position, Quaternion.identity).GetComponent<BattleVisuals>();

            tempBattleVisuals.SetStartingValues(currentParty[i].CurrHealth, currentParty[i].MaxHealth, currentParty[i].Level);
            tempEntity.BattleVisuals = tempBattleVisuals;

            allBattlers.Add(tempEntity);
            playerBattlers.Add(tempEntity);
        }
        //create battle entities for party member
        //assign values


    }

    private void CreateEnemyEntities()
    {
        //get current party
        List<Enemy> currentEnemies = new List<Enemy>();
        currentEnemies = enemyManager.GetCurrentEnemies();

        for (int i = 0; i < currentEnemies.Count; i++)
        {
            BattleEntities tempEntity = new BattleEntities();

            tempEntity.SetEntityValues(currentEnemies[i].EnemyName, currentEnemies[i].CurrHealth, currentEnemies[i].MaxHealth,
            currentEnemies[i].Initiative, currentEnemies[i].Strength, currentEnemies[i].Level, false);

            BattleVisuals tempBattleVisuals = Instantiate(currentEnemies[i].EnemyVisualPrefab,
            enemySpawnPoints[i].position, Quaternion.identity).GetComponent<BattleVisuals>();

            tempBattleVisuals.SetStartingValues(currentEnemies[i].MaxHealth, currentEnemies[i].MaxHealth, currentEnemies[i].Level);
            tempEntity.BattleVisuals = tempBattleVisuals;



            allBattlers.Add(tempEntity);
            enemyBattlers.Add(tempEntity);
        }
        //create battle entities for party member
        //assign values


    }

    public void ShowBattleMenu()
    {
        actionText.text = playerBattlers[currentPlayer].Name + ACTION_MESSAGE;
        battleMenu.SetActive(true);

    }

    public void ShowEnemySelectionMenu()
    {
        battleMenu.SetActive(false);
        SetEnemySelectionButtons();
        enemySelectionMenu.SetActive(true);
    }

    private void SetEnemySelectionButtons()
    {
        //disable all of our buttons
        for (int i = 0; i < enemySelectionButtons.Length; i++)
        {
            enemySelectionButtons[i].SetActive(false);
        }

        for (int j = 0; j < enemyBattlers.Count; j++)
        {
            enemySelectionButtons[j].SetActive(true);
            enemySelectionButtons[j].GetComponentInChildren<TextMeshProUGUI>().text = enemyBattlers[j].Name;
        }
        //enable buttons for each enemy
        // change the buttons text 
    }


    public void SelectEnemy(int _currentEnemy)
    {
        BattleEntities currentPlayerEntity = playerBattlers[currentPlayer];
        currentPlayerEntity.SetTarget(allBattlers.IndexOf(enemyBattlers[_currentEnemy]));
        currentPlayerEntity.BattleAction = BattleEntities.Action.Attack;

        battleMenu.SetActive(false);
        enemySelectionMenu.SetActive(false);

        StartCoroutine(AttackAndContinueTurn(currentPlayerEntity));
    }

    private IEnumerator AttackAndContinueTurn(BattleEntities attacker)
    {
        int index = allBattlers.IndexOf(attacker);
        yield return StartCoroutine(AttackRoutine(index));

        RemoveDeadBattlers();

        // Requeue attacker if still alive
        if (attacker.CurrHealth > 0 && allBattlers.Contains(attacker))
        {
            allBattlers.RemoveAt(index);
            allBattlers.Add(attacker);
        }

        yield return new WaitForSeconds(0.5f);

        StartCoroutine(BattleRoutine());
    }


    public void AttackAction(BattleEntities _currAttacker, BattleEntities _currTarget)
    {
        int damage = _currAttacker.Strength; //get damage
        _currAttacker.BattleVisuals.PlayAttackAnimation(); //play attack animation
        _currTarget.CurrHealth -= damage; // deal dmg
        _currTarget.BattleVisuals.PlayHitAnimation(); // play their hit anim
        _currTarget.UpdateUI(); // update UI
        bottomText.text = string.Format("{0} attacks {1} for {2} damage", _currAttacker.Name, _currTarget.Name, damage);
        SaveHealth();
    }

    private int GetRandomPartyMember()
    {
        List<int> partyMembers = new List<int>(); //create temp list of type int(index

        for (int i = 0; i < allBattlers.Count; i++)
        {
            if (allBattlers[i].IsPlayer == true && allBattlers[i].CurrHealth > 0)
            {
                partyMembers.Add(i);
            }
        }//find all party membeers then add them on our list

        return partyMembers[Random.Range(0, partyMembers.Count)]; //return random party member
    }

    private int GetRandomEnemyMember()
    {
        List<int> enemies = new List<int>(); //create temp list of type int(index

        for (int i = 0; i < allBattlers.Count; i++)
        {
            if (allBattlers[i].IsPlayer == false && allBattlers[i].CurrHealth > 0)
            {
                enemies.Add(i);
            }
        }//find all enemies then add them on our list

        return enemies[Random.Range(0, enemies.Count)]; //return random enemy
    }

    private void SaveHealth()
    {
        for (int i = 0; i < playerBattlers.Count; i++)
        {
            partyManager.SaveHealth(i, playerBattlers[i].CurrHealth);
        }
    }

    private void DetermineBattleOrder()
    {
        allBattlers.Sort((a, b) => b.Initiative.CompareTo(a.Initiative));

        Debug.Log("Battle Order:");
        foreach (var battler in allBattlers)
        {
            Debug.Log($"{battler.Name} - Initiative: {battler.Initiative}");
        }
    }

    private void ProceedToNextTurn()
    {
        if (allBattlers.Count == 0)
        {
            Debug.LogWarning("No battlers in battle.");
            return;
        }

        BattleEntity currentBattler = allBattlers[currentTurnIndex];
        Debug.Log($"It's {currentBattler.Name}'s turn (Initiative: {currentBattler.Initiative})");

        StartCoroutine(ExecuteTurn(currentBattler));
    }

    private IEnumerator ExecuteTurn(BattleEntity battler)
    {
        yield return battler.ExecuteTurn();

        // Advance to next battler
        currentTurnIndex = (currentTurnIndex + 1) % allBattlers.Count;
        ProceedToNextTurn();
    }

    public void SelectRunAction()
    {
        battleMenu.SetActive(false);

        BattleEntities currentPlayerEntity = playerBattlers[currentPlayer];
        currentPlayerEntity.BattleAction = BattleEntities.Action.Run;

        StartCoroutine(RunAndContinueTurn(currentPlayerEntity));
    }

    private IEnumerator RunAndContinueTurn(BattleEntities runner)
    {
        yield return StartCoroutine(RunRoutine());

        int index = allBattlers.IndexOf(runner);
        if (runner.CurrHealth > 0 && allBattlers.Contains(runner))
        {
            allBattlers.RemoveAt(index);
            allBattlers.Add(runner);
        }

        StartCoroutine(BattleRoutine());
    }


}



[System.Serializable]

public class BattleEntities
{
    public enum Action { Attack, Run }
    public Action BattleAction;


    public string Name;
    public int CurrHealth;
    public int MaxHealth;
    public int Initiative;
    public int Strength;
    public int Level;
    public bool IsPlayer;
    public BattleVisuals BattleVisuals;
    public int Target;

    public void SetEntityValues(string _name, int _currHealth, int _maxHealth, int _initiative, int _strength, int _level, bool _isPlayer)
    {
        Name = _name;
        CurrHealth = _currHealth;
        MaxHealth = _maxHealth;
        Initiative = _initiative;
        Strength = _strength;
        Level = _level;
        IsPlayer = _isPlayer;

    }

    public void SetTarget(int _target)
    {
        Target = _target;
    }

    public void UpdateUI()
    {
        BattleVisuals.ChangeHealth(CurrHealth);
    }
}