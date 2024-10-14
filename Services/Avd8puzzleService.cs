using System.Text;

namespace adventuredesign8puzzle.Services;


public class Avd8puzzleService: IAvd8puzzleService
{
    private Random _random;
    private int _size;
    private int _gridSize;
    private int[] _puzzle;
    private int _emptyIndex = 0;
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
            //여기서 puzzle값이 0일 경우 _emptyIndex를 k로 설정

            Swaptiles(n,k);
            if (_puzzle[n] == 0)
                _emptyIndex = n;
            else if (_puzzle[k] == 0)
                _emptyIndex = k;
        }
        return _puzzle;
    }

    public int[] GenNPuzzle(int size)
    {
        _size = size;
        _gridSize = size * size;
        _puzzle = new int[_gridSize];
        for (int i = 1; i < _gridSize + 1; i++) {
            if(i == _gridSize)
                _puzzle[i - 1] = 0;
            else
                _puzzle[i - 1] = i;
        }
        return _puzzle;
    }
    
    //TODO: 로직 다시만들기
    public bool IsSolved() {
        int i;
        for (i = 0; i < _gridSize - 1; i++) {
            if (_puzzle[i] != i + 1)
                return false;
        }
        if(i == _gridSize - 1 && _puzzle[i] == 0)
            return true;
        return false;
    }


    public int MovePuzzleTile(int index) {
        if (_puzzle[index] == 0)
            return -1;
        int up = -1, down = -1, left = -1,right = -1;
        // [ 0, 1, 2 ]
        // [ 3, 4, 5 ]
        // [ 6, 7, 8 ]
        // 왼족 인덱스가 존재하는지는 현재 인덱스가 _size로 나누고 나머지 값이 0이면 된다.
        // 오른쪽 인덱스가 존재하는지는 현재 인덱스가 _size로 나누고 나머지 값이 _size - 1이면 된다.
        left =  (index % _size == 0) ? -1 : index - 1 ;
        right = (index % _size ==  _size - 1) ? -1 :index + 1;
        down = (index + _size >= _gridSize) ?  -1 : index + _size;
        up = (index - _size < 0) ? -1 : index - _size;
        
        if (left != -1 && _puzzle[left] == 0) {
            // 왼쪽으로 이동
            Swaptiles(index,left);
            return left;
        }
        else if (right != -1 && _puzzle[right] == 0) {
            // 오른쪽으로 이동
            Swaptiles(index,right);
            return right;
        }
        else if (down != -1 && _puzzle[down] == 0) {
            // 아래로 이동
            Swaptiles(index,down);
            return down;
        }
        else if (up != -1 && _puzzle[up] == 0) {
            Swaptiles(index,up);
            return up;
        }

        return -1;
    }

    public int[] GetPuzzle() {
        return _puzzle;
    }

    public int GetEmtpyIndex() {
        return _emptyIndex;
    }

    public bool ValidateInversionCountAsBruteForce(int[] puzzle) {
        int inversionCount = 0;

        foreach (var item in _puzzle) {
            for (int i = 0; i < _gridSize; i++) {
                if (item == 0)
                    continue;
                if (item > puzzle[i] && puzzle[i] != 0)
                    inversionCount++;
            }
        }

        return inversionCount % 2 == 0;
    }

    public bool ValidateInversionCountAsMergeSort(int[] puzzle) {
        int inversionCount = MergeSort(ref puzzle, 0, _gridSize - 1);
        
        return inversionCount % 2 == 0;
    }

    private int MergeSort(ref int[] arr, int left, int right) {
        if (left < right) {
            int mid = (left + right) / 2;
            MergeSort(ref arr, left, mid);
            MergeSort(ref arr, mid + 1, right);
            Merge(ref arr, left, mid, right);
        }

        return 0;
    }

    private void Merge(ref int[] arr,int left,int mid,int right) {
        
    }
    
    private bool IsSolvable(int inversionCount) {
        if (_size % 2 == 1)
            return inversionCount % 2 == 0;
        else {
            return (_emptyIndex / _size) % 2 == 1 ? inversionCount % 2 == 0 : inversionCount % 2 == 1;
        }
        return false;
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
    
    private void Swaptiles(int fromIndex, int toIndex) {
        (_puzzle[toIndex], _puzzle[fromIndex]) = (_puzzle[fromIndex], _puzzle[toIndex]);
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
    /// <returns>이전 `0`의 의치</returns>
    public int MovePuzzleTile(int index);

    /// <summary>
    /// 현재 퍼즐을 가져옴
    /// </summary>
    /// <returns></returns>
    public int[] GetPuzzle();
    
    /// <summary>
    /// 현재 퍼즐에서 마지막 타일을 가져옴<br/>
    ///<br/>
    /// 의미가 좀 애매한데<br/>
    /// 처음 퍼즐 상태에서 마지막 인덱스 값을 가져옴<br/>
    /// 3 * 3 이라면 8을반환
    /// </summary>
    /// <returns>마지막 인덱스를가져옴</returns>
    public int GetEmtpyIndex();
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="puzzle"></param>
    /// <returns></returns>
    public bool ValidateInversionCountAsBruteForce(int[] puzzle);
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="puzzle"></param>
    /// <returns></returns>
    public bool ValidateInversionCountAsMergeSort(int[] puzzle);
}