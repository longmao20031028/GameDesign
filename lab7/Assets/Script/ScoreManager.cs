using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class ScoreManager : MonoBehaviour
{
    private int score;
    private int miss;
   
    void Start()
    {
        score = 0;
        miss = 0;
    }

    public int getScore(){ return score; }

    public bool checkGame()
    { 
        return (miss < 15); 
    }
   
    public void Miss() 
    {
        miss += 1;
        Debug.Log("miss"+miss);
    }

    public void Hit(GameObject disk)
    {
        string t = disk.GetComponent<Disk>().nTag;
        if(t!=null)
            score += 1;
        Debug.Log("hit!"+score);
    }

    public void Reset()
    {
        score = 0;
        miss = 0;
    }
}
