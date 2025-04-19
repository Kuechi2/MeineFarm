using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
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
            DispatcherTimer BubbleTimeoutTimer = new DispatcherTimer();
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
                Canvas.SetLeft(this, outer.Position.X + 25);
                Canvas.SetTop(this, outer.Position.Y - 15);
                BubbleTimeoutTimer.Tick += dispatcherTimer_Tick;
                BubbleTimeoutTimer.Interval = TimeSpan.FromSeconds(2);
                BubbleTimeoutTimer.Start();
            }
            private void dispatcherTimer_Tick(object? sender, EventArgs e)
            {
                Outer.Weide.Children.Remove(this);
                BubbleTimeoutTimer.Stop();
            }

        }
        public string Name { get; set; }
        public double Speed { get; set; }
        public int Age { get; set; }
        internal string SoundFile;
        internal Image AnimalImage = new();
        internal Point Position = new(0, 0);
        internal Canvas Weide;
        private Vector moveDirection = new();
        private Point movePoint = new();

        DispatcherTimer AniTimer = new();
        public Point MovePoint 
        { 
            get 
            { 
                return movePoint; 
            } 
            set 
            { 
                moveDirection = GetDirection(value); 
                movePoint = value; 
            } 
        } 
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
            AnimalImage.MouseDown += Klicked;
            AniTimer.Interval = TimeSpan.FromMilliseconds(15);
            AniTimer.Tick += AnimationStep;
        }

        private void AnimationStep(object? sender, EventArgs e)
        {
            Position += moveDirection;
            if (Math.Abs(Position.X - MovePoint.X) < Speed &&
                Math.Abs(Position.Y - MovePoint.Y) < Speed)
            {
                AniTimer.Stop();
                moveDirection = new(0, 0);
            }
            Canvas.SetLeft(AnimalImage, Position.X);
            Canvas.SetTop(AnimalImage, Position.Y);
        }

        private void PlaceAt(Point? position = null)
        {
            if (position != null) Position = (Point)position;
            Canvas.SetLeft(AnimalImage, Position.X);
            Canvas.SetTop(AnimalImage, Position.Y);
        }
        private Vector GetDirection(Point position)
        {
            Vector direction = new Vector((position.X-Position.X), (position.Y-Position.Y));
            direction.Normalize();
            direction *= Speed;
            return direction;
        }
        public void UpdatePosition()
        {
            AniTimer.Start();
        }
        public void Greet(object sender, MouseEventArgs e)
        {
            MediaPlayer AnimalSound = new();
            AnimalSound.Open(new(SoundFile));
            AnimalSound.Play();
            Speaking s = new(this);
        }        
        public void Klicked(object sender, MouseEventArgs e)
        {
            MessageBox.Show("Tier geklickt. Klick es auf die Klicker, damit es jault!!!","HrHrHr!!!");
        }
        public static string[] GetDerivedClasses()
        {
            List<string> children = new List<string>();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (typeof(Animal).IsAssignableFrom(type) && type != typeof(Animal))
                    {
                        children.Add(type.Name);
                    }
                }
            }
            return children.ToArray();
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
