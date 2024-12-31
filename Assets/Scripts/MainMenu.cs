using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenu : MonoBehaviour
{
    public TMP_Dropdown ddTime;
    public TMP_Dropdown ddLevel;
    public TMP_Dropdown ddIASide;
    public TMP_Dropdown ddOnlineSide;

    private readonly int[] timeOptions = { 60, 300, 900, 3600 };

    // Chế độ chơi online
    public void PlayOnline()
    {
        SetGameTime(ddTime.value);
        PieceManager.player1 = (ddOnlineSide.value == 0);

        PieceManager.IAmode = false;
        PieceManager.Online = true;
        LoadScene(2);
    }

    // Chế độ chơi 2 người
    public void PlayGame()
    {
        SetGameTime(ddTime.value);

        PieceManager.IAmode = false;
        PieceManager.Online = false;
        LoadScene(1);
    }

    // Chế độ chơi với máy
    public void PlayIA()
    {
        PieceManager.IAmode = true;
        PieceManager.isAIWhite = (ddIASide.value == 1);

        IA.level = IA.IA_Level[ddLevel.value];
        LoadScene(1);
    }

    // Thoát ứng dụng
    public void QuitGame()
    {
        Debug.Log("Quit");
        Application.Quit();
    }

    // Cài đặt thời gian cho trò chơi
    private void SetGameTime(int index)
    {
        if (index >= 0 && index < timeOptions.Length)
        {
            PieceManager.whiteTime = timeOptions[index];
            PieceManager.blackTime = timeOptions[index];
        }
        else
        {
            Debug.LogError("Invalid time option selected.");
        }
    }

    // Tải scene
    private void LoadScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }
}
