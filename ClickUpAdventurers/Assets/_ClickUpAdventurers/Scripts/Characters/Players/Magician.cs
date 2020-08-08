using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClickUpAdventurers
{
    public class Magician : PlayerCharacter
    {
        private void Start()
        {
            base.InheritedStartCalls();
        }

        private void Update()
        {
            base.InheritedUpdateCalls();
        }

        public override void Attack()
        {
            
        }
    }
}