﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExpressoBits.PoolSimply;

[RequireComponent(typeof(Rigidbody2D))]
public class Ball : MonoBehaviour,IPooler {
    
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    private void Start() {
        spriteRenderer.color = GetRandomColor();
        rb.velocity = new Vector3(0f, 0f, 0f);
        rb.angularVelocity = 0f;
    }

    public void OnPoolerDisable()
    {
        //Example use interface IPooler
    }

    public void OnPoolerEnable()
    {
        //Example use interface IPooler
    }

    void Awake () {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public Color GetRandomColor(){
        return new Color(Random.Range(0f,1f),Random.Range(0f,1f),Random.Range(0f,1f));
    }

}