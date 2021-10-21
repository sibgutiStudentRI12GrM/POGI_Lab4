using System;
using System.Collections.Generic;
using System.Collections;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;
namespace minesweeper_v2 {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        HashSet<int> MinesIds;
        HashSet<int> PickedNonMinesIds;
        Dictionary<int, Cell> AllCells;

        
        int MapSize;
        bool IsGameEnded = false;
        BitmapImage MineBitmapImage = new BitmapImage(new Uri("pack://application:,,,/mine.png", UriKind.Absolute));
        
        int CellsCount {
            get {
                return GameMapUGrid.Columns * GameMapUGrid.Rows;
            }
        }
        

        int PickedNonMinesCount {
            get {
                return PickedNonMinesIds.Count;
            }
        }

        int MinesCount {
            get {
                return MinesIds.Count;
            }
        }
        
        
        public MainWindow() {
            InitializeComponent();
        }
        public class Cell {
            public int MineSweeperStatuscode = 0;
            public int CellId;
            
            public Button btn;
            public Cell(int id, ref Button btn) {
                this.CellId = id;
                this.btn = btn;
            }
        }

        
        void DrawMap(int size) {
            GameMapUGrid.Children.Clear();
            AllCells = new Dictionary<int, Cell>();
            GameMapUGrid.Rows = size;
            GameMapUGrid.Columns = size;

            for (int id = 0; id < size * size; id++) {
                Button btn = new Button();
                btn.Click += Btn_Click;
                btn.Tag = id;
                btn.FontSize = 12;
                
                btn.BorderBrush = Brushes.Black;
                btn.BorderThickness = new Thickness(0.5);
                Cell cell = new Cell(id, ref btn);
                AllCells.Add(id, cell);
                GameMapUGrid.Children.Add(btn);
            }
            
            
        }

        bool PlantMine(int seed) { // true if success plant, false if cant plant due full
            
            Random rnd = new Random(seed);

            int i;
            int positionCondidate = 0;
            bool isPlanted = false;
            int tryLimit = 10 * CellsCount;

            for (i = 0; i <= tryLimit; i++) {
                positionCondidate = rnd.Next(0, CellsCount);
                if (!MinesIds.Contains(positionCondidate)) {
                    bool allMinesAround = true;
                    foreach (int aroundId in GetAroundIdsSet(positionCondidate, MapSize, MapSize)) {
                        if (!MinesIds.Contains(aroundId)) {
                            allMinesAround = false;
                            
                        }
                    }

                    if (!allMinesAround) {
                        MinesIds.Add(positionCondidate);
                        AllCells[positionCondidate].MineSweeperStatuscode = 9;
                        isPlanted = true;
                        break;
                    }
                }
            }

            if (isPlanted) {
                foreach (int aroundId in GetAroundIdsSet(positionCondidate, MapSize, MapSize)) {
                    if (!MinesIds.Contains(aroundId)) {
                        AllCells[aroundId].MineSweeperStatuscode += 1;
                        
                    }
                }
                return true;
            }
            return false;
        }

        void OpenAllMines() {
            for (int cellId = 0; cellId < AllCells.Count; cellId++) {
                Cell currentCell = AllCells[cellId];

                if (currentCell.MineSweeperStatuscode == 9) {
                    Image mineImage = new Image();

                    mineImage.Source = MineBitmapImage;

                    StackPanel stackPnl = new StackPanel();

                    stackPnl.Margin = new Thickness(5);

                    stackPnl.Children.Add(mineImage);
                    currentCell.btn.IsEnabled = false;
                    currentCell.btn.Content = stackPnl;
                    currentCell.btn.Foreground = Brushes.Blue; ;
                    
                }
            }
        }

        void RecursiveOpenCells(int id) {
            HashSet<int> aroundIds = GetAroundIdsSet(id, MapSize, MapSize);
            int currentStatuscode = AllCells[id].MineSweeperStatuscode;

            foreach (int aroundId in aroundIds) {
                Cell aroundCell = AllCells[aroundId];
                int aroundStatuscode = aroundCell.MineSweeperStatuscode;

                if (aroundCell.btn.IsEnabled) {
                    if (currentStatuscode == 0) {
                        if (aroundStatuscode == 0) {
                            OpenCell(aroundId);
                            RecursiveOpenCells(aroundId);
                        } else if (aroundStatuscode != 9) {
                            OpenCell(aroundId);
                        }
                    }
                }
                
            }
        }

