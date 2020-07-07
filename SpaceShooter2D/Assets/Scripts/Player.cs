using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class Player : MonoBehaviour
{
    private bool _isTripleShotActive = false;
    private bool _isFireRateMultiplierActive = false;
    private bool _isShieldActive = false;
    private bool _isBoosting = false;
    private bool _canBoost = true;
    private bool _isMoving = false;
    private bool _isHomingActive = false;
    private float _speed = 5f;
    private float _fireRate = 1f;
    private float _canFire = -1f;
    private float _powerupDuration;
    private float _fireRateMultiplied;
    private float _boostedSpeed = 1.3f;
    private int _ammoCount = 15;
    private int _maxAmmo = 15;
    private int _playerLevel;

    private Color32 _notDamagedShieldColor = new Color32(255, 255, 255, 255);
    private Color32 _moderatleyDamagedShieldColor = new Color32(119, 207, 179, 255);
    private Color32 _severlyDamagedShieldColor = new Color32(55, 103, 88, 255);
    [SerializeField]
    private int _playerLives = 3;
    [SerializeField]
    private int _score;
    [SerializeField]
    private int _randomEngineDamage;
    [SerializeField]
    private int _levelUpThreshold = 10;
    private int _shieldHitPoints = 3;

    public bool IsBoosting { get { return _isBoosting; } set { IsBoosting = value; } }
    public bool IsMoving { get { return _isMoving; } set { IsMoving = value; } }

    [SerializeField]
    private GameObject _tripleShotPrefab;
    [SerializeField]
    private GameObject _laserPrefab;
    [SerializeField]
    private GameObject _playerShieldsVisualizer;
    [SerializeField]
    private GameObject _homingLaserPrefab;
    private SpriteRenderer _playerShieldSpriteRenderer;
    [SerializeField]
    private GameObject[] _damagedEngine;

    private Vector2 _oldPos;

    private Animator _animator;

    private SpawnManager _spawnManager;

    private UIManager _uimanager;

    private AudioSource _laserSound;
    private AudioSource _explosionSound;

    private CameraShake _cameraShake;

    void Start()
    {
        _oldPos = transform.position;
        _cameraShake = GameObject.Find("Main Camera").GetComponent<CameraShake>();
        _animator = GetComponent<Animator>();
        _randomEngineDamage = UnityEngine.Random.Range(0, 2);
        transform.position = new Vector3(0, -2.96f, 0);
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        _uimanager = GameObject.Find("Canvas").GetComponent<UIManager>();
        _playerShieldSpriteRenderer = _playerShieldsVisualizer.GetComponent<SpriteRenderer>();

        _laserSound = GameObject.Find("Audio_Manager/Laser_Sound").GetComponent<AudioSource>();
        _explosionSound = GameObject.Find("Audio_Manager/Explosion_Sound").GetComponent<AudioSource>();

        if (_laserSound == null)
        {
            Debug.LogError("Laser sound is NULL");
        }

        if (_explosionSound == null)
        {
            Debug.LogError("Explosion sound is NULL");
        }

        if (_damagedEngine.Length < 2)
        {
            Debug.LogError("Damaged Engines unassigned!");
        }

        if (_spawnManager == null)
        {
            Debug.LogError("The Spawn Manager is NULL");
        }

        if (_uimanager == null)
        {
            Debug.LogError("The UIManager is NULL");
        }

        if (_animator == null)
        {
            Debug.LogError("The Animator is NULL");
        }
        if (_playerShieldSpriteRenderer == null)
        {
            Debug.LogError("Shield Sprite Renderer is NULL");
        }

        if (_cameraShake == null)
        {
            Debug.LogError("Main Camera is NULL");
        }
    }

    void Update()
    {
        PlayerMovement();
        IsPlayerMoving();
        if (Input.GetKey(KeyCode.Space) && Time.time > _canFire && _ammoCount > 0)
        {
            _ammoCount--;
            _canFire = Time.time + _fireRate;
            LaserFire();
        }
        if (_score > _levelUpThreshold)
            LevelUp();

        if (Input.GetKeyDown(KeyCode.LeftShift) && _canBoost)
                ThrusterBoost();

        if (Input.GetKeyUp(KeyCode.LeftShift))
            if (_isBoosting)
                NormalSpeed();

        if (Input.GetKey(KeyCode.LeftShift) && !_canBoost && _isBoosting)
            NormalSpeed();
    }

    void IsPlayerMoving()
    {
        if (_oldPos.x < transform.position.x || _oldPos.y < transform.position.y || _oldPos.x > transform.position.x || _oldPos.y > transform.position.y)
        {
            _isMoving = true;
            _oldPos = transform.position;
        }
        else
        {
            _isMoving = false;
        }
    }

    void ThrusterBoost()
    {
        _isBoosting = true;
        _speed *= _boostedSpeed;
    }

    void NormalSpeed()
    {
        _isBoosting = false;
        _speed /= _boostedSpeed;
    }

    void PlayerMovement()
    {
        float horizInput = Input.GetAxisRaw("Horizontal");
        float vertiInput = Input.GetAxisRaw("Vertical");
        _animator.SetFloat("PlayerLeftOrRight", horizInput);

        transform.Translate(new Vector3(horizInput, vertiInput, 0) * _speed * Time.deltaTime);

        if (transform.position.x >= 10.3)
        {
            transform.position = new Vector3(-10.2f, transform.position.y, 0);
        }
        else if (transform.position.x <= -10.3f)
        {
            transform.position = new Vector3(10.2f, transform.position.y, 0);
        }
        //may adjust clamp maximum value to 6 on the Y for more player maneuverability
        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, -4f, 2), 0);
    }

    void LaserFire()
    {
        Vector3 laserOffset = new Vector3(0, 1.013f, 0);
        if (_isTripleShotActive)
        {
            Instantiate(_tripleShotPrefab, transform.position, Quaternion.identity);
        }
        else if (_isHomingActive)
        {
            Instantiate(_homingLaserPrefab, transform.position + laserOffset, Quaternion.identity);
        }
        else
        {
            Instantiate(_laserPrefab, transform.position + laserOffset, Quaternion.identity);
        }

        if (_isFireRateMultiplierActive == true)
        {
            _fireRateMultiplied = _fireRate * 0.5f;
            _canFire = Time.time + _fireRateMultiplied;
        }
        _laserSound.Play();
        _uimanager.UpdateAmmo(_ammoCount);
    }

    void LevelUp()
    {

        _playerLevel++;
        if (_speed < 10)
        {
            if (_isBoosting)
                _speed += 0.5f * _boostedSpeed;
            else
                _speed += 0.5f;
        }
        if (_fireRate < 0.75f)
        {
            _fireRate -= 0.05f;
        }
        _levelUpThreshold *= 2;
        Heal();
    }

    public void Heal()
    {
        if (_playerLives < 3)
        {
            _playerLives++;

            switch (_playerLives)
            {
                case 3:
                    _damagedEngine[_randomEngineDamage].SetActive(false);
                    break;
                case 2:
                    if (_randomEngineDamage == 1)
                    {
                        _damagedEngine[0].SetActive(false);
                    }
                    else if (_randomEngineDamage == 0)
                    {
                        _damagedEngine[1].SetActive(false);
                    }
                    break;
                case 1:
                    break;
            }
            _uimanager.UpdateLives(_playerLives);
        }
    }

    public void AmmoRefill()
    {
        _ammoCount += 15;
        if (_ammoCount > _maxAmmo)
                _ammoCount = _maxAmmo;

        _uimanager.UpdateAmmo(_ammoCount);
    }

    public void Damage()
    {
        if (_isShieldActive == true)
        {
            _shieldHitPoints--;
            switch (_shieldHitPoints)
            {
                case 3:
                    _playerShieldSpriteRenderer.color = _notDamagedShieldColor;
                    break;
                case 2: _playerShieldSpriteRenderer.color = _moderatleyDamagedShieldColor;
                    break;
                case 1:
                    _playerShieldSpriteRenderer.color = _severlyDamagedShieldColor;
                    break;
                default:
                    _playerShieldSpriteRenderer.color = _notDamagedShieldColor;
                    break;
            }

            if (_shieldHitPoints < 1)
            {
                _isShieldActive = false;
                _playerShieldsVisualizer.SetActive(false);
            }
            return;
        }

        _playerLives--;
        _cameraShake.TriggerShake();
        switch (_playerLives)
        {
            case 3:
                break;
            case 2:
                _damagedEngine[_randomEngineDamage].SetActive(true);
                break;
            case 1: 
                if (_randomEngineDamage == 1)
                {
                    _damagedEngine[0].SetActive(true);
                }
                else if (_randomEngineDamage == 0)
                {
                    _damagedEngine[1].SetActive(true);
                }
                break;
            case 0:
                _spawnManager.onPlayerDeath();
                _explosionSound.Play();
                Destroy(this.gameObject);
                break;
            default:
                break;
        }
        _uimanager.UpdateLives(_playerLives);
    }

    public void TripleShotActive()
    {
        _isTripleShotActive = true;
        StartCoroutine(TripleShotPowerDown());
    }

    public void HomingShotActive()
    {
        _isHomingActive = true;
        StartCoroutine(HomingShotPowerDown());
    }

    public int PlayerLevelCheck()
    {
        return _playerLevel;
    }
 
    public void SpeedActive()
    {
        _isFireRateMultiplierActive = true;
        StartCoroutine(FireRateBoostPowerDown());
    }

    public void CannotBoost()
    {
        _canBoost = false;
    }
    public void CanBoost()
    {
        _canBoost = true;
    }
   
    public void ShieldActive()
    {
        _isShieldActive = true;
        _playerShieldsVisualizer.SetActive(true);
        _shieldHitPoints = 3;
        _playerShieldSpriteRenderer.color = _notDamagedShieldColor;
    }

    public void AddScore(int points)
    {
        _score += points;
        _uimanager.ScoreUpdate(_score);
    }

    IEnumerator TripleShotPowerDown()
    {
        while (_isTripleShotActive)
        {
            _powerupDuration = 5f;
            yield return new WaitForSeconds(_powerupDuration);
            _isTripleShotActive = false;
        }
    }

    IEnumerator HomingShotPowerDown()
    {
        while (_isHomingActive)
        {
            yield return new WaitForSeconds(5);
            _isHomingActive = false;
        }
    }

    IEnumerator FireRateBoostPowerDown()
    {
        while (_isFireRateMultiplierActive)
        {
            _powerupDuration = 3f;
            yield return new WaitForSeconds(_powerupDuration);
            _isFireRateMultiplierActive = false;
        }
    }
}
