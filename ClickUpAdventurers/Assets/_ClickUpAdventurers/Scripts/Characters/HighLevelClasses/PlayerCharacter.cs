using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClickUpAdventurers
{
    public abstract class PlayerCharacter : Character
    {
        protected float begunTouchTime;
        protected float endTouchTime;
        protected bool holdingTouch = false;

        protected Touch currentTouch;

        public override void InheritedAwakeCalls()
        {
            throw new System.NotImplementedException();
        }

        public override void InheritedStartCalls()
        {
            throw new System.NotImplementedException();
        }

        public override void InheritedUpdateCalls()
        {
            CheckInput();
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