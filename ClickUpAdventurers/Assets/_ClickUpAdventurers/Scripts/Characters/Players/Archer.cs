using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ClickUpAdventurers
{
    public class Archer : PlayerCharacter
    {
        public GameObject arrowPrefab;
        public Transform firePoint;     //Location from which we shoot the arrow

        [Header("Special Properties")]
        [Tooltip("The time it takes to reach max accuracy")]
        public float accuracyMaxTime = 3.0f;
        [Tooltip("The starting angle, when accuracy is lowest")]
        public float minAccuracyAngle = 45.0f;
        public float shootCooldownTime = 0.5f;

        private float accuracyAngle;    //Current accuracy angle
        private float lastAttackTime;

        private void Start()
        {
            InheritedStartCalls();

            lastAttackTime = 0;
        }

        private void Update()
        {
            InheritedUpdateCalls();

            if(holdingTouch && Time.time - lastAttackTime > shootCooldownTime)
            {
                DrawAccuracy();
            }
        }

        #region Own Methods

        private void OnTriggerEnter(Collider other)
        {
            if(other.tag == "Enemy")
            {
                //If an enemy touched the player, then end the game
                BattleManager.instance.EndGame();
            }
        }

        private void DrawAccuracy()
        {
            //Get the time in which we held the tap, in a [0-1] range
            float accuracyTime = (accuracyMaxTime - (Time.time - begunTouchTime)) / accuracyMaxTime;
            //It is negative when accuracy is maxed out so we need to check for it
            if (accuracyTime < 0)
                accuracyTime = 0;
            //Find the actual angle
            accuracyAngle = accuracyTime * minAccuracyAngle;

            //Create a point that we will use to draw the accuracy
            Vector3 targetPoint = transform.position + transform.forward * 5;

            //Convert the angle to radians because Mathf.Cos uses radians
            float radAngle = accuracyAngle * Mathf.Deg2Rad;


            //Use a multiplication matrix to rotate the point
            Vector3 rightPoint = new Vector3(targetPoint.x * Mathf.Cos(radAngle) + 0 + targetPoint.z * Mathf.Sin                                        (radAngle), 
                                0 + targetPoint.y + 0, 
                                targetPoint.x * -Mathf.Sin(radAngle) + 0 + targetPoint.z * Mathf.Cos(radAngle));

            //rotate it in the other direction
            Vector3 leftPoint = new Vector3(targetPoint.x * Mathf.Cos(-radAngle) + 0 + targetPoint.z * Mathf.Sin(-radAngle),
                                0 + targetPoint.y + 0,
                                targetPoint.x * -Mathf.Sin(-radAngle) + 0 + targetPoint.z * Mathf.Cos(-radAngle));

            //Draw the lines. To do: make an actual graphical representation
            Debug.DrawLine(transform.position, rightPoint, Color.red);
            Debug.DrawLine(transform.position, leftPoint, Color.red);
        }

        #endregion

        #region Inherited Methods

        //Called when releasing a tap
        public override void Attack()
        {
            if (Time.time - lastAttackTime > shootCooldownTime)
            {
                //If the cooldown has passed, then shoot
                Transform clone = Instantiate(arrowPrefab).GetComponent<Transform>();
                clone.position = firePoint.position;

                float randomRot = Random.Range(-accuracyAngle, accuracyAngle);

                //Rotate the spawned arrow randomly based on accuracy
                clone.rotation = transform.rotation;
                clone.Rotate(Vector3.up * randomRot);

                lastAttackTime = Time.time;
            }
        }

        //Rotate towards the position of the touch
        public override void RotateTowardsTouchPos()
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

        #endregion
    }
}