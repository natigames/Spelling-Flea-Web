﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class flea : MonoBehaviour
{
    public static flea instance; //singleton to manage pushback


    private Animator anim;
    private SpriteRenderer theSR;

    private void Awake()
    {
        instance = this;

    }

    // Start is called before the first frame update
    void Start()
    {

        anim = GetComponent<Animator>();
        theSR = GetComponent<SpriteRenderer>();
    }

    public void doAnim(string status)
    {
        anim.SetTrigger(status); // set Animator variable to animate jump!
    }

}
