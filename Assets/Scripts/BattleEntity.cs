using System;
using System.Collections;
using UnityEngine;

public class BattleEntity : MonoBehaviour
{
    public string Name;
    public int Initiative;
    public bool IsPlayer;

    public virtual IEnumerator ExecuteTurn()
    {
        Debug.Log($"{Name} is taking their turn...");
        // Default behavior – override this in Player or Enemy classes
        yield return new WaitForSeconds(1.0f);
        Debug.Log($"{Name} ends their turn.");
    }

    public static implicit operator BattleEntity(BattleEntities v)
    {
        throw new NotImplementedException();
    }
}
