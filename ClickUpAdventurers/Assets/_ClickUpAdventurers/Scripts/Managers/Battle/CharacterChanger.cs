using ClickUpAdventurers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClickUpAdventurers
{
    public class CharacterChanger : MonoBehaviour
    {
        #region Singleton

        public static CharacterChanger instance;

        private void Awake()
        {
            if (instance != null)
                Destroy(this.gameObject);
            else
                instance = this;
        }

        #endregion

        public List<PlayerCharacter> characters;    //The list of the characters that we can swap between
        private Quaternion initRot; //Used to reset rotation on swap

        private Vector3[] positions;    //The position where we can place the characters
        private int[] indices;          //Indices used to permute the positioning of the characters

        public bool canChange = true;   //Set by other scripts to block the character changing on certain events (like shooting something)

        private void Start()
        {
            characters[0].isSelected = true;
            initRot = characters[0].transform.rotation;

            positions = new Vector3[characters.Count];
            indices = new int[characters.Count];
            for (int index = 0; index < characters.Count; index++)
            {
                positions[index] = characters[index].transform.position;
                indices[index] = index;
            }
        }

        public void ChangeSelectedCharacter()
        {
            if (!canChange)
                return;

            //Permute the indices vector in order to rotate the characters
            int retainer = indices[0];
            for (int index = 0; index < indices.Length; index++)
            {
                int nextIndex = index + 1;
                if (nextIndex >= indices.Length)
                    nextIndex = 0;
                indices[index] = indices[nextIndex];
            }
            indices[indices.Length - 1] = retainer;

            //Set their position and rotation
            for (int index = 0; index < characters.Count; index++)
            {
                characters[index].isSelected = false;
                if (indices[index] == 0)
                    characters[index].isSelected = true;
                //Setting the position and rotation is processed by the characters because in certain moments some of them may not want to change position (ex: looter is away from the party)
                characters[index].ChangeBasePosition(positions[indices[index]], initRot);
            }
        }
    }
}