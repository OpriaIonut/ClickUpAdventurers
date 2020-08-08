using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ClickUpAdventurers
{
    public abstract class PlayerCharacter : Character
    {
        protected float begunTouchTime;         //Set when we tap the screen
        protected float endTouchTime;           //Set when we take the finger off the screen
        protected bool holdingTouch = false;    //Set to true while we hold the touch

        protected Touch currentTouch;           //The current touch that we will use for logistics
        protected BattleManager battleManager;  //Reference to the battle manager so we can stop logic when the game has ended

        [HideInInspector] public bool isSelected;   //The current player is the one that attacks

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
            if (!battleManager.GameHasEnded && !battleManager.GamePaused)
            {
                CheckInput();
            }

            //If the game is paused then we want to add the paused time to our time variables (touch time)
            if(battleManager.GamePaused)
            {
                resetValuesAfterPause = true;
            }
            else if(resetValuesAfterPause)
            {
                begunTouchTime += battleManager.PausedTimeDiff;
                endTouchTime += battleManager.PausedTimeDiff;
                resetValuesAfterPause = false;
            }
        }

        private bool resetValuesAfterPause = false;
        private bool clickedUI;
        private void CheckInput()
        {
            if (Input.touchCount > 0)
            {
                currentTouch = Input.GetTouch(0);
                //Check if the current player is the selected one and we haven't clicked on an ui element
                if (isSelected && EventSystem.current.IsPointerOverGameObject(currentTouch.fingerId) == false)
                {
                    if (currentTouch.phase == TouchPhase.Began)
                    { 
                        //While we are holding the tap, don't allow the player to change character
                        CharacterChanger.instance.canChange = false;
                        RotateTowardsTouchPos();
                        begunTouchTime = Time.time;
                        holdingTouch = true;
                    }
                    else if (currentTouch.phase == TouchPhase.Moved)
                    {
                        RotateTowardsTouchPos();
                    }
                    else if (currentTouch.phase == TouchPhase.Ended && holdingTouch)
                    {
                        //We released the touch so now the player can change the character if he wants to
                        CharacterChanger.instance.canChange = true;
                        Attack();
                        endTouchTime = Time.time;
                        holdingTouch = false;
                    }
                }
            }
        }

        public abstract void Attack();

        public virtual void ChangeBasePosition(Vector3 position, Quaternion rot)
        {
            transform.position = position;
            transform.rotation = rot;
        }

        public virtual void RotateTowardsTouchPos()
        {
            Vector3 touch = currentTouch.position;
            //The touch is in screen space so we need to convert to world space
            Vector3 worldTouch = Camera.main.ScreenToWorldPoint(new Vector3(touch.x, touch.y, Camera.main.nearClipPlane + 5.0f));
            //Get vector from the position to the touch position
            Vector3 lookPos = worldTouch - transform.position;
            lookPos.y = 0;
            //Rotate the object
            transform.rotation = Quaternion.LookRotation(lookPos);
        }
    }
}