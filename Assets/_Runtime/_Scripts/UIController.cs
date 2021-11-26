using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
// ------------------------------------------------------------------------------ 
// Quiz  
// Written by: Alex Fourneaux 40022711
// For COMP 376 – Fall 2021 
// -----------------------------------------------------------------------------
public class UIController : MonoBehaviour
{
    // Allow access from any object
    public static UIController instance;
    [SerializeField] private List<Sprite> healthSprites;
    [SerializeField] private GameObject UI;
    [SerializeField] private float fadeSpeed = 10.0f;
    [SerializeField] private float fadeDelay = 2.0f;
    private float timeToFade;
    private float healthAlpha = 0f;
    private Image HealthImage;
    private GameObject PauseMenu;
    private TMPro.TMP_Text coinText;
    private List<GameObject> conversations;
    public bool dialogueOpen = false;  // If a dialogue box is on screen
    int activeConversation;

    void OnEnable() {
        instance = this;
        timeToFade = 0f;
        HealthImage = UI.transform.Find("Health").GetComponent<Image>();
        PauseMenu = UI.transform.Find("PauseMenu").gameObject;
        coinText = UI.transform.Find("CoinCount/Text").GetComponent<TMPro.TMP_Text>();
        conversations = new List<GameObject>();
        conversations.Add(UI.transform.Find("Conversation0").gameObject);
        conversations.Add(UI.transform.Find("Conversation1").gameObject);
        conversations.Add(UI.transform.Find("Conversation2").gameObject);
        conversations.Add(UI.transform.Find("Conversation3").gameObject);
        conversations.Add(UI.transform.Find("Conversation4").gameObject);
    }

    void Update()
    {
        // If necessary, fade away the health bar
        if (timeToFade <= 0f) {
            if (healthAlpha > 0f) {
                healthAlpha -= fadeSpeed * Time.deltaTime;
                HealthImage.color = new Color(1f, 1f, 1f, healthAlpha);
            }
        } else {
            timeToFade -= Time.deltaTime;
        }
        // Advance dialogue using the Jump key
        if (dialogueOpen && Input.GetButtonDown("Jump")) {
            DismissConversation(activeConversation);
        }
    }

    public void ShowHealth(int hp) 
    {
        // Display the health bar before slowly fading it away
        if (hp > healthSprites.Count || hp < 1) {
            Debug.LogError("UIController: Trying to show invalid health " + hp);
            return;
        }

        HealthImage.sprite = healthSprites[hp - 1];
        healthAlpha = 1f;
        HealthImage.color = Color.white;        // Reset alpha to 1 to make visible
        timeToFade = fadeDelay;
    }

    public void UpdateCoins(int coins) {
        // Update the text next to the coin display
        coinText.text = "x " + coins.ToString();
    }

    public void Pause(bool isPaused = true) {
        // Toggle pause menu visibility, cursor locking, sound volume, and timescale
        PauseMenu.SetActive(isPaused);
        Cursor.lockState = isPaused ? CursorLockMode.None : CursorLockMode.Locked;
        AudioController.instance.SetQuietSounds(isPaused);
        AudioController.instance.PlaySound("pause");
        Time.timeScale = dialogueOpen ? 0f : (isPaused ? 0f : 1f);
    }

    public void SetMusicQuality(int quality) {
        switch(quality) {
            case 1:
                GameController.instance.musicQuality = GameController.MusicQuality.MUTE;
                break;
            case 2:
                GameController.instance.musicQuality = GameController.MusicQuality.LOW;
                break;
            case 3:
                GameController.instance.musicQuality = GameController.MusicQuality.HIGH;
                break;
            default:
                break;
        }
    }

    public void RestartGame() {
        GameController.instance.RestartGame();
    }

    public void QuitGame() {
        Application.Quit();
    }

    public void DisplayConversation(int conversation) {
        dialogueOpen = true;
        conversations[conversation].SetActive(true);
        Time.timeScale = 0f;
        activeConversation = conversation;
    }

    public void DismissConversation(int conversation) {
        dialogueOpen = false;
        conversations[conversation].SetActive(false);
        Time.timeScale = 1f;
        activeConversation = -1;
    }
}
