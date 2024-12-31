using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SinglePlayerBoard : Board
{
    public override void Create()
    {
        if (CellObject == null)
        {
            Debug.LogError("CellObject không được gán trong SinglePlayerBoard!");
            return;
        }

        CreateCells();
    }

    // Tạo ô cờ
    private void CreateCells()
    {
        float board_width = GetComponent<RectTransform>().rect.width;
        float board_height = GetComponent<RectTransform>().rect.height;

        // Khởi tạo danh sách rỗng cho allCells
        allCells = new List<List<Cell>>();

        // Tạo các ô cờ
        for (int x = 0; x < Column; x++)
        {
            List<Cell> row_cell = new List<Cell>();
            allCells.Add(row_cell); // Thêm dòng vào danh sách

            for (int y = 0; y < Row; y++)
            {
                GameObject newCell = Instantiate(CellObject, transform);
                RectTransform rectTransform = newCell.GetComponent<RectTransform>();

                float cell_width = board_width / Column;
                float cell_height = board_height / Row;
                rectTransform.anchoredPosition = new Vector2(x * cell_width + cell_width / 2, y * cell_height + cell_height / 2);
                rectTransform.sizeDelta = new Vector2(cell_width, cell_height);

                // Thêm ô vào danh sách
                Cell cell = newCell.GetComponent<Cell>();
                if (cell == null)
                {
                    Debug.LogError($"Cell tại vị trí ({x}, {y}) không thể tìm thấy thành phần Cell.");
                    continue;
                }

                cell.Setup(new Vector2Int(x, y), this);
                row_cell.Add(cell);
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
