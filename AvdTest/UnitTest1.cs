using System.Diagnostics;
using AvdTest.Service;
using OfficeOpenXml;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace AvdTest;

public class UnitTest1 {
    private readonly IAvd8puzzleService service;
    private readonly ITestOutputHelper output;

    public UnitTest1(ITestOutputHelper output) {
        service = new Avd8PuzzleService();
        output = new TestOutputHelper();
    }

    [Theory]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(6)]
    [InlineData(8)]
    [InlineData(9)]
    [InlineData(10)]
    [InlineData(25)]
    [InlineData(50)]
    [InlineData(100)] //[InlineData(1000)]
    // [InlineData(10000)]
    public void ShuffleTest(int size) {
        var puzzle = service.GenNPuzzle(size);
        string logFilePath = "test_log.txt";

        Stopwatch stopwatch = new Stopwatch();

        File.AppendAllText(logFilePath, $"{size * size - 1}-Puzzle{Environment.NewLine}");

        stopwatch.Start();
        service.ShufflePuzzle();
        stopwatch.Stop();
        File.AppendAllText(logFilePath, $"{stopwatch.ElapsedTicks}");

        var service1 =　new Avd8PuzzleService();
        service1.CopyArray(service.GetPuzzle());

        stopwatch.Start();
        service.ValidateInversionCountAsBruteForce();
        stopwatch.Stop();
        File.AppendAllText(logFilePath, $"{stopwatch.ElapsedTicks},");

        stopwatch.Start();
        service1.ValidateInversionCountAsMergeSort();
        stopwatch.Stop();
        File.AppendAllText(logFilePath, $"{stopwatch.ElapsedTicks}");
        // Assert: 실행 시간이 1초 미만인지 확인
        Assert.True(stopwatch.ElapsedMilliseconds < 1000, "메서드 실행 시간이 1초를 초과했습니다.");
    }

    [Fact]
    public void ShuffleTestWithExcel() {
        int[] sizes = { 8, 15, 24, 35, 48, 63, 80, 99, 624, 2499, 9999 };
        string excelFilePath = "ExecutionTimes.xlsx";

        // Excel 파일 생성 준비
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        using (var package = new ExcelPackage()) {
            // 시트 생성
            var shuffleSheet = package.Workbook.Worksheets.Add("Shuffle Time");
            var bruteForceSheet = package.Workbook.Worksheets.Add("Brute Force");
            var mergeSortSheet = package.Workbook.Worksheets.Add("Merge Sort");

            // 헤더 작성
            for (int i = 0; i < sizes.Length; i++) {
                shuffleSheet.Cells[1, i + 2].Value = sizes[i];
                bruteForceSheet.Cells[1, i + 2].Value = sizes[i];
                mergeSortSheet.Cells[1, i + 2].Value = sizes[i];
            }

            // 각 시트에 실행 회차 헤더 추가
            for (int run = 1; run <= 10; run++) {
                shuffleSheet.Cells[run + 1, 1].Value = run;
                bruteForceSheet.Cells[run + 1, 1].Value = run;
                mergeSortSheet.Cells[run + 1, 1].Value = run;
            }

            // 각 퍼즐 크기별로 10회씩 실행
            foreach (var size in sizes) {
                var service = new Avd8PuzzleService(); // 퍼즐 생성 및 셔플 서비스
                var puzzle = service.GenNPuzzle(size);
                var service1 = new Avd8PuzzleService();


                for (int run = 0; run < 10; run++) {
                    Stopwatch stopwatch = new Stopwatch();

                    // 1. 셔플 시간 측정
                    stopwatch.Start();
                    service.ShufflePuzzle();
                    stopwatch.Stop();
                    shuffleSheet.Cells[run + 2, Array.IndexOf(sizes, size) + 2].Value =
                        stopwatch.Elapsed.TotalMilliseconds;
                    stopwatch.Reset();

                    service1.CopyArray(service.GetPuzzle());
                    // 2. 브루트 포스 시간 측정
                    stopwatch.Start();
                    service.ValidateInversionCountAsBruteForce();
                    stopwatch.Stop();
                    bruteForceSheet.Cells[run + 2, Array.IndexOf(sizes, size) + 2].Value =
                        stopwatch.Elapsed.TotalMilliseconds;
                    stopwatch.Reset();

                    // 3. 머지 소트 시간 측정
                    stopwatch.Start();
                    service1.ValidateInversionCountAsMergeSort();
                    stopwatch.Stop();
                    mergeSortSheet.Cells[run + 2, Array.IndexOf(sizes, size) + 2].Value =
                        stopwatch.Elapsed.TotalMilliseconds;
                    stopwatch.Reset();
                }
            }

            // Excel 파일 저장
            var fileInfo = new FileInfo(excelFilePath);
            package.SaveAs(fileInfo);
        }

        Console.WriteLine($"Execution times saved to {excelFilePath}");
    }
}