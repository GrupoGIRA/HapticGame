using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Media;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO.Ports;
using System.Windows.Threading;
using Tanvas.TanvasTouch.Resources;
using Tanvas.TanvasTouch.WpfUtilities;

namespace WpfApp1
{

    public partial class MainWindow : Window
    {

        DispatcherTimer gameTimer = new DispatcherTimer();

        private SerialPort serialPort;
        double score;
        double sBurger, sBush;
        int gravity = -10; // g=8
        bool gameOver;
        Rect catHitBox;
        Rect grassHitBox;
        Rect burgerHitBox;
        Rect cloudHitBox;
        private object r;
        TSprite SpriteCat;
        TSprite SpriteBurger;
        TSprite SpriteBush;
        TSprite SpriteGrass;
        private bool isGrounded = false;
        // Parametros de configuracion
        bool sound;
        bool showTextures;
        bool showHamburguer;
        int catSize;





        TanvasTouchViewTracker viewTracker;

        TView myView
        {
            get
            {
                return viewTracker.View;
            }
        }

        public MainWindow(bool sound, bool showTextures, bool showHamburguer, int catSize)
        {

            serialPort = new SerialPort("COM4", 9600); // Change baud rate if necessary
            serialPort.DataReceived += SerialPort_DataReceived;


            try
            {
                serialPort.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening COM port: {ex.Message}");
            }

            this.sound = sound;
            this.showTextures = showTextures;
            this.showHamburguer = showHamburguer;
            this.catSize = catSize;
            InitializeComponent();
            try
            {
                Tanvas.TanvasTouch.API.Initialize();
            }
            catch (Exception ex)
            {

            }
            gameTimer.Tick += MainEventTimer;
            gameTimer.Interval = TimeSpan.FromMilliseconds(20);
            StartGame();


        }

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            // This event is called when data is received on the serial port
            // You can read and process the data here

            // Read the data from the serial port
            string data = serialPort.ReadExisting().Trim(); // Trim para eliminar espacios en blanco, retorno de carro, etc.

            // Update the UI with the received data
            Dispatcher.Invoke(() =>
            {
                // Assuming you have a label named txtSerial in your UI


                // Check the received data and perform actions accordingly
                if (data == "1")
                {
                    DownCat();
                }
                else if (data == "0")
                {
                    // Button pressed: 0
                    // Add your logic here
                    JumpCat(false);
                }
                //else
                //{
                // Unrecognized value
                // Handle as needed
                //   txtSerial.Content = "Es imposible";
                // }
            });
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Close the serial port when the window is closing
            if (serialPort.IsOpen)
            {
                serialPort.Close();
            }
        }




