using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ClickUpAdventurers
{
    public class Archer : PlayerCharacter
    {
        public GameObject arrowPrefab;
        public Transform firePoint;

        [Header("Special Properties")]
        public float accuracyMaxTime = 3.0f;
        public float minAccuracyAngle = 45.0f;
        public float cooldownTime = 0.5f;

        private float accuracyAngle;
        private float lastAttackTime;

        private void Start()
        {
            lastAttackTime = 0;
        }

        private void Update()
        {
            InheritedUpdateCalls();

            if(holdingTouch && Time.time - lastAttackTime > cooldownTime)
            {
                DrawAccuracy();
            }
        }

        #region Own Methods

        private void DrawAccuracy()
        {
            float accuracyTime = (accuracyMaxTime - (Time.time - begunTouchTime)) / accuracyMaxTime;
            if (accuracyTime < 0)
                accuracyTime = 0;
            accuracyAngle = accuracyTime * minAccuracyAngle;

            Vector3 targetPoint = transform.position + transform.forward * 5;

            float radAngle = accuracyAngle * Mathf.Deg2Rad;


            Vector3 rightPoint = new Vector3(targetPoint.x * Mathf.Cos(radAngle) + 0 + targetPoint.z * Mathf.Sin                                        (radAngle), 
                                0 + targetPoint.y + 0, 
                                targetPoint.x * -Mathf.Sin(radAngle) + 0 + targetPoint.z * Mathf.Cos(radAngle));

            Vector3 leftPoint = new Vector3(targetPoint.x * Mathf.Cos(-radAngle) + 0 + targetPoint.z * Mathf.Sin(-radAngle),
                                0 + targetPoint.y + 0,
                                targetPoint.x * -Mathf.Sin(-radAngle) + 0 + targetPoint.z * Mathf.Cos(-radAngle));

            Debug.DrawLine(transform.position, rightPoint, Color.red);
            Debug.DrawLine(transform.position, leftPoint, Color.red);
        }

        #endregion

        #region Inherited Methods

        public override void Attack()
        {
            if (Time.time - lastAttackTime > cooldownTime)
            {
                Transform clone = Instantiate(arrowPrefab).GetComponent<Transform>();
                clone.position = firePoint.position;

                float randomRot = Random.Range(-accuracyAngle, accuracyAngle);

                clone.rotation = transform.rotation;
                clone.Rotate(Vector3.up * randomRot);

                lastAttackTime = Time.time;
            }
        }

        public override void RotateTowardsTouchPos()
        {
            Vector3 touch = currentTouch.position;
            Vector3 screen = Camera.main.ScreenToWorldPoint(new Vector3(touch.x, touch.y, Camera.main.nearClipPlane + 5.0f));
            Vector3 lookPos = screen - transform.position;
            lookPos.y = 0;
            transform.rotation = Quaternion.LookRotation(lookPos);
        }

        #endregion
    }
}