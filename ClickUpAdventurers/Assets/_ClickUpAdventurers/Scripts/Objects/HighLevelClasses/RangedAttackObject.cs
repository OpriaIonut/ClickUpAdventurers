using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClickUpAdventurers
{
    public class RangedAttackObject : MonoBehaviour
    {
        public float deathTime = 5.0f;
        public float movementSpeed = 10.0f;
        private Rigidbody rb;

        private void Start()
        {
            rb = GetComponent<Rigidbody>();

            Destroy(gameObject, deathTime);
        }

        private void FixedUpdate()
        {
            rb.velocity = transform.forward * Time.fixedDeltaTime * movementSpeed;
        }
    }
}