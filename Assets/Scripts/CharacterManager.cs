using Unity.VisualScripting;
using UnityEngine;
using TMPro;
using System.Collections.Generic;
public class CharacterManager : MonoBehaviour
{

    [SerializeField] private GameObject joinPopUp;
    [SerializeField] private TextMeshProUGUI joinPopUpText;

    private bool infrontOfPartyMember;
    private GameObject joinableMember;
    private PlayerControls playerControls;
    private List<GameObject> overWorldCharacters = new List<GameObject>();

    
    private const string NPC_JOINABLE_TAG = "JoinableNPC";
    private const string PARTY_JOINED_MESSAGE = " Joined The Party!";

    private void Awake()
    {
        playerControls = new PlayerControls();
    }
    public void Start()
    {
        playerControls.Player.Interact.performed += _ => Interact();
        SpawnOverWorldMembers();
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Interact(){
        if (infrontOfPartyMember == true && joinableMember != null)
        {
            MemberJoined(joinableMember.GetComponent<JoinableCharacter>().memberToJoin);// add member
            infrontOfPartyMember = false;
            joinableMember = null;
        }
    }

    private void MemberJoined(PartyMemberInfo _partyMember)
    {
        GameObject.FindFirstObjectByType<PartyManager>().AddMemberToPartyByName(_partyMember.MemberName);//add member
        joinableMember.GetComponent<JoinableCharacter>().CheckIfJoined();//disable join member
        joinPopUp.SetActive(true);//join pop up
        joinPopUpText.text = _partyMember.MemberName + PARTY_JOINED_MESSAGE;// adding an overworld member
        SpawnOverWorldMembers();
    }

    private void SpawnOverWorldMembers()
    {
        for (int i = 0; i < overWorldCharacters.Count; i++)
        {
            Destroy(overWorldCharacters[i]);
        }
        overWorldCharacters.Clear();

        List<PartyMember> currentParty = GameObject.FindFirstObjectByType<PartyManager>().GetCurrentParty();    
        
        for (int i = 0; i < currentParty.Count; i++)
        {
            if (i==0)
            {
                GameObject player = gameObject;//get player

                GameObject playerVisual = Instantiate(currentParty[i].MemberOverworldVisualPrefab, 
                player.transform.position, Quaternion.identity);//spawn member visual
                
                playerVisual.transform.SetParent(player.transform);
                playerVisual.transform.localRotation = Quaternion.identity;
                
                player.GetComponent<PlayerController>().SetOverWorldVisuals(playerVisual.GetComponent<Animator>(), 
                playerVisual.GetComponent<SpriteRenderer>(), playerVisual.transform.localScale);// assign player controller vallues
                playerVisual.GetComponent<MemberFollowAI>().enabled = false;
                
                overWorldCharacters.Add(playerVisual);// add overworld character visual to the list
            }
            else
            {
                Vector3 positionToSpawn = transform.position;
                positionToSpawn.x -= 1; // get follower spawn position

                GameObject tempFollower = Instantiate(currentParty[i].MemberOverworldVisualPrefab,
                positionToSpawn, Quaternion.identity); //spawn follower

                tempFollower.GetComponent<MemberFollowAI>().SetFollowDistance(i); // set follow ai setting
                overWorldCharacters.Add(tempFollower); //add follower visual to list

            }
        }
    
    }



    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == NPC_JOINABLE_TAG)
        {
            //enable prompt
            infrontOfPartyMember = true;
            joinableMember = other.gameObject;
            joinableMember.GetComponent<JoinableCharacter>().ShowInteractPrompt(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.tag == NPC_JOINABLE_TAG)
        {
            //enable prompt
            infrontOfPartyMember = false;
            joinableMember.GetComponent<JoinableCharacter>().ShowInteractPrompt(false);
            joinableMember = null;
        }
    }
}
