using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PermenentUI : MonoBehaviour
{
    //Player Stats
    public int cherries = 0;
    public int health = 5;
    public Text cherrytext;
 public Text healthamount;

    public
        static PermenentUI perm;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);

        //Singelton
        if (!perm)
        {
            perm = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Reset()
    {
        health = 5;
        healthamount.text = healthamount.ToString();
        cherries = 0;
        cherrytext.text = cherrytext.ToString();
    }



}
