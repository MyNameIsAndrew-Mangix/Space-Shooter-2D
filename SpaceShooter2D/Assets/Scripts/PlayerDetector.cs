using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDetector : MonoBehaviour
{
    private AggressiveEnemy _aggressiveEnemy;
    private GameObject _aggressiveEnemyObject;
    private Vector3 offset = new Vector3(0, -0.18f, 0);
    private Transform _target;

    void Start()
    {
        _aggressiveEnemyObject = transform.parent.Find("Aggressive_Enemy_Object").gameObject;
        _aggressiveEnemy = _aggressiveEnemyObject.GetComponent<AggressiveEnemy>();

        if (_aggressiveEnemy == null)
        {
            Debug.LogError("Aggressive Enemy is NULL");
        }
        if (_aggressiveEnemyObject == null)
        {
            Debug.LogError("Aggressive Enemy Game Object is NULL");
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = _aggressiveEnemyObject.transform.position + offset;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            _target = other.gameObject.transform;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            _target = null;
        }
    }

    public Transform GetTarget()
    {
        return _target;
    }

}
