using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyBase : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
            //If an enemy touched the player, then end the game
            BattleManager.instance.EndGame();
        }
    }
}
