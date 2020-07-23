using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AggressiveEnemy : MonoBehaviour
{
    private Player _player;
    [SerializeField]
    private GameObject _enemyLaserGameObject;
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
    private bool _isShieldActive = true;
    private EnemyLaser _enemyLaser;
    [SerializeField]
    private GameObject _enemyShieldVisualizer;

    // Start is called before the first frame update
    void Start()
    {
        _enemyLaser = _enemyLaserGameObject.GetComponent<EnemyLaser>();
        _fireRate = UnityEngine.Random.Range(3.5f, 7f);
        _boxCollider2D = GetComponent<BoxCollider2D>();
        _player = GameObject.Find("Player").GetComponent<Player>();
        _animator = GetComponent<Animator>();
        _explosionSound = GameObject.Find("Audio_Manager/Explosion_Sound").GetComponent<AudioSource>();
        _laserFireSound = GameObject.Find("Audio_Manager/Laser_Sound").GetComponent<AudioSource>();

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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Laser")
        {
            _isAlive = false;
            Destroy(other.gameObject);
            if (_isShieldActive)
            {
                _isShieldActive = false;
                _enemyShieldVisualizer.SetActive(false);
            }
            else if (!_isShieldActive)
            {
                if (_player != null)
                {
                    CalculateScore();
                }
                int ammoDropChance = UnityEngine.Random.Range(0, 99);
                if (ammoDropChance >= 89)
                    Instantiate(_ammoDrop, transform.position, Quaternion.identity);
                EnemyDie();
            }
        }

        if(other.tag == "Player")
        {
            if (_isShieldActive)
            {
                _isShieldActive = false;
                _enemyShieldVisualizer.SetActive(false);
            }
            else if (!_isShieldActive)
            {
                _isAlive = false;
                if (_player != null)
                {
                    _player.Damage();
                }
                EnemyDie();
            }
        }
    }

    private void EnemyFire()
    {
        if (_isAlive == true)
        {
            Vector3 laserOffset = new Vector3(0.002f, -0.662f, 0);
            _laserFireSound.Play();
            Instantiate<GameObject>(_enemyLaserGameObject, transform.position + laserOffset, Quaternion.identity);
        }
    }

    private void EnemyMovement()
    {
        transform.Translate(Vector3.down * _enemySpeed * Time.deltaTime);

        if (transform.position.y <= -5.44f)
        {
            float randomX = UnityEngine.Random.Range(-9.56f, 9.56f);
            transform.position = new Vector3(randomX, 7.56f, 0);
        }

    }

    private void EnemyDie()
    {
        _boxCollider2D.enabled = false;
        _animator.SetTrigger("OnEnemyDeath");
        _enemySpeed = 0;
        _explosionSound.Play();
        Destroy(transform.parent.gameObject, 2.35f);
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

