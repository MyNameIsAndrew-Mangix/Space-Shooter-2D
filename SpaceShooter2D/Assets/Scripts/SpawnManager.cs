using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{

    [SerializeField] 
    private GameObject _enemyPrefab;
    [SerializeField]
    private GameObject _enemyContainer;
    [SerializeField]
    private GameObject[] _powerups;
    private int _previousPowerUp;
    private float _powerUpCooldown;
    private bool _stopSpawning = false;

    // Start is called before the first frame update
    void Start()
    {   
        _powerUpCooldown = Random.Range(7.5f, 15.9f);
    }

    public void StartSpawning()
    {
        //only pass in values this way if planned to be dynamic based on conditions elsewhere or to be tweaked in the editor
        StartCoroutine(SpawnEnemyRoutine(3.5f));
        StartCoroutine(SpawnPowerUpRoutine(_powerUpCooldown));
    }

    IEnumerator SpawnEnemyRoutine(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        while (_stopSpawning == false)
        {
            Vector3 enemySpawnPos = new Vector3(Random.Range(-9.56f, 9.56f), 8, 0);
            GameObject newEnemy = Instantiate(_enemyPrefab, enemySpawnPos, Quaternion.identity);
            newEnemy.transform.parent = _enemyContainer.transform;
            yield return new WaitForSeconds(waitTime);
        }
    }

    IEnumerator SpawnPowerUpRoutine(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
            while (_stopSpawning == false)
            {
                Vector3 powerUpSpawnPos = new Vector3(Random.Range(-9.56f, 9.56f), 8, 0);
                int randomPowerUp = RandomWithExclusion(0, 5, _previousPowerUp);
                _previousPowerUp = randomPowerUp;
                Instantiate(_powerups[randomPowerUp], powerUpSpawnPos, Quaternion.identity);
                yield return new WaitForSeconds(waitTime);
            }
    }

    private int RandomWithExclusion(int min, int max, int exclusion)
    {
        int result = Random.Range(min, max);
        if (result == 5 && exclusion != 5)
        {
            int homingChance = Random.Range(0, 99);
            if (homingChance > 94)
            {
                return result;
            }
            else
                return (result == exclusion) ? result - 1 : result;
        }
        return (result == exclusion) ? result - 1 : result;
    }

    public void onPlayerDeath ()
    {
        _stopSpawning = true;
    }
}
