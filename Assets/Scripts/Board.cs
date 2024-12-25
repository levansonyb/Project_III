using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Board : MonoBehaviour
{
    public GameObject CellObject;

    [HideInInspector]
    private int row = 8;
    [HideInInspector]
    private int column = 8;

    [HideInInspector]
    public List<List<Cell>> allCells = new List<List<Cell>>();

    public int Row { get => row; set => row = value; }
    public int Column { get => column; set => column = value; }

    public void Create()
    {
        float board_width = GetComponent<RectTransform>().rect.width;
        float board_height = GetComponent<RectTransform>().rect.height;

        // Khởi tạo bàn cờ
        for (int x = 0; x < Column; x++)
        {
            List<Cell> row_cell = new List<Cell>();
            allCells.Add(row_cell);
            for (int y = 0; y < Row; y++)
            {
                // Khởi tạo ô cờ
                GameObject newCell = Instantiate(CellObject, transform);

                // Vị trí
                RectTransform rectTransform = newCell.GetComponent<RectTransform>();

                float cell_width = board_width / Column;
                float cell_height = board_height / Row;
                rectTransform.anchoredPosition = new Vector2(x * cell_width + cell_width / 2, y * cell_height + cell_height / 2);
                rectTransform.sizeDelta = new Vector2(cell_width, cell_height);

                // Thiết lập
                row_cell.Add(newCell.GetComponent<Cell>());
                allCells[x][y].Setup(new Vector2Int(x, y), this);
            }
        }

        // Tô màu so le
        for (int x = 0; x < Column; x += 2)
        {
            for (int y = 0; y < Row; y++)
            {
                // So le mỗi dòng
                int offset = (y % 2 != 0) ? 0 : 1;
                int finalX = x + offset;

                // Màu
                Color col = new Color32(230, 220, 187, 255);
                Image im = allCells[finalX][y].GetComponent<Image>();
                im.color = col;
            }
        }
    }
}
