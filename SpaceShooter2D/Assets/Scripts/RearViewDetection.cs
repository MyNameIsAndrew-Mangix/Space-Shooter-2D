using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RearViewDetection : MonoBehaviour
{
    private SmartEnemy _smartEnemy;
    private GameObject _smartEnemyGameObject;
    private Vector3 offset = new Vector3(0, 1.69f, 0);

    private void Start()
    {
        _smartEnemyGameObject = transform.parent.Find("Smart_Enemy_Object").gameObject;
        _smartEnemy = _smartEnemyGameObject.GetComponent<SmartEnemy>();

        if (_smartEnemy == null)
        {
            Debug.LogError("Smart Enemy is NULL");
        }
        if (_smartEnemyGameObject == null)
        {
            Debug.LogError("Smart Enemy Game Object is NULL");
        }
    }

    private void Update()
    {
        transform.position = _smartEnemyGameObject.transform.position + offset;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
            _smartEnemy.FireBackwards();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
            _smartEnemy.FireForwards();
    }

}