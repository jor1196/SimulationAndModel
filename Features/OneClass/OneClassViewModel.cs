using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SimulationAndModel.Features.OneClass.Models;
using System.Collections.ObjectModel;

namespace SimulationAndModel.Features.OneClass;

public partial class OneClassViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<OneClassRecord> _oneClassRecords = [];

    [ObservableProperty]
    private TimeSpan? _initialTime = new(8,0,0);
    
    [ObservableProperty]
    private int? _fromCustomerArrivalTime = 30;
    
    [ObservableProperty]
    private int? _toCustomerArrivalTime;
    
    [ObservableProperty]
    private bool _hasCustomerArrivalRange;
    
    [ObservableProperty]
    private int? _fromEndServiceTime = 40;
    
    [ObservableProperty]
    private int? _toEndServiceTime;
    
    [ObservableProperty]
    private bool _hasEndServiceRange;
    
    [ObservableProperty]
    private TimeSpan? _endTime = new(9,0,0);

    [ObservableProperty]
    private bool _serviceStationState;

    [ObservableProperty]
    private int _customerQueueCount;

    [RelayCommand]
    private async Task Calculate()
    {
        await Task.Delay(1000);
        OneClassRecords = [];
        
        int customerNextArrivalSecond = InitialTime!.Value.Seconds + FromCustomerArrivalTime!.Value;
        int customerEndServiceSecond = customerNextArrivalSecond + FromEndServiceTime!.Value;

        OneClassRecord record = new()
        {
            CurrentTime = InitialTime.Value,
            CustomerNextArrivalTime = new(InitialTime.Value.Hours, InitialTime.Value.Minutes, customerNextArrivalSecond),
            NextEndServiceTime = new(InitialTime.Value.Hours, InitialTime.Value.Minutes, customerEndServiceSecond),
            CustomerServedCount = 0,
            CustomerQueueCount = CustomerQueueCount,
            ServiceStationState = ServiceStationState
        };

        OneClassRecords.Add(record);

        while (record.CurrentTime <= EndTime)
        {
            if (record.CustomerNextArrivalTime.GetValueOrDefault() <= record.NextEndServiceTime.GetValueOrDefault())
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

                record.CustomerNextArrivalTime = new(record.CurrentTime!.Value.Hours, record.CurrentTime!.Value.Minutes, record.CurrentTime!.Value.Seconds + FromCustomerArrivalTime!.Value);
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
                record.NextEndServiceTime = new(record.CurrentTime.Value.Hours, record.CurrentTime.Value.Minutes, record.CurrentTime.Value.Seconds + FromEndServiceTime!.Value);
            }

            OneClassRecords.Add(record);

            await Task.Delay(5);
        }
    }

    [RelayCommand]
    private void ClearRecords()
    {
        OneClassRecords = [];
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
}
