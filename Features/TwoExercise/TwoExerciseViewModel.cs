using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SimulationAndModel.Features.TwoExercise.Models;
using System.Collections.ObjectModel;

namespace SimulationAndModel.Features.TwoExercise;

public partial class TwoExerciseViewModel : ObservableObject
{
    private readonly Random _random = new();

    [ObservableProperty]
    private ObservableCollection<TwoExerciseRecord>? _twoExerciseRecords;
    
    [ObservableProperty]
    private TwoExerciseRecord? _lasterRecord;

    [ObservableProperty]
    private TimeSpan _initialTime;

    [ObservableProperty]
    private TimeSpan _endTime;

    [ObservableProperty]
    private TimeSpan _nextEndServiceTime;

    [ObservableProperty]
    private TimeSpan _serverAbandonmentTime;

    [ObservableProperty]
    private int? _fromCustomerArrivalTime;

    [ObservableProperty]
    private int? _fromEndServiceTime;

    [ObservableProperty]
    private int? _customerQueueCount = null;

    [ObservableProperty]
    private bool _serviceStationState;

    [ObservableProperty]
    private bool _serverState;

    [RelayCommand(FlowExceptionsToTaskScheduler = true)]
    private async Task Calculate(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    [RelayCommand]
    private void ClearRecords()
    {
        throw new NotImplementedException();
    }

    [RelayCommand]
    private void CancelCalculate()
    {
        CalculateCommand.Cancel();
        SetLastRecord();
    }

    [RelayCommand]
    private void ServiceStationStateChecking()
    {
        ServiceStationState = !ServiceStationState;
    }

    [RelayCommand]
    private void ServerStateChecking()
    {
        ServerState = !ServerState;
    }

    private void SetLastRecord()
    {
        LasterRecord = TwoExerciseRecords?.Last();
    }
}
