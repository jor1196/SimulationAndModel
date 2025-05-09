using CommunityToolkit.Maui.Converters;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SimulationAndModel.Common.Extensions;
using SimulationAndModel.Features.ThreeExercise.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimulationAndModel.Features.ThreeExercise;

public partial class ThreeExerciseViewModel : ObservableObject
{
    private readonly Random _random = new();
    private readonly IsListNotNullOrEmptyConverter _isListNotNullOrEmptyConverter = new();

    [ObservableProperty]
    private ObservableCollection<ThreeExerciseRecord>? _threeExerciseRecords;

    [ObservableProperty]
    private ThreeExerciseRecord? _lasterRecord;

    [ObservableProperty]
    private ObservableCollection<TimeSpan>? _customerLeaveQueueTimes;

    [ObservableProperty]
    private TimeSpan _initialTime;

    [ObservableProperty]
    private TimeSpan _endTime;

    [ObservableProperty]
    private int? _fromCustomerArrivalTime;

    [ObservableProperty]
    private int? _fromEndServiceTime;

    [ObservableProperty]
    private int? _customerWaitTime;

    [ObservableProperty]
    private int? _customerQueueCount = null;

    [ObservableProperty]
    private bool _serviceStationState;

    [RelayCommand(FlowExceptionsToTaskScheduler = true)]
    private async Task Calculate(CancellationToken cancellationToken)
    {
        await Shell.Current.DisplayAlert("Advertencia", "Aquellos filtros que no estan configurados se les agregara un valor aleatorio", "Ok");

        ThreeExerciseRecords = [];
        CustomerLeaveQueueTimes = [];

        FromCustomerArrivalTime ??= _random.Next(0, 60);
        FromEndServiceTime ??= _random.Next(0, 60);
        FromEndServiceTime ??= _random.Next(0, 60);
        CustomerQueueCount ??= _random.Next(0, 20);
        CustomerWaitTime ??= _random.Next(0, 60);

        if (InitialTime == default && EndTime == default)
        {
            InitialTime = TimeSpanExtensions.GeneratorRandomTimeSpan(8);

            while (InitialTime >= EndTime)
                EndTime = TimeSpanExtensions.GeneratorRandomTimeSpan(8);
        }

        var customerNextArrivalTime = InitialTime.SumSeconds(FromCustomerArrivalTime!.Value);
        var nextEndServiceTime = customerNextArrivalTime.SumSeconds(FromEndServiceTime!.Value);

        ThreeExerciseRecord record = new()
        {
            CurrentTime = InitialTime,
            CustomerNextArrivalTime = customerNextArrivalTime,
            NextEndServiceTime = nextEndServiceTime,
            CustomerServedCount = 0,
            CustomerQueueCount = CustomerQueueCount.Value,
        };

        ThreeExerciseRecords.Add(record);

        while (record.CurrentTime <= EndTime)
        {
            if (_isListNotNullOrEmptyConverter.ConvertFrom(record.CustomerQueueTime) &&
                record.CustomerQueueTime[0] < record.CurrentTime)
            {
                CustomerLeaveQueueTimes.Add(record.CustomerQueueTime[0]);
                record.CustomerQueueTime.RemoveAt(0);
            }

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
                {
                    record.CustomerQueueCount++;
                    record.CustomerQueueTime.Add(record.CurrentTime.SumSeconds(CustomerWaitTime.Value));
                }

                record.CustomerNextArrivalTime = record.CurrentTime.SumSeconds(FromCustomerArrivalTime!.Value);
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

                if (_isListNotNullOrEmptyConverter.ConvertFrom(record.CustomerQueueTime))
                    record.CustomerQueueTime.RemoveAt(0);

                record.NextEndServiceTime = record.CurrentTime.SumSeconds(FromEndServiceTime!.Value);
            }

            record.CustomerLeaveQueueTimes = [.. CustomerLeaveQueueTimes];

            ThreeExerciseRecords.Add(record);

            await Task.Delay(5, cancellationToken);
        }

        LasterRecord = record;
    }

    [RelayCommand]
    private void ClearRecords()
    {
        ThreeExerciseRecords = null;
        CustomerLeaveQueueTimes = null;
        CustomerWaitTime = null;
        LasterRecord = null;
        InitialTime = default;
        EndTime = default;
        FromCustomerArrivalTime = null;
        FromEndServiceTime = null;
        CustomerQueueCount = null;
        ServiceStationState = false;
    }

    [RelayCommand]
    private void CancelCalculate()
    {
        CalculateCommand.Cancel();
        LasterRecord = ThreeExerciseRecords!.Last();
    }

    [RelayCommand]
    private void ServiceStationStateChecking()
    {
        ServiceStationState = !ServiceStationState;
    }
}
