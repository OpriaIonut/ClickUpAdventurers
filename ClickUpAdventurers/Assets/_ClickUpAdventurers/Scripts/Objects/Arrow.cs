using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClickUpAdventurers
{
    public class Arrow : RangedAttackObject
    {
        private void OnTriggerEnter(Collider other)
        {
            Debug.Log(other.gameObject.name);
        }
    }
}