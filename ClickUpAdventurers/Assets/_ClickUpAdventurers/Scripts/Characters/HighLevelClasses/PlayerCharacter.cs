using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClickUpAdventurers
{
    public abstract class PlayerCharacter : Character
    {
        protected float begunTouchTime;         //Set when we tap the screen
        protected float endTouchTime;           //Set when we take the finger off the screen
        protected bool holdingTouch = false;    //Set to true while we hold the touch

        protected Touch currentTouch;           //The current touch that we will use for logistics
        protected BattleManager battleManager;  //Reference to the battle manager so we can stop logic when the game has ended

        public override void InheritedAwakeCalls()
        {
            throw new System.NotImplementedException();
        }

        public override void InheritedStartCalls()
        {
            battleManager = BattleManager.instance;
        }

        public override void InheritedUpdateCalls()
        {
            if (!battleManager.GameHasEnded)
            {
                CheckInput();
            }
        }

        private void CheckInput()
        {
            if (Input.touchCount > 0)
            {
                currentTouch = Input.GetTouch(0);
                if(currentTouch.phase == TouchPhase.Began)
                {
                    RotateTowardsTouchPos();
                    begunTouchTime = Time.time;
                    holdingTouch = true;
                }
                else if(currentTouch.phase == TouchPhase.Moved)
                {
                    RotateTowardsTouchPos();
                }
                else if (currentTouch.phase == TouchPhase.Ended)
                {
                    Attack();
                    endTouchTime = Time.time;
                    holdingTouch = false;
                }
            }
        }

        public abstract void Attack();

        public abstract void RotateTowardsTouchPos();
    }
}