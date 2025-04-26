using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace MeineFarm
{
    internal class Animal
    {
        internal class AniImage : Image
        {
            List<ImageSource> Sources = new();
            int currentFrame = 0;
            public AniImage(string path)
            {
                path = AppContext.BaseDirectory + "\\Media\\" + path ;
                string[] files = System.IO.Directory.GetFiles(path, "*.png");
                foreach(string file in files)
                {
                    Image frame = new();
                    frame.Source = new BitmapImage(new Uri(file));
                    Sources.Add(frame.Source);
                }
            }
            internal ImageSource NextFrame()
            {
                currentFrame++;
                if (currentFrame > Sources.Count - 1) { currentFrame = 0; }
                return Sources[currentFrame];
            }
        }
        private class AnimalData
        {
#pragma warning disable CS8618 
            public string Name { get; set; }
            public double Speed { get; set; }
            public int Age { get; set; }
            public string SoundFile { get; set; }
            public Point Position { get; set; }
            public string Type { get; set; } // "Dog", "Cat", etc.
#pragma warning restore CS8618 
        }

        private class AnimalConverter : JsonConverter<Animal>
        {
            public override Animal? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                var animalData = JsonSerializer.Deserialize<AnimalData>(ref reader, options);
                if (animalData == null) return null;
                Canvas? Weide = Application.Current.MainWindow.FindName("Weide") as Canvas;
                if(Weide == null)
                {
                    throw new NotSupportedException("No Canvas Weide in MainWindow");
                }
                Animal? animal = null;
                if (animalData.Type == "Dog") animal = new Dog(animalData.Name, animalData.Age, Weide, animalData.Position);
                if (animalData.Type == "Cat") animal = new Cat(animalData.Name, animalData.Age, Weide, animalData.Position);
                if(animal==null) throw new NotSupportedException("Unknown Animal Type in SaveFile"); ;

                animal.Speed = animalData.Speed;
                animal.SoundFile = animalData.SoundFile;
                return animal;
            }

            public override void Write(Utf8JsonWriter writer, Animal animal, JsonSerializerOptions options)
            {
                var animalData = new AnimalData
                {
                    Name = animal.Name,
                    Speed = animal.Speed,
                    Age = animal.Age,
                    SoundFile = animal.SoundFile,
                    Position = animal.Position,
                    Type = animal.GetType().Name
                };

                JsonSerializer.Serialize(writer, animalData, options);
            }
        }

        private class Speaking : Canvas
        {
            Image Bubble = new();
            Label Text = new();
            DispatcherTimer BubbleTimeoutTimer = new DispatcherTimer();
            Animal Outer;
            internal Speaking(Animal outer)
            {
                Outer = outer;
                Background = Brushes.LightBlue;
                Bubble.Source = new BitmapImage(outer.GetLoadUri("Sprechblase.png"));
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
            // Interne Datenklasse für Serialisierung


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
        internal Point Position { get; set; } = new(0, 0);
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
                if (!AniTimer.IsEnabled) AniTimer.Start();
            }
        }
        internal Canvas Weide;
        private Vector moveDirection = new();
        private Point movePoint = new();
        private DispatcherTimer AniTimer = new();
        internal bool selected = false;
        internal bool Selected 
        { 
            get 
            { 
                return selected; 
            } 
            set 
            { 
                selected = value; 
                ShowSelectMarker(); 
            } 
        }
        private Ellipse SelectIdentifier = new(); 
        public event EventHandler? SelectedChanged;
        internal AniImage Animation;


        public Animal(string name, int age, string soundfile, string graphfile, Canvas weide, Point position)
        {
            Name = name;
            Age = age;
            SoundFile = soundfile;
            Position = position;

            Animation = new(graphfile);
            AnimalImage.Source = Animation.NextFrame();
            AnimalImage.Width = 80;
            AnimalImage.Height = 120;
            //AnimalImage.Source = new BitmapImage(GetLoadUri(graphfile));
            if (Weide == null) Weide = weide;
            Weide.Children.Add(AnimalImage);
            Canvas.SetLeft(AnimalImage, Position.X);
            Canvas.SetTop(AnimalImage, Position.Y);
            AnimalImage.MouseEnter += Greet;
            AnimalImage.MouseDown += Klicked;
            AniTimer.Interval = TimeSpan.FromMilliseconds(15);
            AniTimer.Tick += AnimationStep;

            SelectIdentifier.Stroke = Brushes.Red;
            SelectIdentifier.StrokeThickness = 1;
            SelectIdentifier.Width = 80;
            SelectIdentifier.Height = 60;

        }
        //Save and Load
        public static void SaveFarm(List<Animal> FarmListe)
        {

            var options = new JsonSerializerOptions
            {
                Converters = { new Animal.AnimalConverter() }, // Zugriff auf den inneren Converter
                WriteIndented = true
            };
            string json = JsonSerializer.Serialize(FarmListe, options);

            File.WriteAllText("test.json", json);
        }
        public static List<Animal> LoadFarm()
        {
            var options = new JsonSerializerOptions
            {
                Converters = { new Animal.AnimalConverter() }, // Zugriff auf den inneren Converter
                WriteIndented = true
            };
            // Deserialisieren
            string json = File.ReadAllText("test.json");
            List<Animal>? ReturnList = new List<Animal>();
            ReturnList = JsonSerializer.Deserialize<List<Animal>>(json, options);
            if(ReturnList == null) return new();    //Wenn Serializenicht 
            return ReturnList;
        }
        private Uri GetLoadUri(string FileName)
        {
            string basePath = AppContext.BaseDirectory;
            return new Uri($"file://{basePath}/Media/{FileName}"); //INTODABOOK
        }
        public void RemoveFromCanvas()
        {
            Weide.Children.Remove(AnimalImage);
            if (Selected) Weide.Children.Remove(SelectIdentifier);
        }
        private void AnimationStep(object? sender, EventArgs e)
        {
            //AnimalImage.Source = Animation.NextFrame();
            Position += moveDirection;
            AnimalImage.Source = Animation.NextFrame();
            if (Math.Abs(Position.X - MovePoint.X) < Speed &&
                Math.Abs(Position.Y - MovePoint.Y) < Speed)
            {
                AniTimer.Stop();
                AnimalImage.RenderTransform = null;
                Position = MovePoint;
                moveDirection = new(0, 0);
            }
            
            Canvas.SetLeft(AnimalImage, Position.X);
            Canvas.SetTop(AnimalImage, Position.Y);
            if (Selected)
            {
                Canvas.SetLeft(SelectIdentifier, Position.X);
                Canvas.SetTop(SelectIdentifier, Position.Y+30);
            }
        }
        private Vector GetDirection(Point position)
        {
            Vector direction = new((position.X-Position.X), (position.Y-Position.Y));
            direction.Normalize();
            direction *= Speed;
            if (direction.X < 0)
                AnimalImage.RenderTransform = new ScaleTransform(-1, 1, 40, 30);
            else
                AnimalImage.RenderTransform = null;
            return direction;
        }
        public void Greet(object sender, RoutedEventArgs e)
        {
            MediaPlayer AnimalSound = new();
            AnimalSound.Open(GetLoadUri(SoundFile));    //ADDTOBOOK
            AnimalSound.Play();
            Speaking s = new(this);
        }        
        public void Klicked(object sender, MouseEventArgs e)
        {
            Selected = !Selected;
            if(SelectedChanged != null)
                SelectedChanged.Invoke(this, e);
        }

        private void ShowSelectMarker()
        {
            if (Selected)
            {
                Weide.Children.Add(SelectIdentifier);
                Canvas.SetLeft(SelectIdentifier, Position.X);
                Canvas.SetTop(SelectIdentifier, Position.Y + 30);
            }
            else
            {
                Weide.Children.Remove(SelectIdentifier);
            }
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
            "wauwau.mp3", "HundAnimated", weide, position) 
        { 
            
        }
    }
    class Cat : Animal
    {
        public Cat(string name, int age, Canvas weide, Point position) : base(name, age,
            "meow.mp3", "KatzeAnimated", weide, position)
        {

        }
    }
}