        private void MainEventTimer(object sender, EventArgs e)
        {
            // puntaje
            txtScore.Content = " " + score;

            // hitbox del gato para detectar su masa
            catHitBox = new Rect(Canvas.GetLeft(cat), Canvas.GetTop(cat), cat.Width - 20, cat.Height - 20);
            grassHitBox = new Rect(Canvas.GetLeft(pasto), Canvas.GetTop(pasto), pasto.Width, pasto.Height);
            burgerHitBox = new Rect(Canvas.GetLeft(burger), Canvas.GetTop(burger), burger.Width, burger.Height);
            cloudHitBox = new Rect(Canvas.GetLeft(cloud), Canvas.GetTop(cloud), cloud.Width - 20, cloud.Height - 50);

            Canvas.SetTop(cat, Canvas.GetTop(cat) + gravity);

            // limite para que no se deborde por arriba o abajo
            if (Canvas.GetTop(cat) < -30) // limite superior para que el gato no salga a volar 
            {
                EndGame();
            }

            if (catHitBox.IntersectsWith(cloudHitBox)) // Gato caminando en el pasto
            {

                Canvas.SetTop(cat, Canvas.GetTop(cloud) + 200); // +
                gravity = 0;

            }

            if (catHitBox.IntersectsWith(grassHitBox)) // Gato caminando en el pasto
            {
                isGrounded = true;

            }
            else
            {
                isGrounded = false;
            }
            if (showTextures)
            {
                SpriteCat.Y = (float)Canvas.GetTop(cat);
                SpriteCat.X = (float)Canvas.GetLeft(cat);
            }
            foreach (var a in MyCanvas.Children.OfType<Image>())
            {
                if ((string)a.Tag == "pasto")
                {
                    Canvas.SetLeft(a, Canvas.GetLeft(a) - 5);
                    if (showTextures)
                    {
                        SpriteGrass.Y = (float)Canvas.GetTop(a);
                        SpriteGrass.X = (float)Canvas.GetLeft(a);
                    }
                    if (Canvas.GetLeft(a) < -100)
                    {
                        Canvas.SetLeft(a, 0);
                    }
                }

            }
            foreach (var x in MyCanvas.Children.OfType<Image>())
            {
                Random random = new Random();
                int numeroAleatorio = random.Next(50, 201);
                int numeroAleatorio2 = random.Next(250, 600);


                // suma de puntos cada vez que pasa un el bush
                if ((string)x.Tag == "obs1")
                {
                    Canvas.SetLeft(x, Canvas.GetLeft(x) - 5);
                    if (showTextures)
                    {
                        SpriteBush.Y = (float)Canvas.GetTop(x);
                        SpriteBush.X = (float)Canvas.GetLeft(x);
                    }


                    if (Canvas.GetLeft(x) < -100)
                    {
                        Canvas.SetLeft(x, 1300 + numeroAleatorio);

                        sBush += 1;
                        score += 3;

                        txtSBush.Content = sBush;
                    }

                    Rect bushHitbox = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height - 10);

                    if (catHitBox.IntersectsWith(bushHitbox))
                    {

                        PlayMySound("pack://application:,,/sound/bush.wav");
                        EndGame();
                    }
                }

                if ((string)x.Tag == "burger")
                {
                    if (showHamburguer)
                    {
                        Canvas.SetLeft(x, Canvas.GetLeft(x) - 5);
                        if (showTextures)
                        {
                            SpriteBurger.Y = (float)Canvas.GetTop(x);
                            SpriteBurger.X = (float)Canvas.GetLeft(x);
                        }
                        if (catHitBox.IntersectsWith(burgerHitBox))
                        {
                            Canvas.SetLeft(x, 1200);
                            Canvas.SetTop(x, 10 + numeroAleatorio2);

                            sBurger += 1;
                            score += 5;
                            txtSBurger.Content = sBurger;
                            PlayMySound("pack://application:,,/sound/hamsound.wav");
                        }

                        if (Canvas.GetLeft(x) < -100)
                        {
                            Canvas.SetLeft(x, numeroAleatorio);

                        }
                    }
                    else
                    {
                        x.Visibility = Visibility.Hidden;
                    }

                }
            }
            elapsedTime = stopwatch.Elapsed;
            DownCat();
            UpdateElapsedTimeLabel();

        }
        private void UpdateElapsedTimeLabel()
        {
            txtTime.Content = $" {elapsedTime:mm\\:ss}"; // Formato: minutos:segundos
        }

        private void KeyIsDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space) // caida del gato
            {
                PlayMySound("pack://application:,,/sound/subir.wav");
                JumpCat(true);

            }

            if (e.Key == Key.Space && gameOver == true) // caida del gato
            {

                e.Handled = true;
                cat.RenderTransform = new RotateTransform(180, cat.Width / 2, cat.Height / 2);
            }

            if (e.Key == Key.R && gameOver == true)
            {
                f.Visibility = Visibility.Hidden;
                f2.Visibility = Visibility.Hidden;
                cat.RenderTransform = new RotateTransform(-10, cat.Width / 2, cat.Height / 2);
                StartGame();

            }

            if (e.Key == Key.L && gameOver == true)
            {
                f.Visibility = Visibility.Hidden;
                f2.Visibility = Visibility.Hidden;
                Window1 window1 = new Window1();
                window1.Show();
                this.Close();


            }


        }

        private void KeyIsUp(object sender, KeyEventArgs e)
        {

        }

        public void JumpCat(bool isKeyBoard)
        {
            if (gameOver == true)
            {
                cat.RenderTransform = new RotateTransform(-10, cat.Width / 2, cat.Height / 2);
            }
            else
            {
                //Test Comment
                if (!isKeyBoard)
                {
                    Canvas.SetTop(cat, Canvas.GetTop(cat) - 150); // -100
                }
                else
                {
                    Canvas.SetTop(cat, Canvas.GetTop(cat) - 150); //-100
                }
            }

        }

        public void DownCat()
        {
            if (!isGrounded)
            {
                Canvas.SetTop(cat, Canvas.GetTop(cat) + 5); //20
            }
            // cat.RenderTransform = new RotateTransform(180, cat.Width / 2, cat.Height / 2);

        }

        private TimeSpan elapsedTime;
        private Stopwatch stopwatch = new Stopwatch();

        private void StartGame()
        {
            MyCanvas.Focus();
            elapsedTime = TimeSpan.Zero;
            gameTimer.Start();
            Stopwatch.StartNew();
            stopwatch.Restart();
            int temp = 300;

            score = 0;

            gameOver = false;

            Canvas.SetTop(cat, 400); // posicion inicial del gato donde 0 es la parte superior de 


        }
        private Frame mainFrame;
        private void EndGame()
        {
            gameTimer.Stop();
            gameOver = true;
            cat.RenderTransform = new RotateTransform(180, cat.Width / 2, cat.Height / 2);
            f.Content = " Game Over :'c ";
            f.Visibility = Visibility.Visible;
            f2.Content = "Press R to restart o L to Leave ";
            f2.Visibility = Visibility.Visible;

            //MessageBox.Show("Game Over!!! Press R to restart.", "Game Over", MessageBoxButton.OK, MessageBoxImage.Hand);






        }


        private void CloseCustomMessageBox()
        {
            // Este método se llamará cuando se haga clic en el botón "OK" en el cuadro de mensaje personalizado
            // Puedes realizar acciones adicionales aquí si es necesario

            // Cerrar la ventana
            (Application.Current.MainWindow as Window)?.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            viewTracker = new TanvasTouchViewTracker(this);

            if (showTextures)
            {
                if (showHamburguer)
                {

                    var uriHamburger = new Uri("pack://application:,,/image/texture.png");
                    SpriteBurger = PNGToTanvasTouch.CreateSpriteFromPNG(uriHamburger);
                    myView.AddSprite(SpriteBurger); SpriteBurger.Width = (int)burgerDimensions.Width;
                    SpriteBurger.Height = (int)burgerDimensions.Height;
                }
                var uriCat = new Uri("pack://application:,,/image/Cattexture.png");
                var uriGrass = new Uri("pack://application:,,/image/grassTexture.png");
                var uriBush = new Uri("pack://application:,,/image/Bushtexture.png");


                //SpriteBurger.Material.AddTexture(3,texture:so);
                SpriteCat = PNGToTanvasTouch.CreateSpriteFromPNG(uriCat);
                SpriteBush = PNGToTanvasTouch.CreateSpriteFromPNG(uriBush);
                SpriteGrass = PNGToTanvasTouch.CreateSpriteFromPNG(uriGrass);

                //SpriteBurger.Width = 105;
                //SpriteBurger.Height = 53;
                //SpriteGrass.Width = 1554;
                //SpriteGrass.Height = 242;
                //SpriteBush.Width = 100;
                //SpriteBush.Height = 100;
                //SpriteCat.Width = 142;
                //SpriteCat.Height = 136;

                myView.AddSprite(SpriteCat);
                myView.AddSprite(SpriteBush);
                myView.AddSprite(SpriteGrass);


                SpriteGrass.Width = (int)grassDimensions.Width;
                SpriteGrass.Height = (int)grassDimensions.Height;
                SpriteBush.Width = (int)bushDimensions.Width;
                SpriteBush.Height = (int)bushDimensions.Height;
                if (catSize == 1)
                {
                    SpriteCat.Width = (int)(catDimensions.Width * 0.5f);
                    SpriteCat.Height = (int)(catDimensions.Height * 0.5f);

                }
                else if (catSize == 2)
                {
                    SpriteCat.Width = (int)(catDimensions.Width * 2f);
                    SpriteCat.Height = (int)(catDimensions.Height * 2f);
                }
                else
                {
                    SpriteCat.Width = (int)catDimensions.Width;
                    SpriteCat.Height = (int)catDimensions.Height;
                }
                cat.Width = SpriteCat.Width;
                cat.Height = SpriteCat.Height;
            }

            // Agregar el marco a tu ventana principal




        }
        public void PlayMySound(string myUri)
        {
            if (sound)
            {
                var sri = Application.GetResourceStream(new Uri(myUri));

                if (sri != null)
                {
                    using (var s = sri.Stream)
                    {
                        System.Media.SoundPlayer player = new System.Media.SoundPlayer(s);
                        player.Load();
                        player.Play();
                    }
                }
                else
                {
                    EndGame();
                }
            }

        }


        private static ImageDimensions burgerDimensions = new ImageDimensions(105, 53);
        private static ImageDimensions grassDimensions = new ImageDimensions(1554, 242);
        private static ImageDimensions bushDimensions = new ImageDimensions(100, 100);
        public static ImageDimensions catDimensions = new ImageDimensions(140, 140);


        public class ImageDimensions
        {
            public double Width { get; set; }
            public double Height { get; set; }

            public ImageDimensions(double width, double height)
            {
                Width = width;
                Height = height;
            }
        }



    }
}