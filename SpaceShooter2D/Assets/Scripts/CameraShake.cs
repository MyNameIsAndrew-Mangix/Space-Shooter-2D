using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    private Transform _camTransform;

    private float _shakeDuration = 0f;

    private float _shakeAmount = 0.3f;
    private float _decreaseFactor = 1.5f;

    private Vector3 _originalPos;

    private void Awake()
    {
        if (_camTransform == null)
        {
            _camTransform = GetComponent<Transform>();
        }
    }
    void Start()
    {
        _originalPos = _camTransform.position;
    }

    private void Update()
    {
        if (_shakeDuration > 0)
        {
            _camTransform.position = _originalPos + Random.insideUnitSphere * _shakeAmount;

            _shakeDuration -= Time.deltaTime * _decreaseFactor;
        }
        else
        {
            _shakeDuration = 0f;
            _camTransform.position = _originalPos;
        }
    }

    public void TriggerShake()
    {
        _shakeDuration = 0.55f;
    }


}
