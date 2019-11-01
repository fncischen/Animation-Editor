using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TimelinePlayButton : MonoBehaviour
{
    public Timeline timeline;
    public Button playButton;
    public Button stopButton;

    public void Awake()
    {
        playButton.gameObject.SetActive(true);
        stopButton.gameObject.SetActive(false);
    }

    public void HidePlayButton()
    {
        playButton.gameObject.SetActive(false);
        stopButton.gameObject.SetActive(true);
    }

    public void HideStopButton()
    {
        playButton.gameObject.SetActive(true);
        stopButton.gameObject.SetActive(false);
    }
}
