using Microsoft.Maui.Graphics; // Color sýnýfýný kullanmak için

namespace Odev2_GorselProgramlama;

public partial class VKIPage : ContentPage
{
    public VKIPage()
    {
        InitializeComponent();

        // Baþlangýçta hesaplamayý çalýþtýr
        CalculateBmi();
    }

    // Slider deðerleri deðiþtiðinde tetiklenir
    private void Sliders_ValueChanged(object sender, ValueChangedEventArgs e)
    {
        // Kilo ve Boy etiketlerini güncelle
        int kilo = (int)Math.Round(KiloSlider.Value);
        int boy = (int)Math.Round(BoySlider.Value);

        KiloLabel.Text = $"{kilo} KG";
        BoyLabel.Text = $"{boy} CM";

        // VKÝ'yi yeniden hesapla ve sonucu güncelle
        CalculateBmi();
    }

    private void CalculateBmi()
    {
        // Slider'lardan tam sayý deðerlerini al
        double kilo = KiloSlider.Value;

        // Boyu santimetreden metreye çevir (Örn: 170 CM -> 1.7 M)
        double boyMetre = BoySlider.Value / 100.0;

        // VKÝ Formülü: Kilo (kg) / (Boy * Boy) (m^2)
        double vki = kilo / (boyMetre * boyMetre);

        // Sonucu 2 ondalýk basamakla etikete yaz
        VkiSonucLabel.Text = $"{vki:F2}";

        // Kategori ve renk belirleme
        string kategori;
        Color renk;
        Color arkaPlanRenk;

        // Dünya Saðlýk Örgütü (WHO) VKÝ Sýnýflandýrmasý
        if (vki < 18.5)
        {
            kategori = "Zayýf";
            renk = Colors.DodgerBlue; // Mavi
            arkaPlanRenk = Color.FromArgb("#BBDEFB"); // Açýk mavi arka plan
        }
        else if (vki >= 18.5 && vki <= 24.9)
        {
            kategori = "Normal Kilolu";
            renk = Colors.ForestGreen; // Yeþil
            arkaPlanRenk = Color.FromArgb("#C8E6C9"); // Açýk yeþil arka plan
        }
        else if (vki >= 25.0 && vki <= 29.9)
        {
            kategori = "Fazla Kilolu";
            renk = Colors.Orange; // Turuncu
            arkaPlanRenk = Color.FromArgb("#FFECB3"); // Açýk turuncu arka plan
        }
        else if (vki >= 30.0 && vki <= 39.9)
        {
            kategori = "Obez";
            renk = Colors.Red; // Kýrmýzý
            arkaPlanRenk = Color.FromArgb("#FFCDD2"); // Açýk kýrmýzý arka plan
        }
        else // vki >= 40.0
        {
            kategori = "Ýleri Derecede Obez (Morbid)";
            renk = Colors.DarkRed; // Koyu Kýrmýzý
            arkaPlanRenk = Color.FromArgb("#EF9A9A"); // Orta kýrmýzý arka plan
        }

        // Sonuç etiketlerini ve rengini güncelle
        VkiKategoriLabel.Text = kategori;
        VkiKategoriLabel.TextColor = renk;
        VkiSonucLabel.TextColor = renk;
        SonucFrame.BackgroundColor = arkaPlanRenk;
    }
}