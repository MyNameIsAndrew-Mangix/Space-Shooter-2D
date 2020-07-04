using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _scoreText;
    [SerializeField]
    private TextMeshProUGUI _ammoText;
    [SerializeField]
    private Image _livesImage;
    [SerializeField]
    private Sprite[] _lifeSprites;
    [SerializeField]
    private TextMeshProUGUI _gameOverText;
    private Animator _gameOverAnimator;
    [SerializeField]
    private TextMeshProUGUI _restartLevelText;

    private GameManager _GM;
    // Start is called before the first frame update
    void Start()
    {
        _restartLevelText.gameObject.SetActive(false);
        _scoreText.text = "Score: " + 0;
        _ammoText.text = "Ammo: " + 15;
        _gameOverText.gameObject.SetActive(false);
        _gameOverAnimator = _gameOverText.GetComponent<Animator>();

        if (_gameOverAnimator == null)
        {
            Debug.LogError("Animator is NULL");
        }

        _GM = GameObject.Find("Game_Manager").GetComponent<GameManager>();

        if (_GM == null)
        {
            Debug.LogError("GameManager is NULL");
        }

    }


    public void ScoreUpdate(int playerScore)
    {
        _scoreText.text = "Score: " + playerScore.ToString();
    }

    public void UpdateAmmo (int ammoCount)
    {
        _ammoText.text = "Ammo: " + ammoCount.ToString();
        if (ammoCount <= 0)
        {
            _ammoText.text = "Ammo: " + ammoCount.ToString() + ". Out of ammo!";
        }
    }

    public void UpdateLives(int currentLives)
    {
        _livesImage.sprite = _lifeSprites[currentLives];
        if (currentLives == 0)
        {
            GameOverSequence();
        }
    }

    private void GameOverSequence()
    {
        _gameOverText.gameObject.SetActive(true);
        _restartLevelText.gameObject.SetActive(true);
        _gameOverAnimator.Play("Game_Over_Text_anim", 0, 0);
        _GM.GameOver();

    }
}
