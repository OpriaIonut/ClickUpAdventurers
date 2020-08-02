using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClickUpAdventurers
{
    //Base class for both players and enemies
    //Useful in case we want a list containing both, or if they have simillar behaviour
    public abstract class Character : MonoBehaviour
    {
        //These methods will be useful if you want to write Awake/Start/Update logic
        //If you write them normaly, then when you inherit the class you overwrite the fuctions without being able to call base functionality
        public abstract void InheritedAwakeCalls();
        public abstract void InheritedStartCalls();
        public abstract void InheritedUpdateCalls();
    }
}
