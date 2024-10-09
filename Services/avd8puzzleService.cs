using System.Text;

namespace adventuredesign8puzzle.Services;


public class avd8puzzleService
{
    private int[,] PuzzleContent = new int[3, 3];
    private int _size = 3;
    private int emptyTile = 0;
    public avd8puzzleService()
    {
        
    }

    public void SolvePuzzle()
    {
        
    }
    
    public int[,] ShufflePuzzle()
    {
        var rand = new Random();
        int rows = _size;
        int cols = _size;

        for (int i = rows * cols - 1; i > 0; i--)
        {
            int j = rand.Next(i + 1);

            // 1차원 인덱스를 2차원 인덱스로 변환
            int iRow = i / cols;
            int iCol = i % cols;
            int jRow = j / cols;
            int jCol = j % cols;

            // 요소 교환
            int temp = PuzzleContent[iRow, iCol];
            PuzzleContent[iRow, iCol] = PuzzleContent[jRow, jCol];
            PuzzleContent[jRow, jCol] = temp;
        }

        Debug.WriteLine(this.ToString());
        return PuzzleContent;
    }

    public int[,] SetSize(int size)
    {
        //2차원 배열 생성
        PuzzleContent = new int[size,size];
        int value = 0;

        for(int i = 0; i < size; i++)
        {
            for(int j = 0; j < size; j++)
            {
                if(++value == size * size)
                {
                    emptyTile = value;
                    value = 0;
                }
                PuzzleContent[i, j] = value;
            }
        }

        _size = size;

        return PuzzleContent;
    }
    
    public bool ValidatePuzzleMergeSort(int[] puzzle)
    {
        int inversionCount = 0;
        for (int i = 0; i < puzzle.Length; i++)
        {
            if (puzzle[i] == 0) continue; 

            for (int j = i + 1; j < puzzle.Length; j++)
            {
                if (puzzle[j] != 0 && puzzle[i] > puzzle[j])
                {
                    inversionCount++;
                }
            }
        }
        return inversionCount % 2 == 0;
    }
    
    public bool ValidatePuzzleBruteForce(int[] puzzle)
    {
        int inversionCount = 0;
        for (int i = 0; i < puzzle.Length; i++)
        {
            if (puzzle[i] == 0) continue; 

            for (int j = i + 1; j < puzzle.Length; j++)
            {
                if (puzzle[j] != 0 && puzzle[i] > puzzle[j])
                {
                    inversionCount++;
                }
            }
        }
        return inversionCount % 2 == 0;
    }
    
    public bool IsSolved()
    {
        int i = 1;
        foreach (var item in Dim2ToDim1())
        {
            if (i == item)
                continue;
            if(i == _size * _size && item == 0)
                return true;
            return false;
        }

        return false;
    }
    
    private int[] Dim2ToDim1()
    {
        var flattened = new int[_size * _size];
        Buffer.BlockCopy(PuzzleContent, 0, flattened, 0, _size * _size * sizeof(int));
        return flattened;
    }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        for(int i = 0; i < _size; i++)
        {
            sb.Append("[ ");
            for(int j = 0; j < _size; j++)
            {
                sb.Append($"{PuzzleContent[i, j],2}, ");
            }
            sb.Append("]\n");
        }

        return sb.ToString();
    }
}