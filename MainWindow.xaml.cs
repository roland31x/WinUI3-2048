using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using System;
using Windows.System;
using Windows.Graphics;
using System.Threading.Tasks;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WinUI3_2048
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        int Size = 4;
        int Steps { get; set; }
        int Score { get; set; }
        readonly double Percent = 0.7;
        bool GameWon = false;
        readonly Random rng = new Random();
        GameTile[,] Mat;
        bool awaiting = false;
        bool endless = false;
        public MainWindow()
        {
            InitializeComponent();
            MainGrid.Loaded += MainGrid_Loaded;
        }

        private void MainGrid_Loaded(object sender, RoutedEventArgs args)
        {
            Mat = new GameTile[Size, Size];
            InitGame();
            StartGame();
            MainGrid.KeyDown += Play;
            MainGrid.SizeChanged += MainGrid_SizeChanged;
            ExtendsContentIntoTitleBar = true;
            this.AppWindow.Resize(new SizeInt32(600, 600));
        }

        private void MainGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Area.Height = MainGrid.ActualHeight * Percent;
            Area.Width = Area.Height;
            foreach (GameTile r in Mat)
            {
                r.Height = Area.Height / 4d - 10;
                r.Width = Area.Width / 4d - 10;                           
            }
            ReDraw();
        }

        private async void Play(object sender, KeyRoutedEventArgs e)
        {
            if (awaiting)
            {
                return;
            }

            
            VirtualKey pressed = e.Key;
           
            switch (pressed)
            {
                case VirtualKey.Left:
                    MoveLeft();
                    break;
                case VirtualKey.Right:
                    MoveRight();
                    break;
                case VirtualKey.Up:
                    MoveUp();
                    break;
                case VirtualKey.Down:
                    MoveDown();
                    break;
                default: return;
            }
            Steps++;
            try
            {
                GenerateNewBox();
            }
            catch (Exception)
            {
                awaiting = true;
                await ShowFailedDialogBox(); 
                awaiting = false;
                StartGame();
                return;
            }          
            ReDraw();

            if (GameWon)
            {
                awaiting = true;
                bool reset = await ShowWonDialogBox();
                awaiting = false;
                if (reset)
                    StartGame();
                else
                {
                    GameWon = false;
                    endless = true;
                }
                
            }

        }

        public void InitGame()
        {
            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    GameTile t = new GameTile
                    {
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        Margin = new Thickness(5,5,5,5)
                    };
                    Area.Children.Add(t);
                    Grid.SetRow(t, i);
                    Grid.SetColumn(t, j);
                    Mat[i, j] = t;
                }
            }
        }
        public void StartGame()
        {
            GameWon = false;
            endless = false;
            Steps = 0;
            Score = 0;
            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    Mat[i, j].Tag = 0;
                }
            }
            GenerateBox();
            GenerateBox();
            ReDraw();
        }
        public void GenerateBox()
        {
            int i = rng.Next(0, Size);
            int j = rng.Next(0, Size);
            while (!OK(i, j))
            {
                i = rng.Next(0, Size);
                j = rng.Next(0, Size);
            }
            GameTile newBox = Mat[i, j];
            newBox.Tag = 1;
        }
        public void GenerateNewBox()
        {
            int i = rng.Next(0, Size);
            int j = rng.Next(0, Size);

            while (!OK(i, j))
            {
                i = rng.Next(0, Size);
                j = rng.Next(0, Size);
            }

            GameTile newBox = Mat[i, j];
            int chance = rng.Next(0, 3);
            if (chance == 2)
                newBox.Tag = 2;
            else 
                newBox.Tag = 1;
        }
        void MoveUp()
        {
            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    for (int k = j + 1; k < Size; k++)
                    {
                        if (Convert.ToInt32(Mat[j, i].Tag) != 0 && Convert.ToInt32(Mat[j, i].Tag) == Convert.ToInt32(Mat[k, i].Tag))
                        {
                            Mat[j, i].Tag = Convert.ToInt32(Mat[j, i].Tag) + 1;
                            Mat[k, i].Tag = 0;
                            Score += (int)Math.Pow(2, Convert.ToDouble(Mat[j, i].Tag));
                            break;
                        }
                        else if (Convert.ToInt32(Mat[k, i].Tag) == 0)
                        {
                            continue;
                        }
                        else break;
                    }
                }
                SortUp(i);
            }
        }
        void SortUp(int i)
        {
            for (int k = 0; k < Size; k++)
            {
                if (Convert.ToInt16(Mat[k, i].Tag) == 0)
                {
                    for (int j = k + 1; j < Size; j++)
                    {
                        if (Convert.ToInt16(Mat[j, i].Tag) != 0)
                        {
                            (Mat[k, i].Tag, Mat[j, i].Tag) = (Mat[j, i].Tag, Mat[k, i].Tag);
                            break;
                        }
                    }
                }
            }
        }
        void MoveDown()
        {
            for (int i = 0; i < Size; i++)
            {
                for (int j = Size - 1; j >= 0; j--)
                {
                    for (int k = j - 1; k >= 0; k--)
                    {
                        if (Convert.ToInt32(Mat[j, i].Tag) != 0 && Convert.ToInt32(Mat[j, i].Tag) == Convert.ToInt32(Mat[k, i].Tag))
                        {
                            Mat[j, i].Tag = Convert.ToInt32(Mat[j, i].Tag) + 1;
                            Mat[k, i].Tag = 0;
                            Score += (int)Math.Pow(2, Convert.ToDouble(Mat[j, i].Tag));
                            break;
                        }
                        else if (Convert.ToInt32(Mat[k, i].Tag) == 0)
                        {
                            continue;
                        }
                        else break;
                    }
                }
                SortDown(i);
            }
        }
        void SortDown(int i)
        {
            for (int k = Size - 1; k >= 0; k--)
            {
                if (Convert.ToInt16(Mat[k, i].Tag) == 0)
                {
                    for (int j = k - 1; j >= 0; j--)
                    {
                        if (Convert.ToInt16(Mat[j, i].Tag) != 0)
                        {
                            (Mat[k, i].Tag, Mat[j, i].Tag) = (Mat[j, i].Tag, Mat[k, i].Tag);
                            break;
                        }
                    }
                }
            }
        }
        void MoveRight()
        {
            for (int i = 0; i < Size; i++)
            {
                for (int j = Size - 1; j >= 0; j--)
                {
                    for (int k = j - 1; k >= 0; k--)
                    {
                        if (Convert.ToInt32(Mat[i, j].Tag) != 0 && Convert.ToInt32(Mat[i, j].Tag) == Convert.ToInt32(Mat[i, k].Tag))
                        {
                            Mat[i, j].Tag = Convert.ToInt32(Mat[i, j].Tag) + 1;
                            Mat[i, k].Tag = 0;
                            Score += (int)Math.Pow(2, Convert.ToDouble(Mat[i, j].Tag));
                            break;
                        }
                        else if (Convert.ToInt32(Mat[i, k].Tag) == 0)
                        {
                            continue;
                        }
                        else break;
                    }
                }
                SortRight(i);
            }
        }
        void SortRight(int i)
        {
            for (int k = Size - 1; k >= 0; k--)
            {
                if (Convert.ToInt16(Mat[i, k].Tag) == 0)
                {
                    for (int j = k - 1; j >= 0; j--)
                    {
                        if (Convert.ToInt16(Mat[i, j].Tag) != 0)
                        {
                            (Mat[i, k].Tag, Mat[i, j].Tag) = (Mat[i, j].Tag, Mat[i, k].Tag);
                            break;
                        }

                    }
                }
            }
        }
        void MoveLeft()
        {
            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    for (int k = j + 1; k < Size; k++)
                    {
                        if (Convert.ToInt32(Mat[i, j].Tag) != 0 && Convert.ToInt32(Mat[i, j].Tag) == Convert.ToInt32(Mat[i, k].Tag))
                        {
                            Mat[i, j].Tag = Convert.ToInt32(Mat[i, j].Tag) + 1;
                            Mat[i, k].Tag = 0;
                            Score += (int)Math.Pow(2, Convert.ToDouble(Mat[i, j].Tag));
                            break;
                        }
                        else if (Convert.ToInt32(Mat[i, k].Tag) == 0)
                        {
                            continue;
                        }
                        else break;
                    }
                }
                SortLeft(i);
            }
        }
        void SortLeft(int i)
        {
            for (int k = 0; k < Size; k++)
            {
                if (Convert.ToInt16(Mat[i, k].Tag) == 0)
                {
                    for (int j = k + 1; j < Size; j++)
                    {
                        if (Convert.ToInt16(Mat[i, j].Tag) != 0)
                        {
                            (Mat[i, k].Tag, Mat[i, j].Tag) = (Mat[i, j].Tag, Mat[i, k].Tag);
                            break;
                        }
                    }
                }

            }
        }
        void ReDraw()
        {
            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    GameBlockSet(Convert.ToInt16(Mat[i, j].Tag), Mat[i, j]);
                }
            }

            foreach(GameTile r in Mat)
            {
                if(r.Height > 0)
                {
                    string text = r.TBlock.Text;
                    if(text.Length > 0)
                        r.TBlock.FontSize = Math.Clamp((r.Height) / ((double)text.Length), 6, r.Height / 3);                  
                }               
            }           

            StepsBox.Text = "Steps: " + Convert.ToString(Steps);
            ScoreBox.Text = "Score: " + Convert.ToString(Score);
        }
        void GameBlockSet(int i, GameTile t)
        {
            if (i == 0)
            {
                t.TBlock.Text = "";
            }
            else
            {
                t.TBlock.Text = Convert.ToString(Math.Pow(2, i));
            }

            if (i == 11 && !endless)
            {
                GameWon = true;
            }
            t.Body.Fill = GetColor(i);
        }
        void Reset(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    Mat[i, j].Tag = "0";
                }
            }
            ReDraw();
            StartGame();
        }
        bool OK(int i, int j)
        {
            bool canGenerate = false;
            for (int i1 = 0; i1 < Size; i1++)
            {
                for (int j1 = 0; j1 < Size; j1++)
                {
                    if (Convert.ToInt16(Mat[i1, j1].Tag) == 0)
                    {
                        canGenerate = true;
                    }
                }
            }
            if (canGenerate)
            {
                if (Convert.ToInt16(Mat[i, j].Tag) == 0)
                {
                    return true;
                }
                else return false;
            }
            else
            {
                throw new Exception("Died");
            }

        }

        private async Task ShowFailedDialogBox()
        {
            ContentDialog dialog = new ContentDialog();

            dialog.XamlRoot = MainGrid.XamlRoot;
            dialog.Title = $"You Failed! Try Again!";
            dialog.Content = $"Your score is {Score} in {Steps} steps.";
            dialog.PrimaryButtonText = "Restart";

            await dialog.ShowAsync();
        }
        private async Task<bool> ShowWonDialogBox()
        {
            ContentDialog dialog = new ContentDialog();

            dialog.XamlRoot = MainGrid.XamlRoot;
            dialog.Title = "You Win!";
            dialog.Content = $"Your score is {Score} in {Steps} steps." + Environment.NewLine + "Do you want to continue endlessly?";
            dialog.PrimaryButtonText = "Yes";
            dialog.SecondaryButtonText = "Restart Game";

            ContentDialogResult dr = await dialog.ShowAsync();
            if (dr == ContentDialogResult.Primary)
                return false;
            else
                return true;
        }
        static SolidColorBrush GetColor(int i)
        {
            switch (i)
            {
                case 0:
                    return new SolidColorBrush(Colors.Gray);
                case 1:
                    return new SolidColorBrush(Colors.Tan);
                case 2:
                    return new SolidColorBrush(Colors.SandyBrown);
                case 3:
                    return new SolidColorBrush(Colors.RosyBrown);
                case 4:
                    return new SolidColorBrush(Colors.Orange);
                case 5:
                    return new SolidColorBrush(Colors.Red);
                case 6:
                    return new SolidColorBrush(Colors.Yellow);
                case 7:
                    return new SolidColorBrush(Colors.Teal);
                case 8:
                    return new SolidColorBrush(Colors.LawnGreen);
                case 9:
                    return new SolidColorBrush(Colors.LightBlue);
                case 10:
                    return new SolidColorBrush(Colors.Purple);
                case 11:
                    return new SolidColorBrush(Colors.Pink);
                default:
                    return new SolidColorBrush(Colors.Black);

            }
        }
    }
}
