using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public abstract class Enemy : MonoBehaviour
{
    //VARIABLES
    protected Player _player { get; private set; }
    [SerializeField]
    protected GameObject _enemyLaserGameObject;
    protected EnemyLaser _enemyLaser { get; private set; }
    [SerializeField]
    protected GameObject _ammoDrop;
    [SerializeField]
    protected Animator _animator { get; private set; }
    protected float _enemySpeed = 5f;
    protected BoxCollider2D _boxCollider2D { get; private set; }
    protected AudioSource _laserFireSound { get; private set; }
    protected AudioSource _explosionSound { get; private set; }
    protected float _canFire = 2f;
    protected float _fireRate { get; private set; }
    protected bool _isAlive = true;

    //METHODS ---------------------------------------------------------------
    private void Awake()
    {
        _fireRate = UnityEngine.Random.Range(3.5f, 7f);
        _boxCollider2D = GetComponent<BoxCollider2D>();
        _player = GameObject.Find("Player").GetComponent<Player>();
        _animator = GetComponent<Animator>();
        _explosionSound = GameObject.Find("Audio_Manager/Explosion_Sound").GetComponent<AudioSource>();
        _laserFireSound = GameObject.Find("Audio_Manager/Laser_Sound").GetComponent<AudioSource>();
        _enemyLaser = _enemyLaserGameObject.GetComponent<EnemyLaser>();
    }
    void Start()
    {
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

        if (_enemyLaserGameObject == null)
            Debug.LogError("The Enemy Laser Game Object is NULL");

        if (_enemyLaser == null)
        {
            Debug.LogError("The Enemy Laser is NULL");
        }
        _fireRate = UnityEngine.Random.Range(3.5f, 7f);
        _enemyLaser.IsNotSmartEnemyLaser();
    }

    protected virtual void EnemyMoveFire()
    {
        EnemyMovement();
        if (Time.time > _canFire)
        {
            _canFire = Time.time + _fireRate;
            EnemyFire();
        }
    }

    protected virtual void EnemyFire()
    {
        if (_isAlive == true)
        {
            Vector3 laserOffset = new Vector3(0.002f, -0.662f, 0);
            _laserFireSound.Play();
            Instantiate(_enemyLaserGameObject, transform.position + laserOffset, Quaternion.identity);
        }
    }

    protected virtual void EnemyMovement()
    {
        transform.Translate(Vector3.down * _enemySpeed * Time.deltaTime);

        if (transform.position.y <= -5.44f)
        {
            float randomX = UnityEngine.Random.Range(-9.56f, 9.56f);
            transform.position = new Vector3(randomX, 7.56f, 0);
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
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

    protected virtual void EnemyDie()
    {
        _isAlive = false;
        _boxCollider2D.enabled = false;
        _animator.SetTrigger("OnEnemyDeath");
        _enemySpeed = 0;
        _explosionSound.Play();
        Destroy(this.gameObject, 2.35f);
    }

    protected virtual void CalculateScore()
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

        if (playerLevel >= 3)
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

    protected virtual IEnumerator RandomFireRoutine(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        while (_isAlive)
        {
            _fireRate = UnityEngine.Random.Range(3f, 7f);
            yield return new WaitForSeconds(_fireRate);
        }
    }

}
