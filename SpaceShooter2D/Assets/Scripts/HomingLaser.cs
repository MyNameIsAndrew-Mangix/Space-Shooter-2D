using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class HomingLaser : MonoBehaviour
{
    private HomingTargetFinder _homingTargetFinder;
    private List<Transform> _targetList;
    private Transform _chosenTarget;
    private float _laserSpeed = 10;
    void Start()
    {
        _homingTargetFinder = transform.Find("Homing_Target_Finder").GetComponent<HomingTargetFinder>();
        _targetList = _homingTargetFinder.GetTargetList();
    }

    void Update()
    {
        ChooseTarget();
        if (_chosenTarget != null)
        {
            transform.up = _chosenTarget.position - transform.position;
            transform.position = Vector3.MoveTowards(transform.position, _chosenTarget.position, (_laserSpeed / 1.5f) * Time.deltaTime);
        }
    }

    public void ChooseTarget()
    {
        _targetList = _homingTargetFinder.GetTargetList();
        _chosenTarget = _homingTargetFinder.GetClosestEnemy(_targetList);
    }

}
