using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;

public class Board: MonoBehaviour
{
    private readonly Cell[,] grid = new Cell[9,9];
    private readonly List<Cell> errorFields = new List<Cell>();
    private readonly List<Cell> editableFields = new List<Cell>();

    private int numHoles = 0;

    private readonly int[] numbers = Enumerable.Range(1, 9).ToArray();
    private readonly System.Random rand = new System.Random();

    public void Start()
    {
        // Generate empty board
        for (int y = 0; y < 9; y++)
        {
            for (int x = 0; x < 9; x++)
            {
                // Each item will contain one grid Cell
                grid[x, y] = ScriptableObject.CreateInstance<Cell>();
                grid[x, y].init(x, y, this);
            }
        }

        Generate(
            Game.IsDifficultyEasy()
                ? 20
                : 40
        );
    }

    // Clear all shown errors
    private void ClearErrors()
    {
        if (Game.IsDifficultyEasy())
        {
            foreach (Cell errorField in errorFields)
            {
                errorField.SetError(false);
            }

            errorFields.Clear();
        }
        
    }

    // Color the areas with errors
    private void SetErrors(int x, int y, int w, int h)
    {
        for (int x1 = x; x1 < x + w; x1++)
        {
            for (int y1 = y; y1 < y + h; y1++)
            {
                grid[x1, y1].SetError(true);
                errorFields.Add(grid[x1, y1]);
            }
        }
    }

    // Check if there are duplicates vertically
    private bool CheckDuplicatesVertical(int x, int y)
    {
        HashSet<int> used = GetUsedVertical(x, y);
        bool isDuplicate = used.Contains(grid[x, y].GetValue());

        if (isDuplicate && Game.IsDifficultyEasy())
        {
            SetErrors(x, 0, 1, 9);
        }

        return isDuplicate;
    }

    // Check if there are duplicates horizontally
    private bool CheckDuplicatesHorizontal(int x, int y)
    {
        HashSet<int> used = GetUsedHorizontal(x, y);
        bool isDuplicate = used.Contains(grid[x, y].GetValue());

        if (isDuplicate && Game.IsDifficultyEasy())
        {
            SetErrors(0, y, 9, 1);
        }

        return isDuplicate;
    }

    // Check if there are duplicates in the 3x3 area
    private bool CheckDuplicatesArea(int x, int y)
    {
        HashSet<int> used = GetUsedArea(x, y);
        bool isDuplicate = used.Contains(grid[x, y].GetValue());

        if (isDuplicate && Game.IsDifficultyEasy())
        {
            SetErrors((x / 3) * 3, (y / 3) * 3, 3, 3);
        }

        return isDuplicate;
    }

    // Check if there are duplicates on the board
    public bool CheckDuplicates()
    {
        ClearErrors();
        bool hasErrors = false;
        int entered = 0;

        foreach(Cell cell in editableFields)
        {
            Vector2Int coords = cell.GetCoords();
            if (cell.GetValue() > 0)

            {
                entered++;
            }

            bool vertical = CheckDuplicatesVertical(coords.x, coords.y);
            bool horizontal = CheckDuplicatesHorizontal(coords.x, coords.y);
            bool area = CheckDuplicatesArea(coords.x, coords.y);

            hasErrors = hasErrors || vertical || horizontal || area;
        }

        if (entered == numHoles && !hasErrors)
        {
            // Won the game
            Character.GetInstance().TriggerWin();
            foreach(Cell cell in grid)
            {
                cell.SetCompleted();
            }
        }

        return hasErrors;
    }

    // Get used numbers in the area
    private HashSet<int> GetUsed(int x, int y, int bx, int by, int w, int h)
    {
        HashSet<int> used = new HashSet<int>();

        for (int x1 = bx; x1 < bx + w; x1++)
        {
            for (int y1 = by; y1 < by + h; y1++)
            {
                if (x == x1 && y == y1)
                {
                    continue;
                }

                int value = grid[x1, y1].GetValue();
                if (value > 0)
                {
                    used.Add(value);
                }
            }
        }
        return used;
    }

    // Get used numbers vertically
    private HashSet<int> GetUsedVertical(int x, int y)
    {
        return GetUsed(x, y, x, 0, 1, 9);
    }

    // Get used numbers horizontally
    private HashSet<int> GetUsedHorizontal(int x, int y)
    {
        return GetUsed(x, y, 0, y, 9, 1);
    }

    // Get used numbers in the 3x3 area
    private HashSet<int> GetUsedArea(int x, int y)
    {
        HashSet<int> used = new HashSet<int>();

        int bx = (x / 3) * 3;
        int by = (y / 3) * 3;

        return GetUsed(x, y, bx, by, 3, 3);
    }

    // Get all used numbers for the given coordinates
    private HashSet<int> GetAllUsed(int x, int y)
    {
        HashSet<int> used = new HashSet<int>();

        used.UnionWith(GetUsedVertical(x, y));
        used.UnionWith(GetUsedHorizontal(x, y));
        used.UnionWith(GetUsedArea(x, y));

        return used;
    }

    // Get the available numbers for the given coordinate
    private int[] GetAvailable(int x, int y)
    {
        HashSet<int> used = GetAllUsed(x, y);
        HashSet<int> available = new HashSet<int>(numbers);

        available.ExceptWith(used);

        return available.OrderBy(number => rand.Next()).ToArray();
    }

    // Fill diagonal areas
    private void FillDiagonal()
    {
        for (int i = 0; i < 3; i++)
        {
            int[] randomNumbers = numbers.OrderBy(number => rand.Next()).ToArray();

            for (int j = 0; j < 9; j++)
            {
                int x = (i * 3) + j % 3;
                int y = (i * 3) + j / 3;
                grid[x, y].SetValue(randomNumbers[j]);
            }

        }
    }

    // Fill the empty board cells
    private Boolean FillEmpty(int x, int y)
    {
        // We hit the right side, go to the next row
        if (x == 9)
        {
            x = 0;
            y++;
        }

        // We filled the entire board
        if (y == 9)
        {
            return true;
        }

        // Skip already filled fields
        if (grid[x, y].GetValue() > 0)
        {
            return FillEmpty(x + 1, y);
        }


        // Get the available numbers for the current field
        int[] available = GetAvailable(x, y);

        // Iterate available numbers and recursively call this method
        foreach(int number in available) {
            grid[x, y].SetValue(number);

            if (FillEmpty(x + 1, y))
            {
                return true;
            }

            // Recursive call didn't find the solution
            // Unset the cell value and try the next number
            grid[x, y].SetValue(0);
        }

        return false;
    }

    private void PunchHoles(int numHoles)
    {
        this.numHoles = numHoles;

        int madeHoles = 0;
        while (numHoles > madeHoles)
        {
            int x = rand.Next(9);
            int y = rand.Next(9);

            if (grid[x, y].GetValue() != 0)
            {
                grid[x, y].SetValue(0);
                editableFields.Add(grid[x, y]);
                madeHoles++;
            } 
        }
    }

    private void Generate(int holes)
    {
        // Randomly fill diagonal 3x3 areas that can't interfere with each other
        FillDiagonal();
        // Fill remaining empty spaces
        FillEmpty(0, 0);
        // Remove random numbers
        PunchHoles(holes);
    }
}
