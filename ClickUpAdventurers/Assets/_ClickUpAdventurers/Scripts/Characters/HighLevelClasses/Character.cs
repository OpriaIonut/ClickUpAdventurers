using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClickUpAdventurers
{
    public abstract class Character : MonoBehaviour
    {
        public abstract void InheritedAwakeCalls();
        public abstract void InheritedStartCalls();
        public abstract void InheritedUpdateCalls();
    }
}
