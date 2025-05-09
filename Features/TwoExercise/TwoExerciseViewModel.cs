using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SimulationAndModel.Common.Extensions;
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
    private TimeSpan? _serverAbandonmentTime;

    [ObservableProperty]
    private TimeSpan? _serverReturnTime;

    [ObservableProperty]
    private int? _fromCustomerArrivalTime;

    [ObservableProperty]
    private int? _fromEndServiceTime;

    [ObservableProperty]
    private int? _fromServerAbandonmentTime;

    [ObservableProperty]
    private int? _fromServerReturnTime;

    [ObservableProperty]
    private int? _customerQueueCount = null;

    [ObservableProperty]
    private bool _serviceStationState;

    [ObservableProperty]
    private bool _serverState;

    [RelayCommand(FlowExceptionsToTaskScheduler = true)]
    private async Task Calculate(CancellationToken cancellationToken)
    {
        await Shell.Current.DisplayAlert("Advertencia", "Aquellos filtros que no estan configurados se les agregara un valor aleatorio", "Ok");

        TwoExerciseRecords = [];

        FromCustomerArrivalTime ??= _random.Next(0, 60);
        FromEndServiceTime ??= _random.Next(0, 60);
        //entre 0 y 5 minutos
        FromServerAbandonmentTime ??= _random.Next(0, 300);
        FromServerReturnTime ??= _random.Next(0, 60);
        FromEndServiceTime ??= _random.Next(0, 60);
        CustomerQueueCount ??= _random.Next(0, 20);

        if (InitialTime == default && EndTime == default)
        {
            InitialTime = TimeSpanExtensions.GeneratorRandomTimeSpan(8);

            while (InitialTime >= EndTime)
                EndTime = TimeSpanExtensions.GeneratorRandomTimeSpan(8);
        }

        var customerNextArrivalTime = InitialTime.SumSeconds(FromCustomerArrivalTime!.Value);
        var nextEndServiceTime = customerNextArrivalTime.SumSeconds(FromEndServiceTime!.Value);

        if (ServerState)
            ServerAbandonmentTime = nextEndServiceTime.SumSeconds(FromServerReturnTime.Value);
        else
        {
            ServerReturnTime = customerNextArrivalTime.SumSeconds(FromServerAbandonmentTime!.Value);
            nextEndServiceTime = ServerReturnTime.Value.SumSeconds(1);
        }

        TwoExerciseRecord record = new()
        {
            CurrentTime = InitialTime,
            CustomerNextArrivalTime = customerNextArrivalTime,
            NextEndServiceTime = nextEndServiceTime,
            ServerAbandonmentTime = ServerAbandonmentTime ?? null,
            ServerReturnTime = ServerReturnTime ?? null,
            CustomerServedCount = 0,
            CustomerQueueCount = CustomerQueueCount.Value,
            ServiceStationState = ServiceStationState,
            ServerState = ServerState
        };

        TwoExerciseRecords.Add(record);
        
        while (record.CurrentTime <= EndTime)
        {
            if (record.CustomerNextArrivalTime <= record.NextEndServiceTime)
            {
                record.CurrentTime = record.CustomerNextArrivalTime;

                if (!record.ServiceStationState)
                {
                    if (record.CustomerQueueCount > 0)
                        record.CustomerQueueCount--;

                    record.ServiceStationState = true;
                }
                else
                    record.CustomerQueueCount++;

                record.CustomerNextArrivalTime = record.CurrentTime.SumSeconds(FromCustomerArrivalTime!.Value);
            }
            else if(record.ServerAbandonmentTime == null ||
                    record.NextEndServiceTime < record.ServerAbandonmentTime)
            {
                if (record.ServerReturnTime != null &&
                    record.ServerReturnTime < record.NextEndServiceTime)
                {
                    record.CurrentTime = record.ServerReturnTime.Value;
                    record.ServerState = true;
                    record.ServerAbandonmentTime = record.ServerReturnTime.Value.SumSeconds(FromServerReturnTime.Value);
                    record.ServerReturnTime = null;
                }
                else
                {
                    record.CurrentTime = record.NextEndServiceTime;

                    if (record.CustomerQueueCount > 0)
                    {
                        record.CustomerQueueCount--;
                        record.ServiceStationState = true;
                        record.CustomerServedCount++;
                    }
                    else
                        record.ServiceStationState = false;
                    
                    record.NextEndServiceTime = record.CurrentTime.SumSeconds(FromEndServiceTime!.Value);
                }
            }
            else
            {
                record.CurrentTime = record.ServerAbandonmentTime.Value;
                record.ServerState = false;
                record.ServerReturnTime = record.ServerAbandonmentTime.Value.SumSeconds(FromServerAbandonmentTime.Value);
                
                if (record.ServiceStationState)
                    record.NextEndServiceTime = record.ServerReturnTime.Value.SumSeconds(1);

                record.ServerAbandonmentTime = null;
            }

            TwoExerciseRecords.Add(record);

            await Task.Delay(5, cancellationToken);
        }

        LasterRecord = record;
    }

    [RelayCommand]
    private void ClearRecords()
    {
        TwoExerciseRecords = null;
        LasterRecord = null;
        InitialTime = default;
        EndTime = default;
        ServerAbandonmentTime = default;
        ServerReturnTime = default;
        FromCustomerArrivalTime = null;
        FromEndServiceTime = null;
        FromServerAbandonmentTime = null;
        FromServerReturnTime = null;
        CustomerQueueCount = null;
        ServiceStationState = false;
        ServerState = false;
    }

    [RelayCommand]
    private void CancelCalculate()
    {
        CalculateCommand.Cancel();
        LasterRecord = TwoExerciseRecords!.Last();
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
}
