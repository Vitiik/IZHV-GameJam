using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour
{
    public int levelNumber;

    float time;
    int diamond;
    TextMeshProUGUI levelNumberText;
    TextMeshProUGUI levelTimeText;
    Image diamondImage;

    Button button;

    void Start()
    {
        button = transform.GetComponent<Button>();

        button.onClick.AddListener(OnButtonClick);

        levelNumberText = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        levelTimeText = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        diamondImage = transform.GetChild(3).GetComponent<Image>();

        levelNumberText.text = levelNumber.ToString();


        time = PlayerPrefs.GetFloat(levelNumber.ToString()+"_time");
        diamond = PlayerPrefs.GetInt(levelNumber.ToString()+"_diamond");

        if(time != 0f){
            TimeSpan timeSpan = TimeSpan.FromSeconds(time);
            levelTimeText.text = timeSpan.ToString(@"m\:ss\.ff");
        }

        if(diamond == 1){
            diamondImage.gameObject.SetActive(true);
        }
    }

    void OnButtonClick(){
        SceneManager.LoadScene("Level" + levelNumber.ToString());
    }
}
