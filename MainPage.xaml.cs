

using Microsoft.Maui.Graphics.Text;

namespace SimpleMines
{
    public class Pole : Button
    {
        public static Pole[,] MinovaSachovnice;
        public static int PocetBomb;
        public static int PocetOdkrytychPoli;
        public static int PocetSloupcu = 10;
        public static int PocetRadku = 15;
        public static int PocetZivotu;
        public bool JeBomba;
        public int PocetOkolnichMin=0;
        public bool JeOtoceno = false;
        public bool JeOznaceno = false;
        public static void VytvorMinovouSachovnici()
        {
            PocetBomb = 0;
            MinovaSachovnice = new Pole[PocetRadku, PocetSloupcu];
            PocetOdkrytychPoli = 0;
            PocetZivotu = 3;
            Pole pole;
            Random rnd = new Random();
            for (int i = 0; i < PocetRadku; i++)
            {
                for (int j = 0; j < PocetSloupcu; j++)
                {
                    pole = new Pole
                    {
                        
                        FontSize = 12,
                        WidthRequest = 40,
                        HeightRequest = 40,
                        BackgroundColor = Colors.Yellow,
                        TextColor = Colors.Black
                    };
                    //losovani bomby
                    if (rnd.Next(1, 7) == 1)
                    {
                        pole.JeBomba = true;
                        PocetBomb++;
                    }
                    else
                        pole.JeBomba = false;
                    pole.IsEnabled = true;
                    MinovaSachovnice[i, j] = pole;
                }
            }

            //Vypocte pocet okolnich min
            for (int i = 0; i < PocetRadku; i++)
            {
                for (int j = 0; j < PocetSloupcu; j++)
                {
                    pole = MinovaSachovnice[i, j];
                    //vlevo
                    if ((j > 0) && (MinovaSachovnice[i, j - 1].JeBomba))
                        pole.PocetOkolnichMin++;
                    //vlevonahore
                    if ((i > 0) && (j > 0) && (MinovaSachovnice[i - 1, j - 1].JeBomba))
                        pole.PocetOkolnichMin++;
                    //nahore
                    if ((i > 0) && (MinovaSachovnice[i - 1, j].JeBomba))
                        pole.PocetOkolnichMin++;
                    //vpravonahore
                    if ((i > 0) && (j < Pole.PocetSloupcu - 1) && (MinovaSachovnice[i - 1, j + 1].JeBomba))
                        pole.PocetOkolnichMin++;
                    //vpravo
                    if ((j < Pole.PocetSloupcu - 1) && (MinovaSachovnice[i, j + 1].JeBomba))
                        pole.PocetOkolnichMin++;
                    //Vpravodole
                    if ((i < Pole.PocetRadku - 1) && (j < Pole.PocetSloupcu - 1) && (MinovaSachovnice[i + 1, j + 1].JeBomba))
                        pole.PocetOkolnichMin++;
                    //dole
                    if ((i < Pole.PocetRadku - 1) && (MinovaSachovnice[i + 1, j].JeBomba))
                        pole.PocetOkolnichMin++;
                    //vlevodole
                    if ((i < Pole.PocetRadku - 1) && (j > 0) && (MinovaSachovnice[i + 1, j - 1].JeBomba))
                        pole.PocetOkolnichMin++;
                }
            }

        }
        public static void ZnepristupniVsechnaPole() 
        {
            for (int i = 0; i < Pole.PocetRadku; i++)
                for (int j = 0; j < Pole.PocetSloupcu; j++)
                    MinovaSachovnice[i,j].IsEnabled = false;
        }
    }
    public partial class MainPage : ContentPage
    {
        private bool IsRezimTlacitko;
        public MainPage()
        {
            InitializeComponent();
            ZalozNovouHru();
        }

        public void ZalozNovouHru()
        {            
            HorizontalStackLayout hsl;
            Pole.VytvorMinovouSachovnici();
            rezimTlacitko.BackgroundColor = Colors.LightBlue;
            rezimTlacitko.TextColor = Colors.Black;
            startTlacitko.TextColor = Colors.Black;
            IsRezimTlacitko = false;
            MinovaSachovniceLayout.Children.Clear();
            rezimTlacitko.TextColor = Colors.Black;

            Zivoty.Text = "  Životů: " + Pole.PocetZivotu;
            for (int i = 0; i < Pole.PocetRadku; i++)
            {
                hsl = new HorizontalStackLayout();
                MinovaSachovniceLayout.Children.Add(hsl);
                for (int j = 0; j < Pole.PocetSloupcu; j++)
                {
                    Pole.MinovaSachovnice[i, j].Clicked += OnClickPole;
                    hsl.Children.Add(Pole.MinovaSachovnice[i, j]);
                }
            }
        }

        private void OnClickPole(object sender, EventArgs e)
        {
            Pole p = (Pole)sender;

            if ((IsRezimTlacitko)&&(!p.JeOtoceno))
            {
                if (p.JeOznaceno)
                {
                    p.JeOznaceno = false;
                    p.Text = "";
                    p.TextColor = Colors.Black;
                }
                else
                {
                    p.JeOznaceno = true;
                    p.Text = "?";
                    p.TextColor = Colors.Red;
                }

            }
            else
            {
                p.JeOtoceno = true;
                if (p.JeBomba)
                {
                    p.BackgroundColor = Colors.Red;
                    Pole.PocetZivotu--;
                    if (Pole.PocetZivotu > 0)
                    {
                        Zivoty.Text = "  Životů: " + Pole.PocetZivotu;
                    }
                    else
                    {
                        startTlacitko.TextColor = Colors.Red;
                        startTlacitko.Text = "Ajaj, prohrál jsi. Klikni pro novou hru.";
                        Pole.ZnepristupniVsechnaPole();
                    }
                }
                else
                {
                    p.BackgroundColor = Colors.LightGreen;
                    p.Text = p.PocetOkolnichMin.ToString();
                    p.TextColor = Colors.Black;
                }
                p.IsEnabled = false;
                Pole.PocetOdkrytychPoli++;
                //Hra uspesne dokoncena
                if (Pole.PocetOdkrytychPoli >= Pole.PocetRadku * Pole.PocetSloupcu - Pole.PocetBomb)
                {
                    startTlacitko.Text = "Huráaa, vyhrál jsi. Klikni pro novou hru.";
                    Pole.ZnepristupniVsechnaPole();
                }
            }
        }

        private void OnClickZacatek(object sender, EventArgs e)
        {
            ZalozNovouHru();
        }
        private void OnClickRezim(object sender, EventArgs e)
        {
            if (IsRezimTlacitko)
            {
                rezimTlacitko.BackgroundColor = Colors.LightBlue;
                IsRezimTlacitko = false;
            }
            else
            {
                rezimTlacitko.BackgroundColor = Colors.Red;
                IsRezimTlacitko = true;
            }

        }

    }
}
