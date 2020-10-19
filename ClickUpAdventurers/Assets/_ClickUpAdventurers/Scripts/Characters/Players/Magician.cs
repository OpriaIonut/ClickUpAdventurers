using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ClickUpAdventurers
{
    public class Magician : PlayerCharacter
    {
        public GameObject magicBallPrefab;
        public Transform firePoint;
        public float castTime = 2.0f;   //The time it takes to fire the magic ball
        public int damage;
        public float ballSizeMultiplier = 1;

        public Image castTimeImage;

        private float castingStartTime;
        private bool casting = false;

        private void Start()
        {
            base.InheritedStartCalls();

            castTimeImage.transform.parent.gameObject.SetActive(false);
        }

        private bool resetValuesPause = false;
        private void Update()
        {
            base.InheritedUpdateCalls();
            if (!BattleManager.instance.GamePaused)
            {
                //If the game was paused add the time that it was paused in order to not mess up the formulas
                if(resetValuesPause)
                {
                    castingStartTime += BattleManager.instance.PausedTimeDiff;
                    resetValuesPause = false;
                }    

                if (casting)
                {
                    //If we are casting
                    if (Time.time - castingStartTime >= castTime)
                    {
                        //And finished casting, then spawn the magic ball
                        Transform clone = Instantiate(magicBallPrefab).transform;
                        clone.rotation = transform.rotation;
                        clone.position = firePoint.position;
                        clone.GetComponent<MagicBall>().damage = damage;
                        clone.localScale *= ballSizeMultiplier;
                        castTimeImage.transform.parent.gameObject.SetActive(false);
                        casting = false;
                        CharacterChanger.instance.canChange = true;
                    }
                    else
                    {
                        //Otherwise fill the image bar
                        float passedTime = (Time.time - castingStartTime) / castTime;
                        castTimeImage.rectTransform.localScale = new Vector3(passedTime, 1, 1);
                    }
                }
            }
            else
            {
                resetValuesPause = true;
            }
        }

        public override void Attack()
        {
            if(!casting)
            {
                CharacterChanger.instance.canChange = false;
                casting = true;
                castingStartTime = Time.time;
                castTimeImage.transform.parent.gameObject.SetActive(true);
            }
        }

        public override void RotateTowardsTouchPos()
        {
            if (!casting)
            {
                base.RotateTowardsTouchPos();
            }
        }
    }
}