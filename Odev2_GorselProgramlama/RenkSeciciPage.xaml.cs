using System;

namespace Odev2_GorselProgramlama;

public partial class RenkSeciciPage : ContentPage
{
    private Random random = new Random();

    public RenkSeciciPage()
    {
        InitializeComponent();
        UpdateColor();
    }

    private void Slider_ValueChanged(object sender, ValueChangedEventArgs e)
    {
        UpdateColor();
    }

    private void UpdateColor()
    {
        int red = (int)RedSlider.Value;
        int green = (int)GreenSlider.Value;
        int blue = (int)BlueSlider.Value;

        RedLabel.Text = red.ToString();
        GreenLabel.Text = green.ToString();
        BlueLabel.Text = blue.ToString();

        string hexKodu = $"#{red:X2}{green:X2}{blue:X2}";
        HexLabel.Text = hexKodu;

        Color renk = Color.FromRgb(red, green, blue);
        Container.BackgroundColor = renk;
    }

    private void RastgeleButton_Clicked(object sender, EventArgs e)
    {
        RedSlider.Value = random.Next(256);
        GreenSlider.Value = random.Next(256);
        BlueSlider.Value = random.Next(256);
    }

    private async void KopyalaButton_Clicked(object sender, EventArgs e)
    {
        string renk_kodu = HexLabel.Text;
        await Clipboard.SetTextAsync(renk_kodu);
        await DisplayAlert("Kopyalandý", $"{renk_kodu}", "OK");
    }
}