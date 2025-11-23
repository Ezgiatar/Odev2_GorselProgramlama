using System;
using Microsoft.Maui.Controls;

namespace Odev2_GorselProgramlama;

public partial class KrediHesaplamaPage : ContentPage
{
    public KrediHesaplamaPage()
    {
        InitializeComponent();

        // Uygulama başladığında Picker'da ilk öğeyi seçili hale getirir
        KrediTuruPicker.SelectedIndex = 0;
    }

    // Vade Slider'ının değeri değiştiğinde etiketini günceller
    private void VadeSlider_ValueChanged(object sender, ValueChangedEventArgs e)
    {
        // Slider değeri her zaman int'e yuvarlanmalıdır
        int vade = (int)Math.Round(e.NewValue);

        // Vade Slider'ın da XAML'de Maximum'u 120 Ay yapmıştık.
        // O yüzden burada da maksimum 120 ay olarak kabul ediyoruz.
        if (VadeSlider.Maximum < vade)
        {
            vade = (int)VadeSlider.Maximum;
        }

        VadeLabel.Text = $"{vade} Ay";
    }

    // Hesaplama butonu tıklandığında çalışacak metot
    private async void HesaplaButton_Clicked(object sender, EventArgs e)
    {
        // --- 1. Giriş Kontrolleri ---

        // Kredi Tutarı Kontrolü
        if (!double.TryParse(TutarEntry.Text, out double tutar) || tutar <= 0)
        {
            await DisplayAlert("Hata", "Lütfen geçerli bir kredi tutarı (TL) girin.", "Tamam");
            return;
        }

        // Faiz Oranı Kontrolü (FaizEntry'den Yıllık Faiz Oranını alıyoruz)
        if (!double.TryParse(FaizEntry.Text, out double yillikFaizOrani) || yillikFaizOrani <= 0)
        {
            await DisplayAlert("Hata", "Lütfen geçerli bir yıllık faiz oranı (%) girin.", "Tamam");
            return;
        }

        // Kredi Türü Kontrolü
        if (KrediTuruPicker.SelectedIndex == -1)
        {
            await DisplayAlert("Hata", "Lütfen bir kredi türü seçin.", "Tamam");
            return;
        }

        // Vade ve Kredi Türü değerlerini alma
        int vade = (int)Math.Round(VadeSlider.Value);
        string krediTuru = KrediTuruPicker.SelectedItem.ToString();

        // --- 2. Faiz ve Vergi Hesaplaması ---

        // Kredi formülü için kullanılan aylık faiz oranını hesaplama
        // Kullanıcı yıllık faizi (örn: 15) girdiği için, önce 100'e bölüp (0.15), sonra 12'ye bölmeliyiz.
        double aylikFaiz = (yillikFaizOrani / 100.0) / 12.0;

        // BSMV ve KKDF oranlarını belirleme (Türkiye'deki mevzuatlara göre)
        double bsmvOran = 0; // Banka ve Sigorta Muameleleri Vergisi Oranı (0.00 - 1.00)
        double kkdfOran = 0; // Kaynak Kullanımını Destekleme Fonu Oranı (0.00 - 1.00)

        switch (krediTuru)
        {
            case "İhtiyaç Kredisi":
                bsmvOran = 0.15; // Güncel %15 olarak aldık
                kkdfOran = 0.15; // Güncel %15 olarak aldık
                break;
            case "Taşıt Kredisi":
                // Taşıt kredisinde BSMV/KKDF oranları vadeye ve tutara göre değişebilir, 
                // ancak bu basit örnek için İhtiyaç Kredisi ile aynı kabul edelim (ya da Konut gibi 0 alabiliriz).
                // Mevzuata göre taşıt kredileri KKDF'den muaftır, ancak BSMV alınır.
                bsmvOran = 0.15;
                kkdfOran = 0.00;
                break;
            case "Konut Kredisi":
                // Konut Kredilerinde BSMV ve KKDF alınmaz.
                bsmvOran = 0.00;
                kkdfOran = 0.00;
                break;
        }

        // Vergi sonrası brüt aylık faiz oranı (Türkiye'deki yasal faiz hesaplama formülüne göre)
        // Aylık faiz oranı = (Basit aylık faiz oranı) * (1 + KKDF oranı) * (1 + BSMV oranı)
        // NOT: Bankalar genellikle bu vergileri faiz oranı üzerinden değil, faiz miktarı üzerinden hesaplar.
        // Ancak taksit formülüne entegre etmek için en yaygın kullanılan basit yaklaşımlardan biri aşağıdaki gibidir:

        // Yasal düzenlemelerden dolayı, aylık faiz oranı brütleştirilir.
        double brutAylikFaiz = aylikFaiz * (1 + kkdfOran) * (1 + bsmvOran);

        // --- 3. Taksit Hesaplama (Anüite Formülü) ---
        // Aylık Taksit = Kredi Tutarı * [ (r * (1 + r)^n) / ((1 + r)^n - 1) ]
        // Burada r = brutAylikFaiz, n = vade

        double ustelIfade = Math.Pow(1 + brutAylikFaiz, vade);

        // (r * (1 + r)^n)
        double pay = brutAylikFaiz * ustelIfade;

        // ((1 + r)^n - 1)
        double payda = ustelIfade - 1;

        double aylikTaksit = tutar * (pay / payda);

        double toplamOdeme = aylikTaksit * vade;
        double toplamFaiz = toplamOdeme - tutar;

        // --- 4. Sonuçları Ekrana Yazdırma ---

        // C2 formatı ile para birimi gösterimi (₺)
        AylikTaksitLabel.Text = $"💸 Aylık Taksit: {aylikTaksit:C2}";
        ToplamOdemeLabel.Text = $"Toplam Ödeme: {toplamOdeme:C2}";
        ToplamFaizLabel.Text = $"Toplam Faiz ve Vergi: {toplamFaiz:C2}";

        // Sonuç alanını görünür yap
        SonucLayout.IsVisible = true;
    }
}