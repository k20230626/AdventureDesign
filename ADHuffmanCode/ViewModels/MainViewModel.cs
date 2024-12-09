using System.Collections.Generic;
using ADHuffmanCode.Message;
using ADHuffmanCode.Model;
using ADHuffmanCode.Service.API;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Devices;
using Microsoft.Maui.Storage;

namespace ADHuffmanCode.ViewModels;

public partial class MainViewModel : BaseViewModel {
    private const string FileExtension = "adhf";
    private readonly ICompressService _huffmanService;
    private readonly IFileService _fileService;
    [ObservableProperty]
    private string _filePath = string.Empty;
    
    [ObservableProperty]
    private string _inputString = string.Empty;

    [ObservableProperty]
    private string _outputString = string.Empty;

    [ObservableProperty]
    private CompressInfo _compressInfo = new CompressInfo(0,0);
    
    public MainViewModel(ICompressService compressService,IFileService fileService) {
        _huffmanService = compressService;
        _fileService = fileService;

        BindMessaage();
    }

    private void BindMessaage() {
        WeakReferenceMessenger.Default.Register<CompressInfoMessage>(this, (r, m) => {
            CompressInfo = m.Value;
        });
    }

    [RelayCommand]
    async Task ProcessStart() {
        if (FilePath.EndsWith(FileExtension))
            OutputString = _huffmanService.Decompress(FilePath);
        else
            OutputString = _huffmanService.Compress(FilePath);
    }

    [RelayCommand]
    async Task OpenFilePicker() {
        var option = new PickOptions() {
            PickerTitle = "Select a file",
            FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>> {
                { DevicePlatform.WinUI, new[] { ".txt", ".json", ".yaml", ".adhf" } },
                { DevicePlatform.MacCatalyst, new[] { ".txt", ".json", ".yaml", ".adhf" } }
            })
        };

        #if MACCATALYST
        var res = await MacFilePicker.PickAsync(option);
        #else
        var res = await FilePicker.Default.PickAsync(option);
        #endif
        if (res is not null)
            FilePath = res.FullPath;
    }


    partial void OnFilePathChanged(string value) { 
        InputString = _fileService.ReadFile(value);
    }
    
    
}