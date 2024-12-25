using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IA : MonoBehaviour
{
    System.Diagnostics.Process process = null; // Đại diện cho một tiến trình chạy chương trình Stockfish
    public static int level = 0; // Mức độ khó của AI
    string lastFEN; // Chuỗi FEN mô tả trạng thái hiện tại của bàn cờ -> Chuỗi này được sử dụng để đồng bộ hóa trạng thái giữa Unity và Stockfish.

    // Mức độ xử lý của AI
    public static Dictionary<int, int> IA_Level = new Dictionary<int, int>()
    {
        {0, 0},
        {1, 5},
        {2, 20}
    };

    // Cấp độ xử lý của AI
    public static Dictionary<int, int> IA_Game_Level = new Dictionary<int, int>()
    {
        {0, 1},
        {5, 2},
        {20, 3}
    };

    // Khởi tạo và cấu hình AI Stockfish
    public void Setup()
    {
        process = new System.Diagnostics.Process();
        process.StartInfo.FileName = Application.dataPath + "/Resources/IA/stockfish_13/stockfish_13_win_x64.exe";
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.CreateNoWindow = true;
        process.StartInfo.RedirectStandardInput = true;
        process.StartInfo.RedirectStandardOutput = true;
        process.Start();
        process.StandardInput.WriteLine("setoption Name Skill Level value " + level);
        process.StandardInput.WriteLine("position startpos");

        //lastFEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
        lastFEN = GetFEN();
    }

    // Dừng tiến trình Stockfish khi không cần sử dụng nữa
    public void Close()
    {
        process.Close();
    }

    // Tìm nước đi tốt nhất dựa trên trạng thái hiện tại
    public string GetBestMove()
    {
        string setupString = "position fen " + lastFEN;
        process.StandardInput.WriteLine(setupString);

        // Yêu cầu Stockfish tính toán nước đi tốt nhất trong 1 giây
        string processString = "go movetime 1";

        // Đọc kết quả từ đầu ra và lấy nước đi được đề xuất
        process.StandardInput.WriteLine(processString);

        string bestMoveInAlgebraicNotation = "";
        do
        {
            bestMoveInAlgebraicNotation = process.StandardOutput.ReadLine();
        } while (!bestMoveInAlgebraicNotation.Contains("bestmove"));

        bestMoveInAlgebraicNotation = bestMoveInAlgebraicNotation.Substring(9, 4);

        return bestMoveInAlgebraicNotation;
    }

    // Lấy trạng thái FEN hiện tại của bàn cờ từ Stockfish
    public string GetFEN()
    {
        process.StandardInput.WriteLine("d");
        string output = "";
        do
        {
            output = process.StandardOutput.ReadLine();
        }
        while (!output.Contains("Fen"));

        output = output.Substring(5);
        return output;
    }

    // Cập nhật trạng thái bàn cờ trong Stockfish sau một nước đi
    public void setIAmove(string move)
    {
        string setupString = "position fen " + lastFEN + " moves " + move;
        process.StandardInput.WriteLine(setupString);
        lastFEN = GetFEN();
    }
}
