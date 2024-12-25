using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ClockManager : MonoBehaviour
{

    [HideInInspector]
    public bool launched = false; // Xác định xem đồng hồ đã được bắt đầu chưa

    private Timer clockWhite;
    private Timer clockBlack;

    public TMP_Text displayWhite;
    public TMP_Text displayBlack;

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
    void Update()
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
}
