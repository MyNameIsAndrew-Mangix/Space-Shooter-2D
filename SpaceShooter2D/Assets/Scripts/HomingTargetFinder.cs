using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingTargetFinder : MonoBehaviour
{
    private HomingLaser _homingLaser;
    private List<Transform> _targetList = new List<Transform>();

    private void Start()
    {
        _homingLaser = transform.parent.gameObject.GetComponent<HomingLaser>();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            _targetList.Add(other.transform);
        }

        _homingLaser.ChooseTarget();

    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (_targetList.Contains(other.transform))
        {
            _targetList.Remove(other.transform);
        }
    }

    public List<Transform> GetTargetList()
    {
        return _targetList;
    }

    public Transform GetClosestEnemy (List<Transform> enemies)
    {
        Transform transformMin = null;
        float minDist = Mathf.Infinity;
        UnityEngine.Vector3 currentPos = transform.position;
        foreach (Transform potentialTarget in enemies)
        {
            float dist = UnityEngine.Vector3.Distance(potentialTarget.position, currentPos);
            if(dist < minDist)
            {
                transformMin = potentialTarget;
                minDist = dist;
            }
        }
        return transformMin;
    }

}
