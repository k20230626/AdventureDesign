using System.Text;

namespace adventuredesign8puzzle.Services;


//TODO: 0번을 마지막에 둬야하는데 이를 array 0번에 둘지 아니면 array의 마지막 숫자를 0번이라고 할지 고민임
public class Avd8puzzleService: IAvd8puzzleService
{
    private Random _random;
    private int _size;
    private int _gridSize;
    private int[] _puzzle;
    public Avd8puzzleService() {
        _size = 3;
        _gridSize = _size * _size;
        _puzzle = new int[_gridSize];
        _random = new Random();
    }
    
    public int[] ShufflePuzzle()
    {
        int n = _gridSize;
        while (n > 1) 
        {
            int k = _random.Next(n--);
            var temp = _puzzle[n];
            _puzzle[n] = _puzzle[k];
            _puzzle[k] = temp;
        }
        return _puzzle;
    }

    public int[] GenNPuzzle(int size)
    {
        _size = size;
        _gridSize = size * size;
        _puzzle = new int[_gridSize];
        for (int i = 0; i < _gridSize; i++) {
            _puzzle[i] = i;
        }
        return _puzzle;
    }
    
    //TODO: 로직 다시만들기
    public bool IsSolved() {
        int i = 1;
        foreach (var item in _puzzle)
        {
            if (i == item)
                continue;
            if(i == _gridSize&& item == 0)
                return true;
            return false;
        }

        return false;
    }


    public int[] MovePuzzleTile(int index) {
        if (_puzzle[index] == 0)
            return _puzzle;
        int up,down,left,right = -1;
        // [ 0, 1, 2 ]
        // [ 3, 4, 5 ]
        // [ 6, 7, 8 ]
        // 왼족 인덱스가 존재하는지는 현재 인덱스가 _size로 나누고 나머지 값이 0이면 된다.
        // 오른쪽 인덱스가 존재하는지는 현재 인덱스가 _size로 나누고 나머지 값이 _size - 1이면 된다.
        left =  index % _size == 0 ? -1 : index - 1 ;
        right = index % _size ==  _size - 1 ? -1 :index + 1;
        down = index + _size > _gridSize -1 ?  -1 : index + _size;
        up = index - _size < 0 ? -1 : index - _size;
        
        if (left != -1 && _puzzle[left] == 0) {
            // 왼쪽으로 이동
            var temp = _puzzle[left];
            _puzzle[left] = _puzzle[index];
            _puzzle[index] = temp;
        }
        else if (right != -1 && _puzzle[right] == 0) {
            // 오른쪽으로 이동
            var temp = _puzzle[right];
            _puzzle[right] = _puzzle[index];
            _puzzle[index] = temp;
        }
        else if (down != -1 && _puzzle[down] == 0) {
            // 아래로 이동
            var temp = _puzzle[down];
            _puzzle[down] = _puzzle[index];
            _puzzle[index] = temp;
        }
        else if (up != -1 && _puzzle[up] == 0) {
            // 위로 이동
            var temp = _puzzle[up];
            _puzzle[up] = _puzzle[index];
            _puzzle[index] = temp;
        }

        return _puzzle;
    }

    public int[] GetPuzzle() {
        return _puzzle;
    }
    public override string ToString() {
        StringBuilder sb = new StringBuilder();
        for(int i = 0; i < _size; i++)
        {
            sb.Append("[ ");
            for(int j = 0; j < _size; j++)
            {
                sb.Append($"{_puzzle[i*_size + j],2}, ");
            }
            sb.Append("]\n");
        }

        return sb.ToString();
    }
}

public interface IAvd8puzzleService {
    /// <summary>
    /// 현재 퍼즐을 랜덤하게 섞음
    /// </summary>
    /// <returns>shuffle된 puzzle</returns>
    int[] ShufflePuzzle();

    /// <summary>
    /// size * size 만큼의 int array를 생성
    /// </summary>
    /// <param name="size">puzzle의 크기</param>
    /// <returns>순서대로 정렬된 puzzle</returns>
    public int[] GenNPuzzle(int size);

    /// <summary>
    /// 퍼즐이 해결되었는지 확인
    /// </summary>
    /// <returns>퍼즐이 풀렸으면 `true`, 아니면 `false`를반환</returns>
    public bool IsSolved();
    
    /// <summary>
    /// index의 상,하,좌,우에 0이 있을 경우 0과 위치를 바꿈
    /// </summary>
    /// <param name="index">선택된 타일의 위치</param>
    /// <returns>바뀐 puzzle</returns>
    public int[] MovePuzzleTile(int index);
}