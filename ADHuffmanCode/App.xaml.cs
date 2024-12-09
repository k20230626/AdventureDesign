using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Controls;

namespace ADHuffmanCode;

public partial class App : Application
{
    public App(IServiceProvider serviceProvider)
    {
        InitializeComponent();

        MainPage = serviceProvider.GetService<MainPage>();
    }
}
