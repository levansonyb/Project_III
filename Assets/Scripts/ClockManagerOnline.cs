using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

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

        highlightClockW.GetComponent<Image>().color = new Color(0.36f, 0.68f, 0.27f, 1);
        highlightClockB.GetComponent<Image>().color = new Color(0.36f, 0.68f, 0.27f, 1);

        if (!isWhite)
        {
            // Đổi chiều hiển thị đồng hồ cho người chơi đen
            SwapClockDisplay();
        }
    }

    private void SwapClockDisplay()
    {
        var tempText = displayWhite.text;
        displayWhite.text = displayBlack.text;
        displayBlack.text = tempText;

        var tempColor = highlightClockW.GetComponent<Image>().color;
        highlightClockW.GetComponent<Image>().color = highlightClockB.GetComponent<Image>().color;
        highlightClockB.GetComponent<Image>().color = tempColor;
    }

    public void StartClocks()
    {
        clockWhite.Start();
        clockBlack.Start();
        launched = true;
    }

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

    private void CheckTimeOut()
    {
        if (clockBlack.runOut)
        {
            pm.gameState = GameState.WHITE_WIN;
            pm.ShowResult();
            StopClocks();
        }
        if (clockWhite.runOut)
        {
            pm.gameState = GameState.BLACK_WIN;
            pm.ShowResult();
            StopClocks();
        }
    }

    public void StopClocks()
    {
        clockWhite.Stop();
        clockBlack.Stop();
        launched = false;
    }

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

    private void ToggleClockHighlight()
    {
        highlightClockW.SetActive(isWhiteTurn);
        highlightClockB.SetActive(!isWhiteTurn);
    }

    public void RotateClocks()
    {
        // Xoay các thành phần đồng hồ của người chơi trắng và đen
        var tempPos = displayWhite.transform.position;
        displayWhite.transform.position = displayBlack.transform.position;
        displayBlack.transform.position = tempPos;

        var tempHighlight = highlightClockW.activeSelf;
        highlightClockW.SetActive(highlightClockB.activeSelf);
        highlightClockB.SetActive(tempHighlight);

        // Thay đổi màu sắc của đồng hồ khi xoay
        var tempColor = highlightClockW.GetComponent<Image>().color;
        highlightClockW.GetComponent<Image>().color = highlightClockB.GetComponent<Image>().color;
        highlightClockB.GetComponent<Image>().color = tempColor;

        // Cập nhật trạng thái lượt chơi (nếu cần)
        isWhiteTurn = !isWhiteTurn;
        ToggleClockHighlight();

        // Cập nhật đồng hồ
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

    internal void Setup(int v1, int v2, GameManagerOnline gameManagerOnline)
    {
        throw new NotImplementedException();
    }
}
