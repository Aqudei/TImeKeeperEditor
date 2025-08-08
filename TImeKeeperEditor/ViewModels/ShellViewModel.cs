using AutoMapper;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using TimeKeeperEditor.Data;
using TimeKeeperEditor.Models;

namespace TimeKeeperEditor.ViewModels
{
    public class ShellViewModel : BindableBase
    {
        private DateTime? _selectedLogDate;
        private Employee _selectedEmployee;
        public ICollectionView Employees { get; set; }
        public ICollectionView Logs { get; set; }
        private ObservableCollection<Employee?> _employees = new();
        private ObservableCollection<Log> _logs = new();
        private DelegateCommand _gotoPreviousLogCommand;
        private DelegateCommand _gotoNextLogCommand;
        private DelegateCommand _updateCommand;
        private Log? _currentLog = new Log();
        public string DatabaseFile { get => _databaseFile; set => SetProperty(ref _databaseFile, value); }
        public Log? CurrentLog { get => _currentLog; set => SetProperty(ref _currentLog, value); }

        public Employee? SelectedEmployee { get => _selectedEmployee; set => SetProperty(ref _selectedEmployee, value); }
        public DateTime? SelectedLogDate { get => _selectedLogDate; set => SetProperty(ref _selectedLogDate, value); }
        private readonly Database _database;
        private readonly IMapper _mapper;

        public ShellViewModel(Database database, IMapper mapper)
        {
            Employees = CollectionViewSource.GetDefaultView(_employees);
            Logs = CollectionViewSource.GetDefaultView(_logs);

            _database = database;
            _mapper = mapper;

            // Start async load on construction (non-blocking)
            _ = LoadEmployeesAsync();

            PropertyChanged += ShellViewModel_PropertyChanged;

            DatabaseFile = Properties.Settings.Default.Database;
        }

        private DelegateCommand _selectDatabaseCommand;

        public DelegateCommand SelectDatabaseCommand
        {
            get { return _selectDatabaseCommand ??= new DelegateCommand(HandleSelectDatabase); }
        }

        private void HandleSelectDatabase()
        {
            var dialog = new OpenFileDialog();
            var result = dialog.ShowDialog();
            if (result.HasValue && result.Value)
            {
                DatabaseFile = dialog.FileName;
                Properties.Settings.Default.Database = DatabaseFile;
                Properties.Settings.Default.Save();
            }
        }

        public DelegateCommand GotoPreviousLogCommand =>
            _gotoPreviousLogCommand ??= new DelegateCommand(HandleGotoToPreviousLog, CanGotoPreviousLog)
                .ObservesProperty(() => CurrentLog);

        public DelegateCommand GotoNextLogCommand =>
            _gotoNextLogCommand ??= new DelegateCommand(HandleGotoToNextLog, CanGotoNextLog)
                .ObservesProperty(() => CurrentLog);

        private void HandleGotoToPreviousLog()
        {
            if (_logs.Count == 0 || CurrentLog == null)
                return;

            int currentIndex = _logs.IndexOf(CurrentLog);
            if (currentIndex > 0)
            {
                CurrentLog = _logs[currentIndex - 1];
            }
        }

        private void HandleGotoToNextLog()
        {
            if (_logs.Count == 0 || CurrentLog == null)
                return;

            int currentIndex = _logs.IndexOf(CurrentLog);
            if (currentIndex < _logs.Count - 1)
            {
                CurrentLog = _logs[currentIndex + 1];
            }
        }
        private bool CanGotoPreviousLog()
        {
            if (_logs.Count == 0 || CurrentLog == null)
                return false;

            return _logs.IndexOf(CurrentLog) > 0;
        }

        private bool CanGotoNextLog()
        {
            if (_logs.Count == 0 || CurrentLog == null)
                return false;

            return _logs.IndexOf(CurrentLog) < _logs.Count - 1;
        }

        public DelegateCommand UpdateCommand { get => _updateCommand ??= new DelegateCommand(HandleUpdate); }

        private void HandleUpdate()
        {
            _database.UpdateLog(CurrentLog);
        }

        private void ShellViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SelectedEmployee) || e.PropertyName == nameof(SelectedLogDate))
            {
                if (!SelectedLogDate.HasValue && SelectedEmployee == null)
                    return;

                _ = LoadLogsAsync(SelectedLogDate, SelectedEmployee?.PersonId);
            }
        }

        private async Task LoadLogsAsync(DateTime? logDate, string? personId)
        {
            // Remember current key before reload
            string? currentRefNo = CurrentLog?.LogRefNo;

            var logs = await Task.Run(() => _database.FindLogs(logDate, personId));

            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                _logs.Clear();
                _logs.AddRange(logs);

                if (_logs.Count > 0)
                {
                    // Try to find previous CurrentLog by primary key
                    var match = !string.IsNullOrEmpty(currentRefNo)
                        ? _logs.FirstOrDefault(l => l.LogRefNo == currentRefNo)
                        : null;

                    // If found, preserve selection, otherwise default to first record
                    CurrentLog = match ?? _logs[0];
                }
                else
                {
                    CurrentLog = null;
                }

                // Update navigation buttons
                GotoPreviousLogCommand.RaiseCanExecuteChanged();
                GotoNextLogCommand.RaiseCanExecuteChanged();
            });
        }

        private DelegateCommand _clearEmployeeCommand;
        private string _databaseFile;

        public DelegateCommand ClearEmployeeCommand
        {
            get { return _clearEmployeeCommand ??= new DelegateCommand(HandleClearEmployee); }
        }

        private void HandleClearEmployee()
        {
            SelectedEmployee = null;
            _logs.Clear();

            _mapper.Map(new Log(), CurrentLog);
        }

        private async Task LoadEmployeesAsync()
        {
            var employees = await Task.Run(_database.ListEmployees);

            // Make sure to update ObservableCollection on the UI thread
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                _employees.Clear();
                _employees.AddRange(employees);
            });
        }
    }
}