        void OpenCell(int id) {
            Cell currentCell = AllCells[id];
            int cellStatuscode = currentCell.MineSweeperStatuscode;
            currentCell.btn.IsEnabled = false;
            if (cellStatuscode > 0 && cellStatuscode < 9) { // display how many mines around picked cell
                currentCell.btn.Content = $"  {cellStatuscode}  ";
                
                
            } else if (cellStatuscode == 9) {
                IsGameEnded = true;
                OpenAllMines();
                DisplayLose();
                return;
            }
            PickedNonMinesIds.Add(id);
            if (IsWin()) {
                OpenAllMines();
                DisplayWin();
            }

        }
        bool IsWin() {
            if ((PickedNonMinesIds.Count + MinesIds.Count) == CellsCount) {
                IsGameEnded = true;
                return true;
            }
            
            return false;
        }
        void DisplayWin() {
            MessageBox.Show("You win! Cool!");
        }
        void DisplayLose() {
            MessageBox.Show("GameOver");
        }
        public HashSet<int> GetAroundIdsSet(int id, int tableHeight, int tableWidth) {
            int maxTableHeightIndex = tableHeight - 1;
            int maxTableWidthIndex = tableWidth - 1;

            HashSet<int> arroundIds = new HashSet<int>();
            int cellHeihtIndex = id / tableWidth;
            int cellWidthIndex = id % tableWidth;


            bool hasUp = cellHeihtIndex != 0;
            bool hasDown = cellHeihtIndex != maxTableHeightIndex;
            bool hasLeft = cellWidthIndex != 0;
            bool hasRight = cellWidthIndex != maxTableWidthIndex;
            // up
            if (hasUp) {
                arroundIds.Add(id - tableWidth);
            }
            // down
            if (hasDown) {
                arroundIds.Add(id + tableWidth);
            }
            // left
            if (hasLeft) {
                arroundIds.Add(id - 1);
            }
            // right
            if (hasRight) {
                arroundIds.Add(id + 1);
            }


            // up left
            if (hasUp && hasLeft) {
                arroundIds.Add(id - tableWidth - 1);
            }
            //up right
            if (hasUp && hasRight) {
                arroundIds.Add(id - tableWidth + 1);
            }
            // down left
            if (hasDown && hasLeft) {
                arroundIds.Add(id + tableWidth - 1);
            }
            // down right
            if (hasDown && hasRight) {
                arroundIds.Add(id + tableWidth + 1);
            }
            return arroundIds;
        }

        private void Btn_Click(object sender, RoutedEventArgs e) {
            if (!IsGameEnded) {
                int cellId = (int)((Button)sender).Tag;
                int cellStatuscode = AllCells[cellId].MineSweeperStatuscode;
                OpenCell(cellId);
                RecursiveOpenCells(cellId);
            } else {
                MessageBox.Show("Game is ended");
            }
            
        }

        private void Btn_NewGame_Click(object sender, RoutedEventArgs e) {
            NewGameSettingsWindow NGWindow = new NewGameSettingsWindow();
            if (NGWindow.ShowDialog() == true) {
                IsGameEnded = false;
                MapSize = NGWindow.PickedMapSize;
                int pickedMinesCount = NGWindow.PickedMinesCount;
                int pickedSeed = NGWindow.PickedSeed;
                PickedNonMinesIds = new HashSet<int>();
                MinesIds = new HashSet<int>();
                DrawMap(MapSize);

                Random randomSubseedGenerator = new Random(pickedSeed);
                for (int i = 0; i < pickedMinesCount; i++) {
                    if (!PlantMine(randomSubseedGenerator.Next())) {
                        MessageBox.Show($"Couldnt plant {pickedMinesCount - MinesCount} mines due overfill");
                        break;
                    }
                }
                MinesCountLabel.Content = $"{MinesCount}";
            }
        }
    }
}
