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
    private FilterOneClass _filter = new()
    {
        InitialTime = new(8, 0, 0),
        CustomerArrivalTime = 30,
        EndServiceTime = 40,
        EndTime = new(9, 0, 0)
    };

    [RelayCommand]
    private void Calculate()
    {
        OneClassRecords = [];

        int customerNextArrivalSecond = Filter.InitialTime!.Value.Seconds + Filter.CustomerArrivalTime!.Value;
        int customerEndServiceSecond = customerNextArrivalSecond + Filter.EndServiceTime!.Value;
        OneClassRecord record = new()
        {
            CurrentTime = Filter.InitialTime.Value,
            CustomerNextArrivalTime = new(Filter.InitialTime.Value.Hours, Filter.InitialTime.Value.Minutes, customerNextArrivalSecond),
            NextEndServiceTime = new(Filter.InitialTime.Value.Hours, Filter.InitialTime.Value.Minutes, customerEndServiceSecond),
            CustomerServedCount = 0,
            CustomerQueueCount = 0,
            ServiceStationState = false
        };

        OneClassRecords.Add(record);

        while (record.CurrentTime <= Filter.EndTime)
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

                record.CustomerNextArrivalTime = new(record.CurrentTime!.Value.Hours, record.CurrentTime!.Value.Minutes, record.CurrentTime!.Value.Seconds + Filter.CustomerArrivalTime!.Value);
            }
            else
            {
                record.CurrentTime = record.NextEndServiceTime;

                if (!record.ServiceStationState)
                {
                    if (record.CustomerQueueCount > 0)
                    {
                        record.CustomerQueueCount--;
                    }

                    record.ServiceStationState = true;
                }
                else
                {
                    record.CustomerQueueCount++;
                }

                record.CustomerServedCount++;
                record.NextEndServiceTime = new(record.CurrentTime.Value.Hours, record.CurrentTime.Value.Minutes, record.CurrentTime.Value.Seconds + Filter.EndServiceTime!.Value);
            }

            OneClassRecords.Add(record);
        }
    }
}
