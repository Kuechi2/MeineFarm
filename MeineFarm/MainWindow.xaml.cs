using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace MeineFarm
{
    public partial class MainWindow : Window
    {
        //AniImage AniCat;
        Point NewPosition = new();
        List<Animal> animalList = new();

        Border InsertFrame = new Border();
        public MainWindow()
        {
            InitializeComponent();
            TierListe.SelectionMode = DataGridSelectionMode.Extended;//ADDTOBOOK
            TType.ItemsSource = Animal.GetDerivedClasses();
        }

        private void NewAnimal(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(TAge.Text, out int age))
            {
                MessageBox.Show("Alter muss ein Zahlenwert sein", "Falsche Altersangabe");
                TAge.Focus();
                TAge.SelectAll();
                return;
            }
            if (TType.SelectedValue == null)
            {
                MessageBox.Show("Du must eine Tierart auswählen", "Keine Tierart gewählt");
                TType.Focus();
                TType.IsDropDownOpen = true;
                return;
            }
            if(!Weide.Children.Contains(InsertFrame))
            {
                MessageBox.Show("Klicke auf die Weide, um dem Tier ein Plätzchen zuzuweisen", "Platz auf der Weide anklicken");
                return;
            }
            if (TType.SelectedItem.ToString() == "Cat")
                animalList.Add(new Cat(TName.Text, age, Weide,
                NewPosition));
            if (TType.SelectedItem.ToString() == "Dog")
                animalList.Add(new Dog(TName.Text, age, Weide,
                NewPosition));
            Animal NewAni = animalList[animalList.Count - 1];
            NewAni.Speed = 1;
            NewAni.SelectedChanged += AniSelect;

            TierListe.ItemsSource = animalList;
            TierListe.ItemsSource = animalList;
            TierListe.Items.Refresh();
            Weide.Children.Remove(InsertFrame);
        }

        private void AniSelect(object? sender, EventArgs e)
        {
            if(sender is Animal)
            {
                if (((Animal)sender).Selected)
                    TierListe.SelectedItems.Add(sender);
                else
                    TierListe.SelectedItems.Remove(sender);
            }
        }

        private void Gruesse(object sender, RoutedEventArgs e)
        {
            if(TierListe.SelectedIndex > -1) 
            animalList[TierListe.SelectedIndex].Greet(sender, e);
        }

        private void Load_Click(object sender, RoutedEventArgs e)
        {
            animalList.Clear();
            Weide.Children.Clear();
            animalList = Animal.LoadFarm();
            foreach(var animal in animalList)
            {
                animal.SelectedChanged += AniSelect;
            }
            TierListe.ItemsSource = animalList;
            TierListe.Items.Refresh();
        }
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            Animal.SaveFarm(animalList);

        }
        private void DeleteAnimal(object sender, RoutedEventArgs e)
        {
            List<Animal> StayAlive = [.. animalList];
            foreach(Animal AnimalToDelete in StayAlive) 
            {
                if(AnimalToDelete.Selected)
                {
                    AnimalToDelete.RemoveFromCanvas();
                    animalList.Remove(AnimalToDelete);
                }
            }
            TierListe.Items.Refresh();
        }

        private void Weide_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource != sender) return; //ADDTOBOOK
            NewPosition = e.GetPosition(this);
            NewPosition.X -= 40;
            NewPosition.Y -= 30;
            InsertFrame.BorderBrush = Brushes.Green;
            InsertFrame.BorderThickness = new Thickness(2);
            InsertFrame.Width = 80;
            InsertFrame.Height = 60;
            Weide.Children.Remove(InsertFrame);
            var s = sender as Canvas;
            if (s != null) s.Children.Add(InsertFrame);
            Canvas.SetLeft(InsertFrame, NewPosition.X);
            Canvas.SetTop(InsertFrame, NewPosition.Y);
            NewPosition.Y -= 30;
            if(TierListe.SelectedItem != null)
            {
                foreach(Animal ani in TierListe.SelectedItems)
                {
                    if(ani.Selected) 
                    ani.MovePoint = NewPosition;
                }
            }
        }

        private void TierListe_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach(Animal Animal in animalList) Animal.Selected = false;
            foreach (Animal SelectedAnimal in TierListe.SelectedItems)
            { 
                if (!SelectedAnimal.Selected)
                    SelectedAnimal.Selected = true; 
            }
        }



        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }
    }
}
