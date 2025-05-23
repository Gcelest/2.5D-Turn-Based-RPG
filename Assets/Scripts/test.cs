// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using TMPro;
// using Unity.VisualScripting;
// using UnityEngine.SceneManagement;

// public class BattleSystem : MonoBehaviour
// {
//     [SerializeField] private enum BattleState { Start, Selection, Battle, Won, Lost, Run }

//     [Header("Battle State")]
//     [SerializeField] private BattleState battleState;

//     [Header("Spawn Points")]
//     [SerializeField] private Transform[] partySpawnPoints;
//     [SerializeField] private Transform[] enemySpawnPoints;

//     [Header("Battlers")]
//     [SerializeField] private List<BattleEntities> allBattlers = new List<BattleEntities>();
//     [SerializeField] private List<BattleEntities> enemyBattlers = new List<BattleEntities>();
//     [SerializeField] private List<BattleEntities> playerBattlers = new List<BattleEntities>();


//     [Header("UI")]
//     [SerializeField] private GameObject[] enemySelectionButtons;
//     [SerializeField] private GameObject battleMenu;
//     [SerializeField] private GameObject enemySelectionMenu;
//     [SerializeField] private TextMeshProUGUI actionText;
//     [SerializeField] private GameObject bottomTextPopUp;
//     [SerializeField] private TextMeshProUGUI bottomText;


//     private PartyManager partyManager;
//     private EnemyManager enemyManager;
//     private int currentPlayer;

//     private const string ACTION_MESSAGE = "'s Action:";
//     private const string WIN_MESSAGE = "You won the battle";
//     private const string LOSE_MESSAGE = "Game Over";
//     private const string OVERWORLD_SCENE = "OverworldScene";
//     private const string SUCCESS_ESCAPE = "Party ran away";
//     private const string FAILED_ESCAPE = "Party failed to escape";

//     private const int TURN_DURATION = 2;
//     private const int RUN_CHANCE = 50;

//     // Start is called once before the first execution of Update after the MonoBehaviour is created
//     void Start()
//     {
//         partyManager = GameObject.FindFirstObjectByType<PartyManager>();
//         enemyManager = GameObject.FindFirstObjectByType<EnemyManager>();

//         CreatePartyEntities();
//         CreateEnemyEntities();
//         ShowBattleMenu();
//         DetermineBattleOrder();
//     }

//     private IEnumerator BattleRoutine()
//     {
//         enemySelectionMenu.SetActive(false);// enemy selection menu disabled
//         battleState = BattleState.Battle;//change our state to the battle state
//         bottomTextPopUp.SetActive(true);//enable our bottom text
//                                         // loop through all our battlers
//                                         // do their actions

//         for (int i = 0; i < allBattlers.Count; i++)
//         {
//             if(battleState == BattleState.Battle && allBattlers[i].CurrHealth > 0)
//                 {
//                 switch (allBattlers[i].BattleAction)
//                 {
//                     case BattleEntities.Action.Attack:
//                         yield return StartCoroutine(AttackRoutine(i));
//                         //Debug.Log(allBattlers[i].Name + " is attacking " + allBattlers[allBattlers[i].Target].Name);
//                         break;
//                     case BattleEntities.Action.Run:
//                     yield return StartCoroutine(RunRoutine());
//                         break;
//                     default:
//                         Debug.Log("Error battle");
//                         break;
//                 }
//             }
//         }

//         RemoveDeadBattlers();

//         if (battleState == BattleState.Battle)
//         {
//             bottomTextPopUp.SetActive(false);
//             currentPlayer = 0;
//             ShowBattleMenu();
//         }
//         // if not won or lost, repeat the loop in battle menu    
//         yield return null;
//     }

//     private IEnumerator AttackRoutine(int i)
//     {
//         if (allBattlers[i].IsPlayer == true)//players turn
//         {
//             BattleEntities currAttacker = allBattlers[i];
//             if(allBattlers[currAttacker.Target].CurrHealth <= 0)
//             {
//                 currAttacker.SetTarget(GetRandomEnemyMember());
//             }
//             BattleEntities currTarget = allBattlers[currAttacker.Target];
//             AttackAction(currAttacker, currTarget);// attack selected enemy(attack action)
//             yield return new WaitForSeconds(TURN_DURATION);// wait few seconds

//             if (currTarget.CurrHealth <= 0)
//             {
//                 bottomText.text = string.Format("{0} defeated {1}", currAttacker.Name, currTarget.Name);
//                 yield return new WaitForSeconds(TURN_DURATION);
//                 enemyBattlers.Remove(currTarget);// kill the enemy

//                 if (enemyBattlers.Count <= 0)
//                 {
//                     battleState = BattleState.Won;
//                     bottomText.text = WIN_MESSAGE;
//                     yield return new WaitForSeconds(TURN_DURATION);
//                     SceneManager.LoadScene(OVERWORLD_SCENE);
//                     //Debug.Log("Return to Overworld");
//                 }
//                  // if no enemies remain
//                   // won
//             }
//         }

