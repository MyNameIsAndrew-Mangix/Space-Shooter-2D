using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrafingEnemy : Enemy
{
    [SerializeField]
    private float _cachedXPos;
    [SerializeField]
    private bool _hasHitCachedXPos = false;
    [SerializeField]
    private Vector3 _downAndLOrR;

    void Start()
    {
        _cachedXPos = transform.position.x + 2;
    }

    // Update is called once per frame
    void Update()
    {
        EnemyMovement();
        if (Time.time > _canFire)
        {
            _canFire = Time.time + _fireRate;
            base.EnemyFire();
        }
    }

    protected override void EnemyMovement()
    {
        _downAndLOrR.y = -0.75f;
        _downAndLOrR.x = CalcStrafeDir(CheckToMoveLeftOrRight(_cachedXPos)); //CheckToMoveLeftOrRight(_cachedXPos);
        transform.Translate(_downAndLOrR * _enemySpeed * Time.deltaTime);

        if (transform.position.y <= -5.44f)
        {
            float randomX = UnityEngine.Random.Range(-9.56f, 9.56f);
            _cachedXPos = randomX;
            transform.position = new Vector3(randomX, 7.56f, 0);
        }
        if (transform.position.x >= 10.4)
        {
            transform.position = new Vector3(-10.2f, transform.position.y, 0);
            _cachedXPos = transform.position.x + 3;
        }
        else if (transform.position.x <= -10.4f)
        {
            transform.position = new Vector3(10.2f, transform.position.y, 0);
            _cachedXPos = transform.position.x - 3;
        }
    }
    private bool CheckToMoveLeftOrRight(float cachedXPos)
    {
        // return true for right
        // return false for left
        if (cachedXPos >= 0 && cachedXPos <= 9.56f)
            return false;
        else if (cachedXPos <= 0 && cachedXPos >= -9.56f)
            return true;
        else
        {
            Debug.LogError("CachedXPos out of range");
            return false;
        }
    }
    private int CalcStrafeDir(bool LeftOrRight)
    {
        if (LeftOrRight && !_hasHitCachedXPos)
        {
            HitCachedPos();
            return 1;
        }
        else if (LeftOrRight && _hasHitCachedXPos)
        {
            HitCachedPos();
            return -1;
        }
        if (!LeftOrRight && !_hasHitCachedXPos)
        {
            HitCachedPos();
            return -1;
        }
        else if (!LeftOrRight && _hasHitCachedXPos)
        {
            HitCachedPos();
            return 1;
        }
        else
        {
            HitCachedPos();
            Debug.LogError("CheckToMoveLeftOrRight has returned null");
            return 0;
        }
    }
    private void HitCachedPos()
    {
        float xPos = transform.position.x;

        if (_hasHitCachedXPos && CheckToMoveLeftOrRight(_cachedXPos))
        {
            if (xPos <= (_cachedXPos - 3))
            {
                _hasHitCachedXPos = false;
            }
        }
        else if (_hasHitCachedXPos && !CheckToMoveLeftOrRight(_cachedXPos))
        {
            if (xPos >= (_cachedXPos + 3))
            {
                _hasHitCachedXPos = false;
            }
        }

        if (!_hasHitCachedXPos && CheckToMoveLeftOrRight(_cachedXPos))
        {
            if (xPos >= _cachedXPos)
            {
                _hasHitCachedXPos = true;
            }
        }
        else if (!_hasHitCachedXPos && !CheckToMoveLeftOrRight(_cachedXPos))
        {
            if (xPos <= _cachedXPos)
            {
                _hasHitCachedXPos = true;
            }
        }
    }
}
