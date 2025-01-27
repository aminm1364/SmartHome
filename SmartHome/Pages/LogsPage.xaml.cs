using SmartHome.Interfaces;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Shapes;
using SmartHome.Db;
using SmartHome.Models;
using System.Collections.ObjectModel;

namespace SmartHome.Pages;

public class Monkey
{
    public string Name { get; set; }
    public string Location { get; set; }
    public string Details { get; set; }
}
public partial class LogsPage : ContentPage
{
    SmartRelayDatabase _database;
    //private IEnumerable<Log> Logs { get; set; }
    //public IEnumerable<Monkey> Monkeys { get; set; }
    public ObservableCollection<Log> Logs { get; set; } = new ObservableCollection<Log>();
    public LogsPage(SmartRelayDatabase database)
	{
        this._database = database;
        InitializeComponent();
        logListview.ItemsSource = Logs;
        BindingContext = this;
        LoadLogs();
        //ListView listView = new ListView();
        //listView.SetBinding(ItemsView.ItemsSourceProperty, "Logs");
    }

    private async void LoadLogs()
    {
        var logs = await _database.GetLogsAsync();
        //Logs = new ObservableCollection<Log>(logs);
        foreach (var log in logs)
        {
            Logs.Add(log);
        }
        //for (int i = 1; i < 4; i++)
        //{
        //    Monkeys.Add(new Monkey() { Name = $"Name{i}", Details = $"Details{i}" });
        //}


        //MainThread.BeginInvokeOnMainThread(async () =>
        //{
        //    var logs = await _database.GetLogsAsync();
        //    Logs = logs.ToList();
        //});
    }

    void OnLogSelected(object sender, SelectedItemChangedEventArgs args)
    {
        Log selectedLog = args.SelectedItem as Log;
    }

    private async void deleteLog_button_Clicked(object sender, EventArgs e)
    {
        if (sender is ImageButton button && button.CommandParameter is int id)
        {
            var logItem = Logs.FirstOrDefault(item => item.Id == id);
            if (logItem != null)
            {
                await _database.DeleteLogAsync(logItem);
                Logs.Remove(logItem);
            }
        }
    }
}