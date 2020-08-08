using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loot : MonoBehaviour
{
    public MeshRenderer renderer;
    public float destroyTime = 25.0f;

    private float startDestroyTime;
    private void Start()
    {
        startDestroyTime = Time.time;
        Destroy(gameObject, destroyTime);
    }

    private void Update()
    {
        float currentTimeDiff = Time.time - startDestroyTime - destroyTime;
        if (currentTimeDiff >= -5)
        {
            if ((currentTimeDiff >= -5 && currentTimeDiff <= -4) || (currentTimeDiff >= -3 && currentTimeDiff <= -2) || (currentTimeDiff >= -1 && currentTimeDiff <= 0))
            {
                renderer.enabled = true;
            }
            else
                renderer.enabled = false;
        }
    }
}