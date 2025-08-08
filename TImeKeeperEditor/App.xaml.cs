using AutoMapper;
using System.Configuration;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using TimeKeeperEditor.Data;
using TimeKeeperEditor.Models;
using TimeKeeperEditor.Views;
namespace TimeKeeperEditor;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : PrismApplication
{

    protected override Window CreateShell()
    {
        return Container.Resolve<Shell>();
    }

    protected override void RegisterTypes(IContainerRegistry containerRegistry)
    {
        containerRegistry.RegisterSingleton<Database>();
        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<Log, Log>();
        });

        containerRegistry.RegisterInstance(config.CreateMapper());
    }
}

