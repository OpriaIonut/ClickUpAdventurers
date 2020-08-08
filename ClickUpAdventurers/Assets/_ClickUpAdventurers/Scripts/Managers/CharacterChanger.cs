using ClickUpAdventurers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterChanger : MonoBehaviour
{
    #region Singleton

    public static CharacterChanger instance;

    private void Awake()
    {
        if (instance != null)
            Destroy(this);
        else
            instance = this;
    }

    #endregion

    public List<PlayerCharacter> characters;

    [HideInInspector] public Vector3 availablePosition;
    private Vector3[] positions;
    private int[] indices;

    private int selectedIndex;

    private void Start()
    {
        selectedIndex = 0;
        characters[selectedIndex].isSelected = true;

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
        int retainer = indices[0];
        for(int index = 0; index < indices.Length; index++)
        {
            int nextIndex = index + 1;
            if (nextIndex >= indices.Length)
                nextIndex = 0;
            indices[index] = indices[nextIndex];
        }
        indices[indices.Length - 1] = retainer;

        for(int index = 0; index < characters.Count; index++)
        {
            characters[index].isSelected = false; 
            if (indices[index] == 0)
                characters[index].isSelected = true;
            characters[index].ChangeBasePosition(positions[indices[index]]);        }
    }
}