//         //enemy turn
//         if(i < allBattlers.Count && allBattlers[i].IsPlayer == false)
//         {
//             BattleEntities currAttacker = allBattlers[i];
//             currAttacker.SetTarget(GetRandomPartyMember());// get random party member (target)
//             BattleEntities currTarget = allBattlers[currAttacker.Target];
            
//             AttackAction(currAttacker,currTarget); //attack selected party member (attack action)
//             yield return new WaitForSeconds(TURN_DURATION);// wait few seconds
            
//             if(currTarget.CurrHealth <= 0)
//             {   // kill the party member
//                 bottomText.text = string.Format("{0} defeated {1}", currAttacker.Name, currTarget.Name);
//                 yield return new WaitForSeconds(TURN_DURATION);
//                 playerBattlers.Remove(currTarget);
//                 //allBattlers.Remove(currTarget);

//                 if(playerBattlers.Count <= 0)// if no party remain
//                 {
//                     battleState = BattleState.Lost;
//                     yield return new WaitForSeconds(TURN_DURATION);
//                     Debug.Log(LOSE_MESSAGE);// lost
//                 }
//             }
            
//         }
        
//     }

//     private IEnumerator RunRoutine()
//     {
//         if(battleState == BattleState.Battle)
//         {
//             if(Random.Range(1,101) >= RUN_CHANCE)
//             {
//                 //ran away

//                 bottomText.text = SUCCESS_ESCAPE;//set our bottom text to tell us we ran
//                 battleState = BattleState.Run;//set our battle state to run
//                 allBattlers.Clear();// clear our all battlers list
//                 yield return new WaitForSeconds(TURN_DURATION);//wait for few sec
//                 SceneManager.LoadScene(OVERWORLD_SCENE);// load the overworld scene
//                 yield break;
//             }
//             else
//             {
//                 //failed to run
//                 bottomText.text = FAILED_ESCAPE;//set our bottom text to say we failed
//                 yield return new WaitForSeconds(TURN_DURATION);//wait for few sec
//             }
//         }
//     }

//     private void RemoveDeadBattlers()
//     {
//         for (int i = 0; i < allBattlers.Count; i++)
//         {
//             if (allBattlers[i].CurrHealth <= 0)
//             {
//                 allBattlers.RemoveAt(i);
//             }
//         }
//     }

//     private void CreatePartyEntities()
//     {
//         //get current party
//         List<PartyMember> currentParty = new List<PartyMember>();
//         currentParty = partyManager.GetAliveParty();

//         for (int i = 0; i < currentParty.Count; i++)
//         {
//             BattleEntities tempEntity = new BattleEntities();

//             tempEntity.SetEntityValues(currentParty[i].MemberName, currentParty[i].CurrHealth, currentParty[i].MaxHealth,
//             currentParty[i].Initiative, currentParty[i].Strength, currentParty[i].Level, true);
//             //spawning the visuals
//             //set visuals starting values and assign it to the battle entity

//             BattleVisuals tempBattleVisuals = Instantiate(currentParty[i].MemberBattleVisualPrefab,
//             partySpawnPoints[i].position, Quaternion.identity).GetComponent<BattleVisuals>();

//             tempBattleVisuals.SetStartingValues(currentParty[i].CurrHealth, currentParty[i].MaxHealth, currentParty[i].Level);
//             tempEntity.BattleVisuals = tempBattleVisuals;

//             allBattlers.Add(tempEntity);
//             playerBattlers.Add(tempEntity);
//         }
//         //create battle entities for party member
//         //assign values


//     }

//     private void CreateEnemyEntities()
//     {
//         //get current party
//         List<Enemy> currentEnemies = new List<Enemy>();
//         currentEnemies = enemyManager.GetCurrentEnemies();

//         for (int i = 0; i < currentEnemies.Count; i++)
//         {
//             BattleEntities tempEntity = new BattleEntities();

//             tempEntity.SetEntityValues(currentEnemies[i].EnemyName, currentEnemies[i].CurrHealth, currentEnemies[i].MaxHealth,
//             currentEnemies[i].Initiative, currentEnemies[i].Strength, currentEnemies[i].Level, false);

//             BattleVisuals tempBattleVisuals = Instantiate(currentEnemies[i].EnemyVisualPrefab,
//             enemySpawnPoints[i].position, Quaternion.identity).GetComponent<BattleVisuals>();

//             tempBattleVisuals.SetStartingValues(currentEnemies[i].MaxHealth, currentEnemies[i].MaxHealth, currentEnemies[i].Level);
//             tempEntity.BattleVisuals = tempBattleVisuals;



//             allBattlers.Add(tempEntity);
//             enemyBattlers.Add(tempEntity);
//         }
//         //create battle entities for party member
//         //assign values


//     }

//     public void ShowBattleMenu()
//     {
//         actionText.text = playerBattlers[currentPlayer].Name + ACTION_MESSAGE;
//         battleMenu.SetActive(true);

//     }

//     public void ShowEnemySelectionMenu()
//     {
//         battleMenu.SetActive(false);
//         SetEnemySelectionButtons();
//         enemySelectionMenu.SetActive(true);
//     }

