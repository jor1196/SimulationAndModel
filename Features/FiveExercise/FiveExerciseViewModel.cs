using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SimulationAndModel.Common.Extensions;
using SimulationAndModel.Features.FiveExercise.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimulationAndModel.Features.FiveExercise;

public partial class FiveExerciseViewModel : ObservableObject
{
    private readonly Random _random = new();

    [ObservableProperty]
    private ObservableCollection<FiveExerciseRecord>? _fiveExerciseRecords;

    [ObservableProperty]
    private FiveExerciseRecord? _lasterRecord;

    [ObservableProperty]
    private TimeSpan _initialTime;

    [ObservableProperty]
    private TimeSpan _endTime;

    [ObservableProperty]
    private int? _fromCustomerArrivalTime;

    [ObservableProperty]
    private int? _fromEndServiceTime;

    [ObservableProperty]
    private int? _customerQueueCount = null;

    [ObservableProperty]
    private bool _serviceStationState = false;

    [ObservableProperty]
    private bool _secureZoneState = false;

    [RelayCommand(FlowExceptionsToTaskScheduler = true)]
    private async Task Calculate(CancellationToken cancellationToken)
    {
        await Shell.Current.DisplayAlert("Advertencia", "Aquellos filtros que no estan configurados se les agregara un valor aleatorio", "Ok");

        FiveExerciseRecords = [];

        FromCustomerArrivalTime ??= _random.Next(0, 60);
        FromEndServiceTime ??= _random.Next(0, 60);
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

        FiveExerciseRecord record = new()
        {
            CurrentTime = InitialTime,
            CustomerNextArrivalTime = customerNextArrivalTime,
            NextEndServiceTime = nextEndServiceTime,
            CustomerQueueCount = CustomerQueueCount.Value,
            ServiceStationState = ServiceStationState,
            SecureZoneState = SecureZoneState
        };

        FiveExerciseRecords.Add(record);

        while (record.CurrentTime <= EndTime)
        {
            if (record.CustomerNextArrivalTime <= record.NextEndServiceTime)
            {
                record.CurrentTime = record.CustomerNextArrivalTime;

                if (!record.SecureZoneState && !record.ServiceStationState)
                {
                    if (record.CustomerQueueCount > 0)
                    {
                        record.CustomerQueueCount--;

                        record.ServiceStationState = true;
                        record.SecureZoneState = true;
                    }
                }
                else
                    record.CustomerQueueCount++;

                record.CustomerNextArrivalTime = record.CurrentTime.SumSeconds(FromCustomerArrivalTime.Value);
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
                {
                    record.ServiceStationState = false;
                    record.SecureZoneState = false;
                }

                record.NextEndServiceTime = record.CurrentTime.SumSeconds(FromEndServiceTime.Value);
            }

            FiveExerciseRecords.Add(record);

            await Task.Delay(5, cancellationToken);
        }

        LasterRecord = record;
    }

    [RelayCommand]
    private void ClearRecords()
    {
        FiveExerciseRecords = null;
        LasterRecord = null;
        InitialTime = default;
        EndTime = default;
        FromCustomerArrivalTime = null;
        FromEndServiceTime = null;
        CustomerQueueCount = null;
        ServiceStationState = false;
        SecureZoneState = false;
    }

    [RelayCommand]
    private void CancelCalculate()
    {
        CalculateCommand.Cancel();
        LasterRecord = FiveExerciseRecords!.Last();
    }

    [RelayCommand]
    private void ServiceStationStateChecking()
    {
        ServiceStationState = !ServiceStationState;
    }
}
