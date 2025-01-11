using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using TMPro;
using System;

public class GameManager : Singleton<GameManager>
{
    public int levelNumber;
    public Light2D globalLight;
    public int hitTaken = 0;
    float globalLightStartIntensity;

    [Header("UI")]
    public TextMeshProUGUI timerText;
    public GameObject gameOverPanel;
    public GameObject levelCompletePanel;

    [Header("Sounds")]
    public AudioClip chaseMusic;
    public AudioClip winSound;
    public AudioClip loseSound;
    [HideInInspector]
    public AudioSource source;

    double time;
    [HideInInspector]
    public bool diamondPickedUp;
    bool playerWon = false;

    void Start(){
        globalLightStartIntensity = globalLight.intensity;
        source = GetComponent<AudioSource>();
    }

    void Update(){
        if(Input.GetKeyDown(KeyCode.R)){
            RestartLevel();
        }
        if(Input.GetKeyDown(KeyCode.Escape)){
            BackToMainMenu();
        }
        if(Input.GetKeyDown(KeyCode.Space)){
            NextLevel();
        }
        if(hitTaken < 3){
            time += Time.deltaTime;
        }
        TimeSpan timeSpan = TimeSpan.FromSeconds(time);

        if(timeSpan.Minutes > 0){
            timerText.text = timeSpan.ToString(@"m\:ss\.ff");
        }else{
            timerText.text = timeSpan.ToString(@"s\.ff");
        }
    }

    public void PlayerTakeHit(){
        if(playerWon) return;
        hitTaken ++;
        LeanTween.value( gameObject, AdjustLight, globalLightStartIntensity, globalLightStartIntensity-0.14f, 0.1f).setEase(LeanTweenType.easeOutElastic);
        if(hitTaken < 3){
            LeanTween.value( gameObject, AdjustLight, globalLightStartIntensity-0.14f, globalLightStartIntensity, 0.3f).setEase(LeanTweenType.easeOutCubic).setDelay(0.1f);
        }else{
            PlayerController.instance.lockControls = true;
            LeanTween.value( gameObject, AdjustLight, 0.01f, 0f, 1f).setEase(LeanTweenType.easeOutCubic).setDelay(0.1f);
            DisplayGameOver();
        }
    }
    public void AdjustLight(float val){
        globalLight.intensity = val;
    }

    public void TMProAlpha(float val,TextMeshProUGUI text){
        text.alpha = val;
    }

    public void DisplayGameOver(){
        source.PlayOneShot(loseSound);
        LeanTween.value(timerText.gameObject,(x) => TMProAlpha(x,timerText),1f,0f,0.5f).setEaseInOutExpo();
        gameOverPanel.SetActive(true);
    }

    public void DisplayLevelComplete(){
        LeanTween.value(timerText.gameObject,(x) => TMProAlpha(x,timerText),1f,0f,0.5f).setEaseInOutExpo();
        levelCompletePanel.SetActive(true);
        TimeSpan timeSpan = TimeSpan.FromSeconds(time);
        levelCompletePanel.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = timeSpan.ToString(@"m\:ss\.ff");
    }

    public void Win(){
        source.PlayOneShot(winSound);
        playerWon = true;
        PlayerPrefs.SetInt(levelNumber.ToString()+"_diamond",diamondPickedUp ? 1 : 0);
        PlayerPrefs.SetFloat(levelNumber.ToString()+"_time",(float)time);
        LeanTween.value( gameObject, AdjustLight, globalLight.intensity, 0f, 1f).setEase(LeanTweenType.easeOutCubic);
        PlayerController.instance.lockControls = true;
        DisplayLevelComplete();
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void NextLevel(){
        if(!playerWon) return;
        SceneManager.LoadScene("Level" + (levelNumber+1).ToString());
    }

    public void RestartLevel(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void BackToMainMenu(){
        SceneManager.LoadScene("MainMenu");
    }
}