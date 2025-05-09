using CommunityToolkit.Maui.Converters;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SimulationAndModel.Common.Extensions;
using SimulationAndModel.Features.FourExercise.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimulationAndModel.Features.FourExercise;

public partial class FourExerciseViewModel : ObservableObject
{
    private readonly Random _random = new();
    private readonly IsListNotNullOrEmptyConverter _isListNotNullOrEmptyConverter = new();

    [ObservableProperty]
    private ObservableCollection<FourExerciseRecord>? _fourExerciseRecords;

    [ObservableProperty]
    private FourExerciseRecord? _lasterRecord;

    [ObservableProperty]
    private TimeSpan _initialTime;

    [ObservableProperty]
    private TimeSpan _endTime;

    [ObservableProperty]
    private int? _fromCustomerArrivalTime;

    [ObservableProperty]
    private int? _fromImportantCustomerArrivalTime;

    [ObservableProperty]
    private int? _fromEndServiceTime;

    [ObservableProperty]
    private int? _customerQueueCount = null;

    [ObservableProperty]
    private int? _importantCustomerQueueCount = null;

    [ObservableProperty]
    private bool _serviceStationState = false;

    [RelayCommand(FlowExceptionsToTaskScheduler = true)]
    private async Task Calculate(CancellationToken cancellationToken)
    {
        await Shell.Current.DisplayAlert("Advertencia", "Aquellos filtros que no estan configurados se les agregara un valor aleatorio", "Ok");

        FourExerciseRecords = [];

        FromCustomerArrivalTime ??= _random.Next(0, 60);
        FromImportantCustomerArrivalTime ??= _random.Next(0, 60);
        FromEndServiceTime ??= _random.Next(0, 60);
        FromEndServiceTime ??= _random.Next(0, 60);
        CustomerQueueCount ??= _random.Next(0, 20);
        ImportantCustomerQueueCount ??= _random.Next(0, 20);

        if (InitialTime == default && EndTime == default)
        {
            InitialTime = TimeSpanExtensions.GeneratorRandomTimeSpan(8);

            while (InitialTime >= EndTime)
                EndTime = TimeSpanExtensions.GeneratorRandomTimeSpan(8);
        }

        var customerNextArrivalTime = InitialTime.SumSeconds(FromCustomerArrivalTime!.Value);
        var importantCustomerNextArrivalTime = InitialTime.SumSeconds(FromImportantCustomerArrivalTime!.Value);
        var nextEndServiceTime = importantCustomerNextArrivalTime.SumSeconds(FromEndServiceTime!.Value);

        FourExerciseRecord record = new()
        {
            CurrentTime = InitialTime,
            CustomerNextArrivalTime = customerNextArrivalTime,
            ImportantCustomerNextArrivalTime = importantCustomerNextArrivalTime,
            NextEndServiceTime = nextEndServiceTime,
            CustomerQueueCount = CustomerQueueCount.Value,
            ServiceStationState = ServiceStationState
        };

        FourExerciseRecords.Add(record);

        while (record.CurrentTime <= EndTime)
        {
            if (record.ImportantCustomerNextArrivalTime < record.CustomerNextArrivalTime)
            {
                record.CurrentTime = record.ImportantCustomerNextArrivalTime;
                if (!record.ServiceStationState)
                {
                    if (record.ImportantCustomerQueueCount > 0)
                    {
                        record.ImportantCustomerQueueCount--;
                        record.ServiceStationState = true;
                    }
                    else if (record.CustomerQueueCount > 0)
                    {
                        record.CustomerQueueCount--;
                        record.ServiceStationState = false;
                    }
                }
                else
                    record.ImportantCustomerQueueCount++;

                record.ImportantCustomerNextArrivalTime = record.CurrentTime.SumSeconds(FromImportantCustomerArrivalTime.Value);
            }
            else if (record.CustomerNextArrivalTime < record.NextEndServiceTime)
            {
                record.CurrentTime = record.CustomerNextArrivalTime;

                if (!record.ServiceStationState)
                {
                    if (record.ImportantCustomerQueueCount > 0)
                    {
                        record.ImportantCustomerQueueCount--;
                        record.ServiceStationState = true;
                    }
                    else if (record.CustomerQueueCount > 0)
                    {
                        record.CustomerQueueCount--;
                        record.ServiceStationState = false;
                    }
                    else
                        record.CustomerQueueCount++;
                }
                else
                    record.CustomerQueueCount++;

                record.CustomerNextArrivalTime = record.CurrentTime.SumSeconds(FromCustomerArrivalTime!.Value);
            }
            else
            {
                record.CurrentTime = record.NextEndServiceTime;
                if (record.ImportantCustomerQueueCount > 0)
                {
                    record.ImportantCustomerQueueCount--;
                    record.ServiceStationState = true;
                    record.ImportantCustomerServedCount++;
                }
                else if (record.CustomerQueueCount > 0)
                {
                    record.CustomerQueueCount--;
                    record.ServiceStationState = true;
                    record.CustomerServedCount++;
                }
                else
                    record.ServiceStationState = false;

                record.NextEndServiceTime = record.CurrentTime.SumSeconds(FromEndServiceTime.Value);
            }

            FourExerciseRecords.Add(record);

            await Task.Delay(5, cancellationToken);
        }

        LasterRecord = record;
    }

    [RelayCommand]
    private void ClearRecords()
    {
        FourExerciseRecords = null;
        LasterRecord = null;
        InitialTime = default;
        EndTime = default;
        FromCustomerArrivalTime = null;
        FromImportantCustomerArrivalTime = null;
        FromEndServiceTime = null;
        CustomerQueueCount = null;
        ImportantCustomerQueueCount = null;
        ServiceStationState = false;
    }

    [RelayCommand]
    private void CancelCalculate()
    {
        CalculateCommand.Cancel();
        LasterRecord = FourExerciseRecords!.Last();
    }

    [RelayCommand]
    private void ServiceStationStateChecking()
    {
        ServiceStationState = !ServiceStationState;
    }
}
