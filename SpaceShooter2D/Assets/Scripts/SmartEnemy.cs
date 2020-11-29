using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmartEnemy : Enemy
{
    private RearViewDetection _backwardsEyes;
    void Start()
    {                  //transform.parent.transform.Find("Backwards_Eyes").GetComponent<RearViewDetection>();
        _backwardsEyes = transform.parent.transform.GetChild(1).GetComponent<RearViewDetection>();
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

    protected override void EnemyDie()
    {
        _isAlive = false;
        _boxCollider2D.enabled = false;
        _animator.SetTrigger("OnEnemyDeath");
        _enemySpeed = 0;
        _explosionSound.Play();
        Destroy(this.gameObject, 2.35f);
        Destroy(transform.parent.gameObject, 2.35f);
    }

    public void FireBackwards()
    {
        _enemyLaser.IsSmartEnemyLaser();
    }

    public void FireForwards()
    {
        _enemyLaser.IsNotSmartEnemyLaser();
    }

    protected override void CalculateScore()
    {
        base.CalculateScore();
    }
}
