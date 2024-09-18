using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfApp1
{
    /// <summary>
    /// Lógica de interacción para Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            this.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            this.WindowState = WindowState.Maximized;
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // 1. Pequeño
            // 2. Grande
            // cualquier otro numero es mediano
            //MainWindow window = new MainWindow(sound: false, showTextures: true, showHamburguer: true, catSize: 1);
            ConfigWindow window = new ConfigWindow();
            this.Close();
            window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            window.WindowState = WindowState.Maximized;
            window.Show();
        }
    }
}
