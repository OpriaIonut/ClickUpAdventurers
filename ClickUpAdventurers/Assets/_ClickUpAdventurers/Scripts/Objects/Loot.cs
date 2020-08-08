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
    }

    private bool resetValuesAfterPause = false;
    private void Update()
    {
        if (!BattleManager.instance.GamePaused)
        {
            //If the game was paused, add the paused time to the destroy time
            if(resetValuesAfterPause)
            {
                startDestroyTime += BattleManager.instance.PausedTimeDiff;
                resetValuesAfterPause = false;
            }

            //We render an animation in the last 5 seconds of the loot lifetime
            float currentTimeDiff = Time.time - startDestroyTime - destroyTime;
            if (currentTimeDiff >= -5)
            {
                //This condition represents the seconds between which to enable/disable the renderer to simulate the animation
                if ((currentTimeDiff >= -5 && currentTimeDiff <= -4) || (currentTimeDiff >= -3 && currentTimeDiff <= -2) || (currentTimeDiff >= -1 && currentTimeDiff <= 0))
                {
                    renderer.enabled = true;
                }
                else
                    renderer.enabled = false;
            }
            if (currentTimeDiff >= 0)
            {
                Destroy(gameObject);
            }
        }
        else
        {
            resetValuesAfterPause = true;
        }
    }
}