using System.Text;

namespace adventuredesign8puzzle.Services;


public class Avd8PuzzleService: IAvd8puzzleService
{
    private readonly Random _random;
    private int _size;
    private int _gridSize;
    private int[] _puzzle;
    private int _emptyIndex = 0;
    public Avd8PuzzleService() {
        _size = 3;
        _gridSize = _size * _size;
        _puzzle = new int[_gridSize];
        _random = new Random();
    }

    private int _shuffleCount = 0;
    public int[] ShufflePuzzle()
    {
        while (true) {
            
            var n = _gridSize;
            while (n > 1) 
            {
                int k = _random.Next(n--);
                SwapTiles(n,k);
                if (_puzzle[n] == 0)
                    _emptyIndex = n;
                else if (_puzzle[k] == 0)
                    _emptyIndex = k;
            }
            bool isValidate = ValidateInversionCountAsMergeSort();

    #if DEBUG
            bool debugValidate = ValidateInversionCountAsBruteForce();
            if (isValidate != debugValidate)
            {
                throw new Exception("Validate Error");
            }
    #endif
            if (isValidate)
                break; 
            _shuffleCount++;
        }
        Debug.WriteLine($"Shuffle Count : {_shuffleCount}");
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
            SwapTiles(index,left);
            return left;
        }
        else if (right != -1 && _puzzle[right] == 0) {
            // 오른쪽으로 이동
            SwapTiles(index,right);
            return right;
        }
        else if (down != -1 && _puzzle[down] == 0) {
            // 아래로 이동
            SwapTiles(index,down);
            return down;
        }
        else if (up != -1 && _puzzle[up] == 0) {
            // 위로 이동
            SwapTiles(index,up);
            return up;
        }

        //이동 안할시 - 1 반환 
        return -1;
    }

    public int[] GetPuzzle() {
        return _puzzle;
    }

    public bool ValidateInversionCountAsBruteForce() {
        int inversionCount = 0;
        int n = _puzzle.Length;
        for (int i = 0; i < n - 1; i++)
            for (int j = i + 1; j < n; j++)
                if (_puzzle[i] > 0 && _puzzle[j] > 0 && _puzzle[i] > _puzzle[j])
                    inversionCount++;
        

        return IsSolvable(inversionCount);
    }

    public bool ValidateInversionCountAsMergeSort() {
        var cpPuzzle = new int[_gridSize];
        //int[] puzzle을 넣게 되면 퍼즐이 정렬됨;;
        Array.Copy(_puzzle,cpPuzzle,_gridSize);

        var zeroRemovedPuzzle = cpPuzzle.Where(x => x != 0).ToArray();

        int inversionCount = MergeSort(zeroRemovedPuzzle, new int[_gridSize - 1], 0, _gridSize - 2);
        Debug.WriteLine($"MergeSort Inversion Count : {inversionCount}");
        return IsSolvable(inversionCount);
    }

    private int MergeSort(int[] arr, int[] temp, int left, int right){
        int mid, inversionCount = 0;
        if (left < right)
        {
            mid = (left + right) / 2;

            // 왼쪽 절반에서의 inversion 개수
            inversionCount += MergeSort(arr, temp, left, mid);

            // 오른쪽 절반에서의 inversion 개수
            inversionCount += MergeSort(arr, temp, mid + 1, right);

            // 병합 시 발생하는 inversion 개수
            inversionCount += Merge(arr, temp, left, mid, right);
        }
        return inversionCount;


        return 0;
    }

    private int Merge(int[] arr, int[] temp, int left, int mid, int right) {
        int i = left;    // 왼쪽 배열의 시작 인덱스
        int j = mid + 1; // 오른쪽 배열의 시작 인덱스
        int k = left;    // 임시 배열의 시작 인덱스
        int inversionCount = 0;

        while (i <= mid && j <= right)
        {
            if (arr[i] <= arr[j])
            {
                temp[k++] = arr[i++];
            }
            else
            {
                temp[k++] = arr[j++];
                inversionCount += (mid + 1) - i;  // 왼쪽 배열에서 남은 요소의 수만큼 inversion 발생
            }
        }

        // 남은 왼쪽 배열 복사
        while (i <= mid)
            temp[k++] = arr[i++];

        // 남은 오른쪽 배열 복사
        while (j <= right)
            temp[k++] = arr[j++];

        // 원본 배열로 복사
        for (i = left; i <= right; i++)
            arr[i] = temp[i];

        return inversionCount;
    }
    
    private bool IsSolvable(int inversionCount) {
        if (_gridSize % 2 == 1) {
            Debug.WriteLine($"isSolvelable : {inversionCount % 2 == 0}");
            return inversionCount % 2 == 0;
        }
        return (_size - _emptyIndex / _size) % 2 == 1 ? inversionCount % 2 == 0 : inversionCount % 2 == 1;
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
    
    private void SwapTiles(int fromIndex, int toIndex) {
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
    /// BruteForce로 inversion count를 계산하여 퍼즐이 풀 수 있는지 확인
    /// </summary>
    /// <returns></returns>
    public bool ValidateInversionCountAsBruteForce();
    
    /// <summary>
    /// MergeSort로 inversion count를 계산하여 퍼즐이 풀 수 있는지 확인
    /// </summary>
    /// <returns></returns>
    public bool ValidateInversionCountAsMergeSort();
}