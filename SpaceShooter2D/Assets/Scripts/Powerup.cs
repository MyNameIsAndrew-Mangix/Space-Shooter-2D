using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{
    [SerializeField]
    private float _speed = 3.0f;
    private float _magnetSpeed = 8.0f;
    [SerializeField] // 0 = tripleshot, 1 = attack speed, 2 = shields
    private int _powerupID;
    private AudioSource _powerUpPickupSound;
    private bool _isMovingToPlayer = false;


    void Start()
    {
        _powerUpPickupSound = GameObject.Find("Audio_Manager/PowerUp_Sound").GetComponent<AudioSource>();

        if (_powerUpPickupSound == null)
        {
            Debug.LogError("PowerUp sound is NULL");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!_isMovingToPlayer)
            transform.Translate(Vector3.down * _speed * Time.deltaTime);

        if (transform.position.y <= -6.0f)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.tag == "Player")
        {
            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                switch (_powerupID)
                {
                    case 0:
                        player.TripleShotActive();
                        break;
                    case 1:
                        player.SpeedActive();
                        break;
                    case 2:
                        player.ShieldActive();
                        break;
                    case 3:
                        player.Heal();
                        break;
                    case 4:
                        player.AmmoRefill();
                        break;
                    case 5:
                        player.HomingShotActive();
                        break;
                }
                _powerUpPickupSound.Play();
            }
            else Debug.Log("Player component not found");

            Destroy(this.gameObject);
        }
    }

    public void MoveToPlayer(Vector3 targetPos)
    {
        _isMovingToPlayer = true;
        transform.position = Vector3.MoveTowards(transform.position, targetPos, _magnetSpeed * Time.deltaTime);
    }
}
