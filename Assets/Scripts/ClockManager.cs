using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ClockManager : MonoBehaviour
{

    [HideInInspector]
    public bool launched = false; // Xác định xem đồng hồ đã được bắt đầu chưa

    public TMP_Text displayWhiteLab; // Hiển thị thời gian cho người chơi trắng
    public TMP_Text displayBlackLab; // Hiển thị thời gian cho người chơi đen

    private Timer clockWhite; // Đồng hồ cho người chơi trắng
    private Timer clockBlack; // Đồng hồ cho người chơi đen

    public TMP_Text displayWhite; // Hiển thị thời gian cho người chơi trắng
    public TMP_Text displayBlack; // Hiển thị thời gian cho người chơi đen

    public GameObject highlightClockW;
    public GameObject highlightClockB;

    private PieceManager pm = null; // Tham chiếu đến quản lý quân cờ, để cập nhật trạng thái trò chơi

    private bool isWhiteTurn = true;

    // Khởi tạo đồng hồ và đặt ô sáng ban đầu
    public void Setup(float whiteTime, float blackTime, PieceManager newPm)
    {

        pm = newPm;
        launched = false;
        clockWhite = new Timer();
        clockBlack = new Timer();

        displayWhiteLab.text = "White";
        displayBlackLab.text = "Black";

        clockWhite.Setup(whiteTime, displayWhite);
        clockBlack.Setup(blackTime, displayBlack);

        highlightClockW.SetActive(true);
        highlightClockB.SetActive(false);

        highlightClockW.GetComponent<Image>().color = new Color((float)0.36, (float)0.68, (float)0.27, 1);
        highlightClockB.GetComponent<Image>().color = new Color((float)0.36, (float)0.68, (float)0.27, 1);
    }

    // Bắt đầu cả hai đồng hồ
    public void StartClocks()
    {
        clockWhite.Start();
        clockBlack.Start();
        launched = true;
    }

    // cập nhật thời gian còn lại của người chơi hiện tại và xử lý hết thời gian
    public void Update()
    {
        if (launched)
        {
            if (isWhiteTurn == true)
            {
                clockWhite.Update();
            }
            else
            {
                clockBlack.Update();
            }
            if (clockBlack.runOut)
            {
                pm.gameState = GameState.WHITE_WIN;
                pm.ShowResult();
                launched = false;
            }
            if (clockWhite.runOut)
            {
                pm.gameState = GameState.BLACK_WIN;
                pm.ShowResult();
                launched = false;
            }
        }
    }

    // Dừng cả hai đồng hồ
    public void StopClocks()
    {
        clockWhite.Stop();
        clockBlack.Stop();
    }

    // Chuyển lượt chơi
    public void changeTurn()
    {
        isWhiteTurn = !isWhiteTurn;
        highlightClockW.SetActive(!highlightClockW.activeSelf);
        highlightClockB.SetActive(!highlightClockB.activeSelf);

    }

    // Đặt lượt chơi ban đầu
    public void setTurn(bool isWhiteTurn)
    {
        this.isWhiteTurn = isWhiteTurn;
        highlightClockW.SetActive(isWhiteTurn);
        highlightClockB.SetActive(!isWhiteTurn);
    }

    public void Text()
    {
        displayWhiteLab.text = "White";
        displayBlackLab.text = "Black";
    }

    public void ReverseClocks()
    {
        // Xoay các thành phần đồng hồ của người chơi trắng và đen
        var tempPos = displayWhite.transform.position;
        displayWhite.transform.position = displayBlack.transform.position;
        displayBlack.transform.position = tempPos;

        var temp = displayWhiteLab.transform.position;
        displayWhiteLab.transform.position = displayBlackLab.transform.position;
        displayBlackLab.transform.position = temp;

        var tempHighlight = highlightClockW.activeSelf;
        highlightClockW.SetActive(highlightClockB.activeSelf);
        highlightClockB.SetActive(tempHighlight);

        // Thay đổi màu sắc của đồng hồ khi xoay
        var tempColor = highlightClockW.GetComponent<Image>().color;
        highlightClockW.GetComponent<Image>().color = highlightClockB.GetComponent<Image>().color;
        highlightClockB.GetComponent<Image>().color = tempColor;


    }


    internal void Setup(int v1, int v2, GameManagerOnline gameManagerOnline)
    {
        throw new NotImplementedException();
    }

}
