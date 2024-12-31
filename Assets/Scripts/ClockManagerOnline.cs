using System.Collections;
using Photon.Pun;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ClockManagerOnline : MonoBehaviourPun
{
    [HideInInspector]
    public bool launched = false;
    public TMP_Text displayWhite;
    public TMP_Text displayBlack;

    private Timer clockWhite;
    private Timer clockBlack;

    public GameObject highlightClockW;
    public GameObject highlightClockB;

    private PieceManager pm = null;
    private bool isWhiteTurn = true;

    // Cài đặt đồng hồ cho trò chơi online
    public void Setup(float whiteTime, float blackTime, bool isWhite, PieceManager newPm)
    {
        pm = newPm;
        launched = false;

        clockWhite = new Timer();
        clockBlack = new Timer();

        clockWhite.Setup(whiteTime, displayWhite);
        clockBlack.Setup(blackTime, displayBlack);

        highlightClockW.SetActive(true);
        highlightClockB.SetActive(false);

        ApplyClockColor();

        if (!isWhite)
        {
            SwapClockDisplay();
        }
    }

    // Đổi màu và vị trí hiển thị đồng hồ cho người chơi đen
    private void SwapClockDisplay()
    {
        SwapText(displayWhite, displayBlack);
        SwapColor(highlightClockW, highlightClockB);
    }

    // Đổi text của 2 đồng hồ
    private void SwapText(TMP_Text text1, TMP_Text text2)
    {
        string tempText = text1.text;
        text1.text = text2.text;
        text2.text = tempText;
    }

    // Đổi màu của 2 highlight
    private void SwapColor(GameObject obj1, GameObject obj2)
    {
        Color tempColor = obj1.GetComponent<Image>().color;
        obj1.GetComponent<Image>().color = obj2.GetComponent<Image>().color;
        obj2.GetComponent<Image>().color = tempColor;
    }

    // Bắt đầu đếm giờ
    public void StartClocks()
    {
        clockWhite.Start();
        clockBlack.Start();
        launched = true;
    }

    // Cập nhật thời gian cho mỗi lượt
    public void UpdateClocks()
    {
        if (launched)
        {
            if (isWhiteTurn)
                clockWhite.Update();
            else
                clockBlack.Update();

            CheckTimeOut();
        }
    }

    // Kiểm tra hết thời gian
    private void CheckTimeOut()
    {
        if (clockBlack.runOut)
        {
            pm.gameState = GameState.WHITE_WIN;
            pm.ShowResult();
            StopClocks();
        }
        else if (clockWhite.runOut)
        {
            pm.gameState = GameState.BLACK_WIN;
            pm.ShowResult();
            StopClocks();
        }
    }

    // Dừng đồng hồ
    public void StopClocks()
    {
        clockWhite.Stop();
        clockBlack.Stop();
        launched = false;
    }

    // Chuyển lượt chơi và đồng bộ qua mạng
    public void ChangeTurn()
    {
        isWhiteTurn = !isWhiteTurn;
        photonView.RPC("RPC_ChangeTurn", RpcTarget.All, isWhiteTurn);
    }

    [PunRPC]
    void RPC_ChangeTurn(bool newTurn)
    {
        isWhiteTurn = newTurn;
        ToggleClockHighlight();
    }

    // Đổi trạng thái hiển thị của đồng hồ theo lượt chơi
    private void ToggleClockHighlight()
    {
        highlightClockW.SetActive(isWhiteTurn);
        highlightClockB.SetActive(!isWhiteTurn);
    }

    // Xoay đồng hồ cho người chơi và cập nhật trạng thái
    public void RotateClocks()
    {
        SwapText(displayWhite, displayBlack);
        SwapColor(highlightClockW, highlightClockB);

        isWhiteTurn = !isWhiteTurn;
        ToggleClockHighlight();

        if (isWhiteTurn)
        {
            clockWhite.Start();
            clockBlack.Stop();
        }
        else
        {
            clockBlack.Start();
            clockWhite.Stop();
        }
    }

    private void ApplyClockColor()
    {
        highlightClockW.GetComponent<Image>().color = new Color(0.36f, 0.68f, 0.27f, 1);
        highlightClockB.GetComponent<Image>().color = new Color(0.36f, 0.68f, 0.27f, 1);
    }
}
