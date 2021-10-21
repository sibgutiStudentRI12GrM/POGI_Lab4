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

namespace minesweeper_v2 {
    /// <summary>
    /// Interaction logic for NewGameSettingsWindow.xaml
    /// </summary>
    public partial class NewGameSettingsWindow : Window {
        public int PickedMapSize;
        public int PickedMinesCount;
        public int PickedSeed;
        public NewGameSettingsWindow() {
            InitializeComponent();
        }
        int generateSeed(int length) {
            Random rnd = new Random();
            string seed = "";
            seed += rnd.Next(1, 10).ToString();
            for (int i = 0; i < length - 1; i++) {
                seed += rnd.Next(10).ToString();
            }
            return Int32.Parse(seed);
        }

        private void Btn_RandomizeSeed_Click(object sender, RoutedEventArgs e) {
            SeedTextBox.Text = generateSeed(5).ToString();
        }
        private void Btn_PlayGame_Click(object sender, RoutedEventArgs e) {

            // validations for picked data
            if (!Int32.TryParse(MapSizeTextBox.Text, out PickedMapSize)) {
                MessageBox.Show("Incorrect map format");
                return;
            }

            if (PickedMapSize < 2) {
                MessageBox.Show("Map size should be >= 2");
                return;
            }
            if (!Int32.TryParse(MinesCountTextBox.Text, out PickedMinesCount)) {
                MessageBox.Show("Incorrect mines format");
                return;
            }


            if (!Int32.TryParse(SeedTextBox.Text, out PickedSeed)) {
                MessageBox.Show("Seed must be a number");
                return;
            }

            this.DialogResult = true;

        }
    }
}