//     private void SetEnemySelectionButtons()
//     {
//         //disable all of our buttons
//         for (int i = 0; i < enemySelectionButtons.Length; i++)
//         {
//             enemySelectionButtons[i].SetActive(false);
//         }

//         for (int j = 0; j < enemyBattlers.Count; j++)
//         {
//             enemySelectionButtons[j].SetActive(true);
//             enemySelectionButtons[j].GetComponentInChildren<TextMeshProUGUI>().text = enemyBattlers[j].Name;
//         }
//         //enable buttons for each enemy
//         // change the buttons text 
//     }


//     public void SelectEnemy(int _currentEnemy)
//     {
//         BattleEntities currentPlayerEntity = playerBattlers[currentPlayer]; //setting the current members target
//         currentPlayerEntity.SetTarget(allBattlers.IndexOf(enemyBattlers[_currentEnemy])); //tell the battle system this member intends to attack

//         currentPlayerEntity.BattleAction = BattleEntities.Action.Attack; //increment through our party members
//         currentPlayer++;

    
//         // if (currentPlayer >= playerBattlers.Count) //if all players have selected an action
//         // {
//         //     StartCoroutine(BattleRoutine());//start the battle
//         // }
//         // else //else
//         // {
//         //     enemySelectionMenu.SetActive(false); //show the battle menu for the next player
//         //     ShowBattleMenu();
//         // }

//     }

//     public void AttackAction(BattleEntities _currAttacker, BattleEntities _currTarget)
//     {
//         int damage = _currAttacker.Strength; //get damage
//         _currAttacker.BattleVisuals.PlayAttackAnimation(); //play attack animation
//         _currTarget.CurrHealth -= damage; // deal dmg
//         _currTarget.BattleVisuals.PlayHitAnimation(); // play their hit anim
//         _currTarget.UpdateUI(); // update UI
//         bottomText.text = string.Format("{0} attacks {1} for {2} damage", _currAttacker.Name, _currTarget.Name, damage);
//         SaveHealth();
//     }

//     private int GetRandomPartyMember()
//     {
//         List<int> partyMembers = new List<int>(); //create temp list of type int(index

//         for (int i = 0; i < allBattlers.Count; i++)
//         {
//             if (allBattlers[i].IsPlayer == true)
//             {
//                 partyMembers.Add(i);
//             }
//         }//find all party membeers then add them on our list

//         return partyMembers[Random.Range(0, partyMembers.Count)]; //return random party member
//     }

//     private int GetRandomEnemyMember()
//     {
//         List<int> enemies = new List<int>(); //create temp list of type int(index

//         for (int i = 0; i < allBattlers.Count; i++)
//         {
//             if (allBattlers[i].IsPlayer == false)
//             {
//                 enemies.Add(i);
//             }
//         }//find all enemies then add them on our list

//         return enemies[Random.Range(0, enemies.Count)]; //return random enemy
//     }

//     private void SaveHealth()
//     {
//         for (int i = 0; i < playerBattlers.Count; i++)
//         {
//             partyManager.SaveHealth(i, playerBattlers[i].CurrHealth);
//         }
//     }

//     private void DetermineBattleOrder()
//     {
//         //sort list by initiative in ascending order
//         allBattlers.Sort((bi1, bi2) => -bi1.Initiative.CompareTo(bi2.Initiative));


//     }

//     public void SelectRunAction()
//     {
//         battleState = BattleState.Selection;

//         BattleEntities currentPlayerEntity = playerBattlers[currentPlayer]; //setting the current members target
        
//         currentPlayerEntity.BattleAction = BattleEntities.Action.Run; // tell battle system to run
        
//         battleMenu.SetActive(false);
//         //increment through our party members
//         currentPlayer++;

//         if (currentPlayer >= playerBattlers.Count) //if all players have selected an action
//         {
//             StartCoroutine(BattleRoutine());//start the battle
//         }
//         else //else
//         {
//             enemySelectionMenu.SetActive(false); //show the battle menu for the next player
//             ShowBattleMenu();
//         }

//     }

// }



// [System.Serializable]

// public class BattleEntities
// {
//     public enum Action { Attack, Run }
//     public Action BattleAction;


//     public string Name;
//     public int CurrHealth;
//     public int MaxHealth;
//     public int Initiative;
//     public int Strength;
//     public int Level;
//     public bool IsPlayer;
//     public BattleVisuals BattleVisuals;
//     public int Target;

//     public void SetEntityValues(string _name, int _currHealth, int _maxHealth, int _initiative, int _strength, int _level, bool _isPlayer)
//     {
//         Name = _name;
//         CurrHealth = _currHealth;
//         MaxHealth = _maxHealth;
//         Initiative = _initiative;
//         Strength = _strength;
//         Level = _level;
//         IsPlayer = _isPlayer;

//     }

//     public void SetTarget(int _target)
//     {
//         Target = _target;
//     }

//     public void UpdateUI()
//     {
//         BattleVisuals.ChangeHealth(CurrHealth);
//     }
// }