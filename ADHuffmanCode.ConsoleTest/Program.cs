// See https://aka.ms/new-console-template for more information


using System.Diagnostics;
using ADHuffmanCode.Service;

var DummyPath = Path.Combine(Path.GetTempPath(), "dummy.txt");
const int DummyDataLength = 1000000;

var dummyData = new GenerateDummyData(DummyDataLength).CreateDummyData();
new FileService().WriteFile(DummyPath,dummyData);

preTask();
var huffmanCompress = new CompressService(new HuffmanAlgorithmService(), new FileService());
var stopwatch = new Stopwatch();
		
//Huffman Compress
stopwatch.Start();
huffmanCompress.Compress(DummyPath);
stopwatch.Stop();
Console.WriteLine($"Huffman Compress Time: {stopwatch.ElapsedMilliseconds}ms");
stopwatch.Reset();
		
//Huffman Decompress
stopwatch.Start();
huffmanCompress.Decompress(Path.Combine(Path.GetTempPath(), "dummy.adhf"));
stopwatch.Stop();
Console.WriteLine($"Huffman Compress Time: {stopwatch.ElapsedMilliseconds}ms");

//validation





void preTask() {
    var dummyData = new GenerateDummyData(DummyDataLength).CreateDummyData();
    new FileService().WriteFile(DummyPath,dummyData);
}


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