using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{

    [SerializeField] 
    private GameObject[] _enemies;
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
        _previousPowerUp = 12;
        _powerUpCooldown = Random.Range(10.5f, 17.5f);
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
            int randomEnemy = Random.Range(0, 3);
            GameObject newEnemy = Instantiate(_enemies[randomEnemy], enemySpawnPos, Quaternion.identity);
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
                int randomPowerUp = PreventSamePowerUp(RollPowerUp(), _previousPowerUp); //RandomWithExclusion(0, 6, _previousPowerUp);
                _previousPowerUp = randomPowerUp;
                Instantiate(_powerups[randomPowerUp], powerUpSpawnPos, Quaternion.identity);
                yield return new WaitForSeconds(waitTime);
            }
    }

    private int RollPowerUp()
    {
        int roll = Random.Range(0, 101);
        if (roll <= 39)
            return 4;
        else if (roll >= 40 && roll <= 60)
            return 3;
        else if (roll >= 61 && roll <= 72)
            return 0;
        else if (roll >= 73 && roll <= 84)
            return 1;
        else if (roll >= 85 && roll <= 96)
            return 2;
        else if (roll >= 97 && roll <= 100)
            return 5;
        else
        {
            Debug.LogError("int roll is out of range");
            return 0;
        }
    }

    private int PreventSamePowerUp(int roll, int exclusion)
    {
        if (roll == exclusion)
        {
            switch (roll)
            {
                case 0:
                    int i = Random.Range(1, 6);
                    return roll + i;
                case 1:
                    i = Random.Range(1, 5);
                    return roll + i;
                case 2:
                    i = Random.Range(1, 4);
                    return roll + i;
                case 3:
                    i = Random.Range(1, 4);
                    return roll - i;
                case 4:
                    i = Random.Range(1, 5);
                    return roll - i;
                case 5:
                    i = Random.Range(1, 6);
                    return roll - i;
                default: 
                    return roll;
            }
        }
        else
            return roll;
    }

    public void onPlayerDeath ()
    {
        _stopSpawning = true;
    }
}
