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
        Point NewPosition = new();
        List<Animal> animalList = new();

        Border InsertFrame = new Border();
        public MainWindow()
        {
            InitializeComponent();
            TType.ItemsSource = Animal.GetDerivedClasses();
        }

        private void NewAnimal(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(TAge.Text, out int age))
            {
                MessageBox.Show("Alter muss ein Zahlenwert sein", "Du Spunk!");
                TAge.Focus();
                TAge.SelectAll();
                return;
            }
            if (TType.SelectedValue == null)
            {
                MessageBox.Show("Du must eine Tierart auswählen", "Du Spust!");
                TType.Focus();
                TType.IsDropDownOpen = true;
                return;
            }
            if(!Weide.Children.Contains(InsertFrame))
            {
                MessageBox.Show("Klicke auf die Weide, um dem Tier ein Plätzchen zuzuweisen", "Du Honk!");
                return;
            }
            if(TType.SelectedItem.ToString() == "Cat") animalList.Add((Cat)new(TName.Text, age, Weide, NewPosition));
            if(TType.SelectedItem.ToString() == "Dog") animalList.Add((Dog)new(TName.Text, age, Weide, NewPosition));
            TierListe.ItemsSource = animalList;
            TierListe.Items.Refresh();
            Weide.Children.Remove(InsertFrame);
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
            foreach(Animal animal in animalList)
            animal.UpdatePosition();
        }

        private void Weide_MouseDown(object sender, MouseButtonEventArgs e)
        {
            NewPosition = e.GetPosition(this);
            InsertFrame.BorderBrush = Brushes.Green;
            InsertFrame.BorderThickness = new Thickness(2);
            InsertFrame.Width = 50;
            InsertFrame.Height = 80;
            Weide.Children.Remove(InsertFrame);
            var s = sender as Canvas;
            if (s != null) s.Children.Add(InsertFrame);
            Canvas.SetLeft(InsertFrame, NewPosition.X);
            Canvas.SetTop(InsertFrame, NewPosition.Y);
            if(TierListe.SelectedItem != null)
            {
                Animal animal = animalList[TierListe.SelectedIndex];
                animal.Speed = 10;
                animal.MovePoint = NewPosition;
                TierListe.ItemsSource = null;
                TierListe.ItemsSource = animalList;
                TierListe.UpdateLayout();
            }
        }
    }
}