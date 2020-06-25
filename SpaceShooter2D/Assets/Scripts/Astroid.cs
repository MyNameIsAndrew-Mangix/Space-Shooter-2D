using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Astroid : MonoBehaviour
{
    [SerializeField]
    private float _rotateSpeed = 3;
    [SerializeField]
    GameObject _explosionPrefab;
    private SpawnManager _spawnManager;
    [SerializeField]
    private AudioSource _explosionSound;

    private void Start()
    {
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
    }

    void Update()
    {
        transform.Rotate(new Vector3(0, 0, -1) * _rotateSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Laser")
        {
            float posX = transform.position.x;
            float posY = transform.position.y;
            float posZ = transform.position.z;
            GameObject explosionClone = Instantiate<GameObject>(_explosionPrefab, new Vector3(posX, posY, posZ), Quaternion.identity);
            Destroy(explosionClone, 2.57f);
            Destroy(other.gameObject);
            _spawnManager.StartSpawning();
            Destroy(this.gameObject, 0.25f);
            _explosionSound.Play();
        }
    }

}
