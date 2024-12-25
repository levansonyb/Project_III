using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MainMenu : MonoBehaviour
{
    public TMP_Dropdown ddTime;
    public TMP_Dropdown ddLevel;
    public TMP_Dropdown ddSide;

    // Chế độ chơi online
    public void PlayGameOnline()
    {
        if (ddTime.value == 0)
        {
            PieceManager.whiteTime = 60;
            PieceManager.blackTime = 60;
        }
        if (ddTime.value == 1)
        {
            PieceManager.whiteTime = 300;
            PieceManager.blackTime = 300;
        }
        if (ddTime.value == 2)
        {
            PieceManager.whiteTime = 900;
            PieceManager.blackTime = 900;
        }
        if (ddTime.value == 3)
        {
            PieceManager.whiteTime = 3600;
            PieceManager.blackTime = 3600;
        }

        PieceManager.IAmode = false;
        SceneManager.LoadScene(2); // Game
    }

    // Chế độ chơi 2 người
    public void PlayGame()
    {
        if (ddTime.value == 0)
        {
            PieceManager.whiteTime = 60;
            PieceManager.blackTime = 60;
        }
        if (ddTime.value == 1)
        {
            PieceManager.whiteTime = 300;
            PieceManager.blackTime = 300;
        }
        if (ddTime.value == 2)
        {
            PieceManager.whiteTime = 900;
            PieceManager.blackTime = 900;
        }
        if (ddTime.value == 3)
        {
            PieceManager.whiteTime = 3600;
            PieceManager.blackTime = 3600;
        }

        PieceManager.IAmode = false;
        SceneManager.LoadScene(1); // Game
    }

    // Chế độ chơi với máy
    public void PlayIA()
    {
        PieceManager.IAmode = true;

        if (ddSide.value == 0)
            PieceManager.isIAWithe = false;
        if (ddSide.value == 1)
            PieceManager.isIAWithe = true;

        IA.level = IA.IA_Level[ddLevel.value];

        SceneManager.LoadScene(1); // Game
    }

    // Thoát ứng dụng
    public void QuitGame()
    {
        Debug.Log("Quit");
        Application.Quit();
    }
}
