using System;

namespace SimulationAndModel;

public partial class MainPage : ContentPage
{
    private int count = 0;
    public MainPage()
    {
        InitializeComponent();        
    }

    private void CounterBtn_Clicked(object sender, EventArgs e)
    {
        count++;
        CounterBtn.Text = $"{count} times";
    } 
}
