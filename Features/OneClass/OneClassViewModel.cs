using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SimulationAndModel.Features.BasicExample.Models;
using System.Collections.ObjectModel;

namespace SimulationAndModel.Features.OneClassPage;

public partial class OneClassViewModel : ObservableObject
{
    private ObservableCollection<RecordOneClass> _recordOneClass = [];

    [RelayCommand]
    private void Calculate()
    {
    }
}
