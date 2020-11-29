using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VanillaEnemy : Enemy
{

    private void Start()
    {
        StartCoroutine(base.RandomFireRoutine(_fireRate));
    }

    void Update()
    {
        base.EnemyMoveFire();
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);
    }
}
