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
    /// Lógica de interacción para ConfigWindow.xaml
    /// </summary>
    public partial class ConfigWindow : Window
    {
        int catSize = 3;
        public ConfigWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // 1. Pequeño
            // 2. Grande
            // cualquier otro numero es mediano
            if (radioSmall.IsChecked.Value)
            {
                catSize = 1;
            }else if (radioLarge.IsChecked.Value)
            {
                catSize = 2;
            }else
            {
                catSize = 3;
            }

            MainWindow window = new MainWindow(sound: chksonido.IsChecked.Value, showTextures: chkTextures.IsChecked.Value, showHamburguer: chkBurgers.IsChecked.Value, catSize: catSize); 
            
            window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            window.WindowState = WindowState.Maximized;
            window.Show();
            this.Close();
        }
    }
}
