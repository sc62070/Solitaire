using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public enum UIPanel { 
        GAME,
        PLAY,
        SETTING
    }

    public UIPanel uiPanel;

    public GameObject gamePanel;
    public GameObject playPanel;
    public GameObject settingPanel;

    public Text effectSoundText;
    public Text threeModeText;

    public void UIPanelState(UIPanel uiPanel)
    {
        this.uiPanel = uiPanel;

        //gamePanel.SetActive(false);
        playPanel.SetActive(false);
        settingPanel.SetActive(false);
        GameController.Instance.canClick = false;

        if(uiPanel == UIPanel.GAME)
        {
            gamePanel.SetActive(true);
            GameController.Instance.canClick = true;
        }
        else if(uiPanel == UIPanel.PLAY)
        {
            playPanel.SetActive(true);
        }
        else if(uiPanel == UIPanel.SETTING)
        {
            UpdateSettingText();
            settingPanel.SetActive(true);
        }
    }

    public void UpdateSettingText()
    {
        bool effectSound = Settings.Instance.intToBool(Settings.Instance.effectSoundActive);
        bool threeMode = Settings.Instance.intToBool(Settings.Instance.threeModeTurnActive);

        if (effectSound)
            effectSoundText.text = "Effect Sound: On";
        else
            effectSoundText.text = "Effect Sound: Off";

        if (threeMode)
            threeModeText.text = "Three Mode: On";
        else
            threeModeText.text = "Three Mode: Off";

    }

    public void OnClickPlayPanelButton()
    {
        UIPanelState(UIPanel.PLAY);
    }

    public void OnClickSettingPanelButton()
    {
        UIPanelState(UIPanel.SETTING);
    }

    public void OnClickContinueButton()
    {
        UIPanelState(UIPanel.GAME);
    }

    public void OnClickRepeatButton()
    {
        UndoMovement.Instance.BackToFirstMove();
        GameController.Instance.ResetGame();
        UIPanelState(UIPanel.GAME);
    }

    public void OnClickNewGameButton()
    {
        SceneManager.LoadScene("Game");
    }

    public void OnClickExitButton()
    {
        Application.Quit();
    }

    public void OnClickEffectSoundButton()
    {
        Settings.Instance.ChangeEffectSound();
        UpdateSettingText();
    }

    public void OnClickThreeModeTurnButton()
    {
        Settings.Instance.ChangeThreeModeTurn();
        UpdateSettingText();
    }

    public void OnClickBackButton()
    {
        UIPanelState(UIPanel.GAME);
    }
}
