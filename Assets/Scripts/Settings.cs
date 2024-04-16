using System.Collections;
using System.Collections.Generic;
using Lofelt.NiceVibrations;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    public static Settings Instance;

    [SerializeField]private List<Button> soundButtonsList,vibrationButtonsList;

    public bool soundEnabled, vibrationEnabled,settingsEnabled;


    void Awake()
    {
        if (Instance==null)
        {
            Instance = this;
        }

        CheckSettings();
    }
    public void SettingsButtonPressed(GameObject obj)
    {
        if (!settingsEnabled)
            GameUI.Instance.StopTimer();
        else
            GameUI.Instance.StartTimer();

        settingsEnabled = !settingsEnabled;
        obj.SetActive(settingsEnabled);
    }
    public void RetryButtonPressed(GameObject obj)
    {
        LevelManager.Instance.LoadNextLevel();
        obj.SetActive(false);
    }
    private void CheckSettings()
    {
        if (PlayerPrefs.GetInt("SoundsActive", 1) == 1)
            soundEnabled = true;
        else
            soundEnabled = false;


        if (PlayerPrefs.GetInt("VibrationActive", 1) == 1)
            vibrationEnabled = true;
        else
            vibrationEnabled = false;

        ChangeSoundSettings();
        ChangeVibrationSettings();
    }
    public void SoundButtonPressed()
    {
      
        if (soundEnabled)
        {
            soundEnabled = false;
            PlayerPrefs.SetInt("SoundsActive", 0);
        }
            
        else
        {
            soundEnabled = true;
            PlayerPrefs.SetInt("SoundsActive", 1);
        }

        ChangeSoundSettings();
    }
    public void VibrationButtonPressed()
    {
        if (vibrationEnabled)
        {
            vibrationEnabled = false;
            PlayerPrefs.SetInt("VibrationActive", 0);
        }
        else
        {
            vibrationEnabled = true;
            PlayerPrefs.SetInt("VibrationActive", 1);
        }
        ChangeVibrationSettings();

    }
    private void ChangeSoundSettings()
    {
        foreach (var item in soundButtonsList)
        {
           item.transform.GetChild(0).gameObject.SetActive(!soundEnabled);
        }

        if (soundEnabled)
            AudioListener.volume = 1;
        else
            AudioListener.volume = 0;
    }
    private void ChangeVibrationSettings()
    {
        foreach (var item in vibrationButtonsList)
        {
            item.transform.GetChild(0).gameObject.SetActive(!vibrationEnabled);
        }

        if (vibrationEnabled)
            HapticController.hapticsEnabled = true;
        else
            HapticController.hapticsEnabled = false;
       
    }
}
