namespace ADHuffmanCode.Tests.Utils;

public class GenerateDummyData {
    private readonly int _length;
    private readonly List<char> _asciiRange;

    // 생성자: 데이터 길이와 사용할 아스키 코드 범위 설정
    public GenerateDummyData(int length, IEnumerable<char> asciiRange = null)
    {
        _length = length;
        _asciiRange = asciiRange?.ToList() ?? Enumerable.Range(32, 95).Select(i => (char)i).ToList();
    }

    // 더미 데이터 생성
    public string CreateDummyData()
    {
        var random = new Random();
        var dummyData = new char[_length];

        for (int i = 0; i < _length; i++)
        {
            dummyData[i] = _asciiRange[random.Next(_asciiRange.Count)];
        }

        return new string(dummyData);
    }

}