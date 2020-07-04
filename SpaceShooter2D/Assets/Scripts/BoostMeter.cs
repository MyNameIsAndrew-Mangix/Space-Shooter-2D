using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.UI;

public class BoostMeter : MonoBehaviour
{
    [SerializeField]
    private Slider _slider;
    [SerializeField]
    private Text _displayText;
    private Player _player;

    private bool _canBoost = true;
    private float _currentValue = 0f;
    public float CurrentValue
    {
        get { return _currentValue; }
        set
        {
            _currentValue = value;
            _slider.value = _currentValue;
            _displayText.text = (_slider.value * 100).ToString("0.00") + "%";
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        CurrentValue = 1f;
        if (_player == null)
        {
            Debug.LogError("Player is NULL");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_player.IsBoosting && _player.IsMoving)
            CurrentValue -= 0.0073f;
        else if (CurrentValue <= 1)
            CurrentValue += 0.0043f;

        if (CurrentValue <= 0.005f)
        {
            _player.CannotBoost();
            _canBoost = false;
        }

        if (!_canBoost && CurrentValue >= 0.993)
        {
            _player.CanBoost();
            _canBoost = true;
        }

    }

}
