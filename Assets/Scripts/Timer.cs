using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Timer
{
    private float timeRemaining; // Thời gian còn lại (được cập nhật liên tục)
    private bool timerIsRunning = false; // Xác định trạng thái của bộ đếm
    private TMP_Text clock;

    [HideInInspector]
    public bool runOut = false; // Xác định liệu thời gian có hết chưa 

    // Dừng bộ đếm thời gian
    public void Stop()
    {
        timerIsRunning = false;
    }

    // Cấu hình bộ đếm thời gian
    public void Setup(float timeMax, TMP_Text display)
    {
        runOut = false;
        timeRemaining = timeMax;
        clock = display;
        clock.color = Color.black;

        DisplayTime(timeRemaining);
    }

    // Bắt đầu chạy bộ đếm thời gian
    public void Start()
    {
        timerIsRunning = true;
        DisplayTime(timeRemaining);
    }

    // Cập nhật thời gian còn lại
    public void Update()
    {
        if (timerIsRunning)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                DisplayTime(timeRemaining);
            }
            else
            {
                Debug.Log("Time has run out !");
                timeRemaining = 0;
                timerIsRunning = false;
                clock.text = string.Format("0:{0:00}:{1:0}", 0, 0);
                clock.color = Color.red;
                timerIsRunning = false;
                runOut = true;
            }
        }
    }

    // Hiển thị thời gian dưới dạng văn bản
    public void DisplayTime(float timeToDisplay)
    {
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);
        float tenthOfSecond;
        if (minutes == 0 && seconds < 20)
        {
            tenthOfSecond = Mathf.FloorToInt((timeToDisplay % 1) * 10);
            clock.text = string.Format("0:{0:00}:{1:0}", seconds, tenthOfSecond);

        }
        else
        {
            clock.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }
}