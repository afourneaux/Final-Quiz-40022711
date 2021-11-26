using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
// ------------------------------------------------------------------------------ 
// Quiz  
// Written by: Alex Fourneaux 40022711
// For COMP 376 – Fall 2021 
// -----------------------------------------------------------------------------
public class GameController : MonoBehaviour
{
    // Allow access from any object
    public static GameController instance;
    private int maxHealth = 8;
    private bool isPaused = false;
    public enum MusicQuality {MUTE, LOW, HIGH};
    int musicIndex;
    private MusicQuality _musicQuality;
    public bool raceStarted = false;
    public bool booFinished = false;
    public bool marioFinished = false;
    public bool marioCursed = false;
    private float curseTimer = 0f;
    private float curseDelay = 0.5f;
    public bool gameOver = false;

    public MusicQuality musicQuality {
        get {
            return _musicQuality;
        }
        set {
            // When the music quality is changed, start the appropriate song version
            if (value != _musicQuality) {
                AudioController.instance.StopLoopingSound(musicIndex);
                switch(value) {
                    case MusicQuality.MUTE:
                        musicIndex = -1;
                        break;
                    case MusicQuality.LOW:
                        musicIndex = AudioController.instance.PlayLoopingSound("music_low");
                        break;
                    case MusicQuality.HIGH:
                        musicIndex = AudioController.instance.PlayLoopingSound("music_high");
                        break;
                    default:
                        Debug.LogError("UNRECOGNISED MUSIC QUALITY: " + value);
                        break;
                }
                _musicQuality = value;
            }
        }
    }

    private int _health;
    public int health {
        get {
            return _health;
        }
        set {
            if (value < _health) {
                // If the new health value is less than the previous, take damage
                AudioController.instance.PlaySound("mario_ouf");
            }
            if (value > maxHealth) {
                _health = maxHealth;
            } else {
                _health = value;
            }
            if (value > 0) {
                UIController.instance.ShowHealth(_health);
            } else {
                // When out of health, the game is over
                UIController.instance.DisplayConversation(4);
                gameOver = true;
                // TODO: Play the sound of Mario defeated
            }
        }
    }

    private int _coins;
    public int coins {
        get {
            return _coins;
        }
        set {
            // Ensure the UI is always up to date with the coin count
            UIController.instance.UpdateCoins(value);
            _coins = value;
        }
    }

    void OnEnable() {
        instance = this;
    }

    void Start()
    {
        musicQuality = MusicQuality.HIGH;
        health = maxHealth;
        coins = 0;
    }

    void Update()
    {
        if (marioCursed) {
            curseTimer += Time.deltaTime;
            if (curseTimer > curseDelay) {
                curseTimer = 0f;
                TakeDamage();
            }
        }
        if (UIController.instance.dialogueOpen == false && Input.GetButtonDown("Submit")) {
            PauseGame();
        }

        if (UIController.instance.dialogueOpen == false && gameOver) {
            RestartGame();
        }
    }

    public void GetCoin(int value = 1) {
        AudioController.instance.PlaySound("coin_get");
        coins += value;
        health += value;
    }

    public void TakeDamage(int value = 1) {
        AudioController.instance.PlaySound("mario_ouf");
        health -= value;
    }

    public void PauseGame() {
        isPaused = !isPaused;
        UIController.instance.Pause(isPaused);
    }

    public void PauseGame(bool setPaused) {
        isPaused = setPaused;
        UIController.instance.Pause(isPaused);
    }

    public void RestartGame() {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
