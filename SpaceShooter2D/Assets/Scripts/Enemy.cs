using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Player _player;
    [SerializeField]
    private GameObject _enemyLaserGameObject;
    private EnemyLaser _enemyLaser;
    [SerializeField]
    private GameObject _ammoDrop;
    private Animator _animator;
    private float _enemySpeed = 5f;
    private BoxCollider2D _boxCollider2D;
    private AudioSource _laserFireSound;
    private AudioSource _explosionSound;
    private float _canFire = 2f;
    private float _fireRate;
    private bool _isAlive = true;
    private int _coinFlip;
    [SerializeField]
    private float _cachedXPos;
    [SerializeField]
    private Vector3 _downAndLOrR;
    [SerializeField]
    private bool _hasHitCachedXPos = false;
    // Start is called before the first frame update
    void Start()
    {
        _cachedXPos = transform.position.x + 2;
        _coinFlip = UnityEngine.Random.Range(1, 3);
        _fireRate = UnityEngine.Random.Range(3.5f, 7f);
        _boxCollider2D = GetComponent<BoxCollider2D>();
        _player = GameObject.Find("Player").GetComponent<Player>();
        _animator = GetComponent<Animator>();
        _explosionSound = GameObject.Find("Audio_Manager/Explosion_Sound").GetComponent<AudioSource>();
        _laserFireSound = GameObject.Find("Audio_Manager/Laser_Sound").GetComponent<AudioSource>();
        _enemyLaser = _enemyLaserGameObject.GetComponent<EnemyLaser>();

        if (_boxCollider2D == null)
        {
            Debug.LogError("Enemy Box Collider is NULL");
        }

        if (_player == null)
        {
            Debug.LogError("The Player is NULL");
        }

        if (_animator == null)
        {
            Debug.LogError("The Animator is NULL");
        }

        if (_enemyLaser == null)
        {
            Debug.LogError("The Enemy Laser is NULL");
        }
        StartCoroutine(RandomFireRoutine(_fireRate));
        _enemyLaser.IsNotSmartEnemyLaser();

    }

    // Update is called once per frame
    void Update()
    {
        EnemyMovement();
        if (Time.time > _canFire)
        {
            _canFire = Time.time + _fireRate;
            EnemyFire();
        }
    }

    private void EnemyFire()
    {
        if (_isAlive == true)
        {
            Vector3 laserOffset = new Vector3(0.002f, -0.662f, 0);
            _laserFireSound.Play();
            Instantiate(_enemyLaserGameObject, transform.position + laserOffset, Quaternion.identity);
        }
    }

    private void EnemyMovement()
    {
        switch (_coinFlip)
        {
            case 1:
                transform.Translate(Vector3.down * _enemySpeed * Time.deltaTime);

                if (transform.position.y <= -5.44f)
                {
                    float randomX = UnityEngine.Random.Range(-9.56f, 9.56f);
                    transform.position = new Vector3(randomX, 7.56f, 0);
                }
                break;
            case 2:
                _downAndLOrR.y = -1;
                _downAndLOrR.x = MoveLeftOrRight(CheckToMoveLeftOrRight(_cachedXPos)); //CheckToMoveLeftOrRight(_cachedXPos);
                transform.Translate(_downAndLOrR * _enemySpeed * Time.deltaTime);

                if (transform.position.y <= -5.44f)
                {
                    float randomX = UnityEngine.Random.Range(-9.56f, 9.56f);
                    _cachedXPos = randomX;
                    transform.position = new Vector3(randomX, 7.56f, 0);
                }
                break;
        }
        if (transform.position.x >= 10.4)
        {
            transform.position = new Vector3(-10.2f, transform.position.y, 0);
            _cachedXPos = transform.position.x + 2;
        }
        else if (transform.position.x <= -10.4f)
        {
            transform.position = new Vector3(10.2f, transform.position.y, 0);
            _cachedXPos = transform.position.x - 2;
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



    private int MoveLeftOrRight(bool LeftOrRight)
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
            if (xPos <= (_cachedXPos - 2))
            {
                _hasHitCachedXPos = false;
            }
        }
        else if (_hasHitCachedXPos && !CheckToMoveLeftOrRight(_cachedXPos))
        {
            if (xPos >= (_cachedXPos + 2))
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Laser")
        {
            Destroy(other.gameObject);
            EnemyDie();
            if (_player != null) 
            {
                CalculateScore();
            }
            int ammoDropChance = UnityEngine.Random.Range(0, 99);
            if (ammoDropChance >= 89)
                Instantiate<GameObject>(_ammoDrop, transform.position, Quaternion.identity);
        }


        if (other.tag == "Player")
        {
            //if player is not null (if player exists), damage player.
            if (_player != null)
            {
                _player.Damage();
                EnemyDie();
            }
        }
    }

    private void EnemyDie()
    {
        _isAlive = false;
        _boxCollider2D.enabled = false;
        _animator.SetTrigger("OnEnemyDeath");
        _enemySpeed = 0;
        _explosionSound.Play();
        Destroy(this.gameObject, 2.35f);
    }
    private void CalculateScore()
    {
        int playerLevel = _player.PlayerLevelCheck();
        int scoreCalculationInt;
        float distanceBetweenPlayerAndEnemy = Vector3.Distance(transform.position, _player.transform.position);

        if (distanceBetweenPlayerAndEnemy <= 3.5f)
        {
            scoreCalculationInt = 15;
            _player.AddScore(scoreCalculationInt);
        }
        else if (distanceBetweenPlayerAndEnemy >= 3.6f && distanceBetweenPlayerAndEnemy <= 5f)
        {
            scoreCalculationInt = 10;
            _player.AddScore(scoreCalculationInt);
        }
        else
        {
            scoreCalculationInt = 5;
            _player.AddScore(scoreCalculationInt);
        }

        if(playerLevel >= 3)
        {
            if (distanceBetweenPlayerAndEnemy <= 3.5f)
            {
                scoreCalculationInt = 30;
                _player.AddScore(scoreCalculationInt);
            }
            else if (distanceBetweenPlayerAndEnemy >= 3.6f && distanceBetweenPlayerAndEnemy <= 5f)
            {
                scoreCalculationInt = 20;
                _player.AddScore(scoreCalculationInt);
            }
            else
            {
                scoreCalculationInt = 10;
                _player.AddScore(scoreCalculationInt);
            }
        }

    }

    private IEnumerator RandomFireRoutine(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        while (this.gameObject.activeInHierarchy)
        {
            _fireRate = UnityEngine.Random.Range(3f, 7f);
            yield return new WaitForSeconds(_fireRate);
        }
    }

}
