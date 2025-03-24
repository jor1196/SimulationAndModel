using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SimulationAndModel.Features.OneClass.Models;
using System.Collections.ObjectModel;

namespace SimulationAndModel.Features.OneClass;

public partial class OneClassViewModel : ObservableObject
{
    private readonly Random _random = new();

    [ObservableProperty]
    private ObservableCollection<OneClassRecord> _oneClassRecords = [];

    [ObservableProperty]
    private TimeSpan? _initialTime;
    
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
    private TimeSpan? _endTime;

    [ObservableProperty]
    private bool _serviceStationState;

    [ObservableProperty]
    private int _customerQueueCount;

    [RelayCommand]
    private async Task Calculate()
    {
        await Shell.Current.DisplayAlert("Advertencia", "Aquellos filtros que no estan configurados se les agregara un valor aleatorio", "Ok");

        if (HasCustomerArrivalRange && ToCustomerArrivalTime == null)
            ToCustomerArrivalTime ??= _random.Next(0, 60);
        
        FromCustomerArrivalTime ??= _random.Next(0, 60);

        if (HasEndServiceRange && ToEndServiceTime == null)
            ToEndServiceTime ??= _random.Next(0, 60);

        FromEndServiceTime ??= _random.Next(0, 60);

        OneClassRecords = [];

        InitialTime ??= GeneratorRandomTimeSpan(8);
        EndTime ??= InitialTime + GeneratorRandomTimeSpan(8);

        var customerNextArrivalSecond = CalculateCustomerNextArrivalTime(InitialTime.Value).Seconds;
        var endNextServiceSecond = customerNextArrivalSecond + CalculateEndNextServiceTime(InitialTime.Value).Seconds;

        OneClassRecord record = new()
        {
            CurrentTime = InitialTime.Value,
            CustomerNextArrivalTime = new(InitialTime.Value.Hours, InitialTime.Value.Minutes, customerNextArrivalSecond),
            NextEndServiceTime = new(InitialTime.Value.Hours, InitialTime.Value.Minutes, endNextServiceSecond),
            CustomerServedCount = 0,
            CustomerQueueCount = CustomerQueueCount,
            ServiceStationState = ServiceStationState
        };

        OneClassRecords.Add(record);

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
                record.ServiceStationState = false;

                if (record.CustomerQueueCount > 0)
                {
                    record.CustomerQueueCount--;
                    record.ServiceStationState = true;
                }

                record.CustomerServedCount++;
                record.NextEndServiceTime = CalculateEndNextServiceTime(record.CurrentTime);
            }

            OneClassRecords.Add(record);

            await Task.Delay(5);
        }
    }

    [RelayCommand]
    private void ClearRecords()
    {
        OneClassRecords = [];
        InitialTime = null;
        FromCustomerArrivalTime = null;
        ToCustomerArrivalTime = null;
        FromEndServiceTime = null;
        ToCustomerArrivalTime = null;
        EndTime = null;
        ServiceStationState = false;
        HasCustomerArrivalRange = false;
        HasEndServiceRange = false;
        CustomerQueueCount = 0;
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
}
