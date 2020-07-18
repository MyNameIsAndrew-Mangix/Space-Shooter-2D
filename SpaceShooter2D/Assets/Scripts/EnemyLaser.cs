using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class EnemyLaser : MonoBehaviour
{
    [SerializeField]
    private float _laserSpeed = 8;
    [SerializeField]
    private bool _isSmartEnemyLaser = false;
    private Player _player;

    private void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();

        if (_player == null)
        {
            Debug.LogError("The Player is NULL");
        }
    }

    void Update()
    {
        if (_isSmartEnemyLaser)
        {
            transform.Translate(Vector3.up * _laserSpeed * Time.deltaTime);
        }
        else
        {
            transform.Translate(Vector3.down * _laserSpeed * Time.deltaTime);
        }

        if (transform.position.y <= -7.5f)
        {
            if (transform.parent != null)
            {
                Destroy(transform.parent.gameObject);
            }
            Destroy(this.gameObject);
        }

        if (transform.position.y >= 7.5f)
        {
            if (transform.parent != null)
            {
                Destroy(transform.parent.gameObject);
            }
            Destroy(this.gameObject);
        }

    }

    public void IsSmartEnemyLaser()
    {
        _isSmartEnemyLaser = true;
    }

    public void IsNotSmartEnemyLaser()
    {
        _isSmartEnemyLaser = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            _player = other.GetComponent<Player>();
            //if player is not null (if player exists), damage player.
            if (_player != null)
            {
                _player.Damage();
            }
            Destroy(this.gameObject);
        }
    }
}
