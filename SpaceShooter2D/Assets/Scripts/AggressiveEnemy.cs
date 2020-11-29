using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AggressiveEnemy : Enemy
{
    private bool _isShieldActive = true;
    private GameObject _enemyShieldVisualizer;
    private PlayerDetector _playerDetector;
    private Transform _target;
    private Vector3 _transformUp;

    // Start is called before the first frame update
    void Start()
    {
        _playerDetector = transform.parent.Find("Player_Detector").GetComponent<PlayerDetector>();
        _enemyShieldVisualizer = transform.GetChild(0).gameObject;
        _transformUp = transform.up;

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

        StartCoroutine(base.RandomFireRoutine(_fireRate));
    }

    // Update is called once per frame
    void Update()
    {
        _target = _playerDetector.GetTarget();
        EnemyMoveFire();
    }

    protected override void EnemyMoveFire()
    {
        EnemyMovement();
        if (Time.time > _canFire)
        {
            _canFire = Time.time + _fireRate;
            base.EnemyFire();
        }
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Laser")
        {
            Destroy(other.gameObject);
            if (_isShieldActive)
            {
                _isShieldActive = false;
                _enemyShieldVisualizer.SetActive(false);
            }
            else if (!_isShieldActive)
            {
                EnemyDie();
                if (_player != null)
                {
                    CalculateScore();
                }
                int ammoDropChance = UnityEngine.Random.Range(0, 99);
                if (ammoDropChance >= 89)
                    Instantiate(_ammoDrop, transform.position, Quaternion.identity);
            }
        }

        if (other.tag == "Player")
        {
            if (_isShieldActive)
            {
                _isShieldActive = false;
                _enemyShieldVisualizer.SetActive(false);
                _player.Damage();
            }
            else if (!_isShieldActive)
            {
                EnemyDie();
                if (_player != null)
                {
                    _player.Damage();
                }
            }
        }
    }

    protected override void EnemyMovement()
    {
        if (_target == null && _isAlive)
        {
            transform.up = _transformUp;
            transform.Translate(Vector3.down * _enemySpeed * Time.deltaTime);

            if (transform.position.y <= -5.44f)
            {
                float randomX = UnityEngine.Random.Range(-9.56f, 9.56f);
                transform.position = new Vector3(randomX, 7.56f, 0);
            }
        }

        if (_target != null && _isAlive)
        {
            transform.up = (_target.position - transform.position) * -1.0f;
            transform.position = Vector3.MoveTowards(transform.position, _target.position, (_enemySpeed / 1.2f) * Time.deltaTime);
        }
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
}

