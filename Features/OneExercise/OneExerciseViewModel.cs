using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SimulationAndModel.Features.OneExercise.Models;
using System.Collections.ObjectModel;

namespace SimulationAndModel.Features.OneExercise;

public partial class OneExerciseViewModel : ObservableObject
{
    private readonly Random _random = new();

    [ObservableProperty]
    private ObservableCollection<OneExerciseRecord>? _oneExerciseRecords;

    [ObservableProperty]
    private TimeSpan _initialTime;
    
    [ObservableProperty]
    private int? _fromCustomerArrivalTime;
    
    [ObservableProperty]
    private int? _toCustomerArrivalTime;
    
    [ObservableProperty]
    private bool _hasCustomerArrivalRange;
    
    [ObservableProperty]
    private int? _fromEndServiceTime;
    
    [ObservableProperty]
    private int? _toEndServiceTime;
    
    [ObservableProperty]
    private bool _hasEndServiceRange;
    
    [ObservableProperty]
    private TimeSpan _endTime;

    [ObservableProperty]
    private bool _serviceStationState;

    [ObservableProperty]
    private int? _customerQueueCount = null;


    [ObservableProperty]
    private OneExerciseRecord? _lasterRecord;

    [RelayCommand(FlowExceptionsToTaskScheduler = true)]
    private async Task Calculate(CancellationToken cancellationToken)
    {
        await Shell.Current.DisplayAlert("Advertencia", "Aquellos filtros que no estan configurados se les agregara un valor aleatorio", "Ok");
        
        OneExerciseRecords = [];
        LasterRecord = null;

        FromCustomerArrivalTime ??= _random.Next(0, 60);

        if (HasCustomerArrivalRange && ToCustomerArrivalTime == null)
        {
            do
            {
                ToCustomerArrivalTime = _random.Next(0, 60);
            }
            while (FromCustomerArrivalTime <= ToCustomerArrivalTime);
        }

        FromEndServiceTime ??= _random.Next(0, 60);

        if (HasEndServiceRange && ToEndServiceTime == null)
        {
            do
            {
                ToEndServiceTime = _random.Next(0, 60);
            }
            while (FromEndServiceTime <= ToEndServiceTime);
        }

        if (InitialTime == default && EndTime == default)
        {
            InitialTime = GeneratorRandomTimeSpan(8);

            while (InitialTime >= EndTime)
                EndTime = GeneratorRandomTimeSpan(8);
        }

        CustomerQueueCount ??= _random.Next(0, 20);

        var customerNextArrivalSecond = CalculateCustomerNextArrivalTime(InitialTime).Seconds;
        var endNextServiceSecond = customerNextArrivalSecond + CalculateEndNextServiceTime(InitialTime).Seconds;

        OneExerciseRecord record = new()
        {
            CurrentTime = InitialTime,
            CustomerNextArrivalTime = new(InitialTime.Hours, InitialTime.Minutes, customerNextArrivalSecond),
            NextEndServiceTime = new(InitialTime.Hours, InitialTime.Minutes, endNextServiceSecond),
            CustomerServedCount = 0,
            CustomerQueueCount = CustomerQueueCount.Value,
            ServiceStationState = ServiceStationState
        };

        OneExerciseRecords.Add(record);

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

                record.CustomerNextArrivalTime = CalculateCustomerNextArrivalTime(record.CurrentTime);
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

                record.NextEndServiceTime = CalculateEndNextServiceTime(record.CurrentTime);
            }

            OneExerciseRecords.Add(record);

            await Task.Delay(5, cancellationToken);
        }

        SetLastRecord();
    }

    [RelayCommand]
    private void ClearRecords()
    {
        OneExerciseRecords = null;
        InitialTime = default;
        FromCustomerArrivalTime = null;
        ToCustomerArrivalTime = null;
        FromEndServiceTime = null;
        ToCustomerArrivalTime = null;
        EndTime = default;
        ServiceStationState = false;
        HasCustomerArrivalRange = false;
        HasEndServiceRange = false;
        CustomerQueueCount = null;
        LasterRecord = null;
    }

    [RelayCommand]
    private void HasEndServiceRangeChecking()
    {
        HasEndServiceRange = !HasEndServiceRange;
    }

    [RelayCommand]
    private void HasCustomerArrivalRangeChecking()
    {
        HasCustomerArrivalRange = !HasCustomerArrivalRange;
    }

    [RelayCommand]
    private void ServiceStationStateChecking()
    {
        ServiceStationState = !ServiceStationState;
    }

    [RelayCommand]
    private void CancelCalculate()
    {
        CalculateCommand.Cancel();
        SetLastRecord();
    }

    private TimeSpan CalculateCustomerNextArrivalTime(TimeSpan currentTime)
    {
        int customerNextSecond;
        if (HasCustomerArrivalRange)
            customerNextSecond = new Random().Next(FromCustomerArrivalTime!.Value, ToCustomerArrivalTime!.Value);
        else
            customerNextSecond = FromCustomerArrivalTime!.Value;

        return new(currentTime.Hours, currentTime.Minutes, currentTime.Seconds + customerNextSecond);
    }

    private TimeSpan CalculateEndNextServiceTime(TimeSpan currentTime)
    {
        int customerNextSecond;
        if (HasEndServiceRange)
            customerNextSecond = new Random().Next(FromEndServiceTime!.Value, ToEndServiceTime!.Value);
        else
            customerNextSecond = FromEndServiceTime!.Value;

        return new(currentTime.Hours, currentTime.Minutes, currentTime.Seconds + customerNextSecond);
    }

    private TimeSpan GeneratorRandomTimeSpan(int maxHours)
    {
        long randomTicks = (long)(_random.NextDouble() * TimeSpan.FromHours(maxHours).Ticks);

        return new TimeSpan(randomTicks);
    }

    private void SetLastRecord()
    {
        LasterRecord = OneExerciseRecords?.Last();
    }
}
