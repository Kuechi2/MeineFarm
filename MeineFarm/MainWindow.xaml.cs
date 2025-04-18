using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MeineFarm
{
    public partial class MainWindow : Window
    {
        AniImage AniCat;
        List<Animal> animalList = new();
        public MainWindow()
        {
            InitializeComponent();
        }

        private void NewAnimal(object sender, RoutedEventArgs e)
        {
            Dog Tier1 = new("Hasso", 3, Weide, new(0,0));
            animalList.Add(Tier1);

            animalList.Add((Dog)new("Bello", 2, Weide, new(0, 90)));
            animalList.Add((Cat)new("Minka", 5, Weide, new(0, 180)));
            animalList.Add((Cat)new("Mautz", 1, Weide, new(0, 270)));

            TierListe.ItemsSource = animalList;
            TierListe.Items.Refresh();
        }

        private void Gruesse(object sender, RoutedEventArgs e)
        {
            if(TierListe.SelectedIndex > -1) 
            animalList[TierListe.SelectedIndex].Greet(sender, e as MouseEventArgs);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            List<ImageSource> KatzeAnimation = new List<ImageSource>();
            List<String> AniFiles = Directory.GetFiles("C:\\Users\\skuec\\Desktop\\KatzeAnimated", "*.png").ToList();
            foreach(String AniFile in AniFiles)
            KatzeAnimation.Add(new BitmapImage(new(AniFile)));
            AniCat = new(KatzeAnimation);
            Weide.Children.Add(AniCat);
        }

        private void Ani(object sender, RoutedEventArgs e)
        {
            AniCat.NextFrame();
        }
    }
}