using System.Collections.Generic;
using UnityEngine;

public class JoinableCharacter : MonoBehaviour
{

    public PartyMemberInfo memberToJoin;
    [SerializeField] private GameObject interactPrompt;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CheckIfJoined();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ShowInteractPrompt(bool showPrompt)
    {
        if (showPrompt == true)
        {
            interactPrompt.SetActive(true);
        }
        else
        {
            interactPrompt.SetActive(false);
        }
    }

    public void CheckIfJoined()
    {
        var partyManager = GameObject.FindFirstObjectByType<PartyManager>();
        if (partyManager == null) return;

        List<PartyMember> currParty = GameObject.FindFirstObjectByType<PartyManager>().GetCurrentParty();

        for (int i = 0; i < currParty.Count; i++)
        {
            if (currParty[i].MemberName == memberToJoin.MemberName)
            {
                gameObject.SetActive(false);
            }
        }
    }

}
