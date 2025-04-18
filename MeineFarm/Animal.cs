using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
/*
if (weide == null ) 
{
    weide = Application.Current.MainWindow.FindName("Weide") as Canvas;
}
if(weide != null )
{

}*/
namespace MeineFarm
{
    internal class Animal
    {
        private class Speaking : Canvas
        {
            Image Bubble = new();
            Label Text = new();
            DispatcherTimer dispatcherTimer = new DispatcherTimer();
            Animal Outer;
            public Speaking(Animal outer)
            {
                Outer = outer;
                Background = Brushes.LightBlue;
                Bubble.Source = new BitmapImage(new("file://C:/Users/skuec/source/repos/MeineFarm/MeineFarm/Media/Sprechblase.png"));
                Text.Content = Outer.Name;
                Text.FontSize = 18;
                Text.Foreground = Brushes.White;
                Children.Add(Bubble);
                Children.Add(Text);
                SetLeft(Text, 10);
                SetTop(Text, 3);
                Outer.Weide.Children.Add(this);
                Canvas.SetLeft(this, outer.Position.X+25);
                Canvas.SetTop(this, outer.Position.Y-15);
                dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
                dispatcherTimer.Interval = TimeSpan.FromSeconds(2);
                dispatcherTimer.Start();
            }
            private void dispatcherTimer_Tick(object? sender, EventArgs e)
            {
                Outer.Weide.Children.Remove(this);
                dispatcherTimer.Stop();
            }

        }
        public string Name {get; set; }
        public int Age {get; set; }
        internal string SoundFile;
        internal Image AnimalImage = new();
        internal Point Position = new(0,0);
        internal Canvas Weide;
        public Animal(string name, int age, string soundfile, string graphfile, Canvas weide, Point position)
        {
            Name = name;
            Age = age;
            SoundFile = soundfile;
            Position = position;
            AnimalImage.Source = new BitmapImage(new(graphfile));
            Weide = weide;
            Weide.Children.Add(AnimalImage);
            Canvas.SetLeft(AnimalImage, Position.X);
            Canvas.SetTop(AnimalImage, Position.Y);
            AnimalImage.MouseEnter += Greet;
        }

        private void PlaceAt(Point? position = null)
        {
            if (position != null) Position = (Point)position;
            Canvas.SetLeft(AnimalImage, Position.X);
            Canvas.SetTop(AnimalImage, Position.Y);
        }

        public void Greet(object sender, MouseEventArgs e)
        {
            MediaPlayer AnimalSound = new();
            AnimalSound.Open(new(SoundFile));
            AnimalSound.Play();
            Speaking s = new(this);
            MessageBox.Show(sender.ToString());
        }
    }
    class Dog : Animal
    {
        public Dog(string name, int age, Canvas weide, Point position) : base(name, age, 
            "file://C:/Users/skuec/source/repos/MeineFarm/MeineFarm/Media/wauwau.mp3",
            "file://C:/Users/skuec/source/repos/MeineFarm/MeineFarm/Media/Hund.png", weide, position) 
        { 
            
        }
    }
    class Cat : Animal
    {
        public Cat(string name, int age, Canvas weide, Point position) : base(name, age,
    "file://C:/Users/skuec/source/repos/MeineFarm/MeineFarm/Media/meow.mp3",
    "file://C:/Users/skuec/source/repos/MeineFarm/MeineFarm/Media/Katze.png", weide, position)
        {

        }
    }
    internal class AniImage : Image
    {
        List<ImageSource> Sources = new();
        int currentFrame =0;
        public AniImage(List<ImageSource> sources)
        {
            Sources = sources;
        }
        public void NextFrame()
        {
            currentFrame++;
            if (currentFrame > Sources.Count-1) { currentFrame=0; }
            Source = Sources[currentFrame];
        }
    }
}
