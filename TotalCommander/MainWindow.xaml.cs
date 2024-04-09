using System.Diagnostics;
using System.Formats.Tar;
using System.IO;
using System.Reflection.Emit;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Net.Mime.MediaTypeNames;

namespace TotalCommander
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string curDirectory;//bien toan cuc
        private List<DirectoryInfo> History1 = new List<DirectoryInfo>();
        private List<DirectoryInfo> History2 = new List<DirectoryInfo>();


        private int activePane = 1;

        private int HistoryIndex1 = 0;

        private int HistoryIndex2 = 0;

        public MainWindow()
        {
            InitializeComponent();

            //BackBtn.Click += HandleBackBtnClick;
            //ForwardBtn.Click += HandleForwardBtnClick;
            //UpBtn.Click += HandleUpBtnClick;

            myListView1.GotFocus += MyListView1_GotFocus;
            myListView2.GotFocus += MyListView2_GotFocus;

            myListView1.SizeChanged += ListView_SizeChanged;
            myListView2.SizeChanged += ListView_SizeChanged;

        }

        private void ListView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ListView listView = sender as ListView; // Ép kiểu sender thành ListView    

            if (listView != null)
            {
                GridView grid = listView.View as GridView;

                var workingWidth = listView.ActualWidth - SystemParameters.VerticalScrollBarWidth;
                var col1 = 0.45;
                var col2 = 0.15;
                var col3 = 0.15;
                var col4 = 0.25;

                grid.Columns[0].Width = workingWidth * col1;
                grid.Columns[1].Width = workingWidth * col2;
                grid.Columns[2].Width = workingWidth * col3;
                grid.Columns[3].Width = workingWidth * col4;
            }
        }

        private void MyListView1_GotFocus(object sender, RoutedEventArgs e)
        {
            activePane = 1;
            curDirectory = History1[HistoryIndex1].FullName;
        }

        private void MyListView2_GotFocus(object sender, RoutedEventArgs e)
        {
            activePane = 2;
            curDirectory = History2[HistoryIndex2].FullName;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (DriveInfo subDrive in DriveInfo.GetDrives())
            {
                myComboBox1.Items.Add(subDrive.Name);
                myComboBox2.Items.Add(subDrive.Name);
            }
        }

        private void ComboBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //History1.Clear();
            string selectionDrive = myComboBox1.SelectedItem as string;

            AddToNavigationHistory1(selectionDrive);
            curDirectory = History1[HistoryIndex1].FullName;
        }

        private void ComboBox2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //History2.Clear();
            string selectionDrive = myComboBox2.SelectedItem as string;

            AddToNavigationHistory2(selectionDrive);
            curDirectory = History2[HistoryIndex2].FullName;

        }


        private void myListView1_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            HandleDoubleClick(myListView1, myLabel1);
        }

        private void myListView2_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            HandleDoubleClick(myListView2, myLabel2);
        }

        private void HandleDoubleClick(ListView listView, System.Windows.Controls.Label label)
        {
            if (listView.SelectedItem != null)
            {
                dynamic selectedItem = listView.SelectedItem;
                string itemName = selectedItem.Name;
                string itemPath = System.IO.Path.Combine(curDirectory, itemName);
                if (selectedItem.Type == "Folder")
                {
                    DirectoryInfo d = new DirectoryInfo(itemPath);
                    if (activePane == 1)
                    {
                        AddToNavigationHistory1(itemPath);
                    }
                    else if (activePane == 2)
                    {
                        AddToNavigationHistory2(itemPath);
                    }
                    curDirectory = itemPath; // Update the current directory
                }
                else if (selectedItem.Type == "File")
                {
                    try
                    {
                        System.Diagnostics.Process.Start(new ProcessStartInfo { FileName = itemPath, UseShellExecute = true });
                    }
                    catch (Exception Ex)
                    {
                        MessageBox.Show(Ex.Message);
                    }
                }
            }
        }

        private void AddToNavigationHistory1(string itemPath)
        {
            DirectoryInfo d = new DirectoryInfo(itemPath);

            int currentIndex = HistoryIndex1;

            //if (currentIndex < History1.Count - 1)
            //{
            //    History1.RemoveRange(currentIndex + 1, History1.Count - currentIndex - 1);
            //}

            History1.Add(d);
            HistoryIndex1 = History1.Count - 1;
            Utilities.UpdateUI(History1[HistoryIndex1].FullName, myListView1, myLabel1,myComboBox1);
        }

        private void AddToNavigationHistory2(string itemPath)
        {
            DirectoryInfo d = new DirectoryInfo(itemPath);

            int currentIndex = HistoryIndex2;

            //if (currentIndex < History2.Count - 1)
            //{
            //    History2.RemoveRange(currentIndex + 1, History2.Count - currentIndex - 1);
            //}

            History2.Add(d);
            HistoryIndex2 = History2.Count - 1;
            Utilities.UpdateUI(History2[HistoryIndex2].FullName, myListView2, myLabel2,myComboBox2);
        }

        private void HandleUpBtnClick(object sender, RoutedEventArgs e)
        {
            if(activePane==1)
            {
                var upPath = History1[HistoryIndex1].FullName;
                int i = upPath.Length - 1;
                for ( ; i >= 0; i--)
                {
                    if (upPath[i] == '\\')
                    {
                        break;
                    }
                }
                upPath = upPath.Substring(0, i);
                if(upPath.Length<3)
                {
                    upPath+= "\\";
                }
                AddToNavigationHistory1(upPath);
            }
            else if(activePane==2) 
            {
                var upPath = History2[HistoryIndex2].FullName;
                int i = upPath.Length - 1;
                for (; i >= 0; i++)
                {
                    if (upPath[i] == '\\')
                    {
                        break;
                    }
                }
                upPath = upPath.Substring(0, i);
                if (upPath.Length < 3)
                {
                    upPath += "\\";
                }
                AddToNavigationHistory2(upPath);
            }
        }
    
        private void HandleForwardBtnClick(object sender, RoutedEventArgs e)
        {
            if (HistoryIndex1 < History1.Count - 1 && activePane == 1)
            {
                HistoryIndex1++;
                Utilities.UpdateUI(History1[HistoryIndex1].FullName, myListView1, myLabel1, myComboBox1);
            }
            else if (HistoryIndex2 < History2.Count - 1 && activePane == 2)
            {
                HistoryIndex2++;
                Utilities.UpdateUI(History2[HistoryIndex2].FullName, myListView2, myLabel2, myComboBox2); ;
            }
        }

        private void HandleBackBtnClick(object sender, RoutedEventArgs e)
        {
            if (HistoryIndex1 > 0 && HistoryIndex1 < History1.Count && activePane == 1)
            {
                HistoryIndex1--;
                //AddToNavigationHistory1(History1[HistoryIndex1].FullName);
                Utilities.UpdateUI(History1[HistoryIndex1].FullName, myListView1, myLabel1, myComboBox1);
            }
            else if (HistoryIndex2 > 0 && HistoryIndex2 < History2.Count && activePane == 2)
            {
                HistoryIndex2--;
                //AddToNavigationHistory2(History2[HistoryIndex2].FullName);
                Utilities.UpdateUI(History2[HistoryIndex2].FullName, myListView2, myLabel2, myComboBox2);
            }
        }

        private void HandleCopyBtnClick(object sender, RoutedEventArgs e)
        {
            if (activePane == 1)
            {
                string source = History1[HistoryIndex1].FullName;
                string destination = History2[HistoryIndex2].FullName;
                var selectedItem = myListView1.SelectedItem as dynamic;

                if (selectedItem != null && source != destination)
                {
                    if (selectedItem.Type == "Folder")
                    {
                        string folderName = selectedItem.Name;
                        string destinationFolderPath = System.IO.Path.Combine(destination, folderName);
                        CopyFolder(System.IO.Path.Combine(source, folderName), destinationFolderPath);
                    }
                    else if (selectedItem.Type == "File")
                    {
                        string fileName = selectedItem.Name;
                        string destinationFilePath = System.IO.Path.Combine(destination, fileName);
                        CopyFile(System.IO.Path.Combine(source, fileName), destinationFilePath);
                    }
                    MessageBox.Show("Copy successsfully.");
                    Utilities.UpdateUI(destination, myListView2, myLabel2, myComboBox2);

                }
            }
            else if (activePane == 2)
            {
                string source = History2[HistoryIndex2].FullName;
                string destination = History1[HistoryIndex1].FullName;
                var selectedItem = myListView2.SelectedItem as dynamic;

                if (selectedItem != null && source != destination)
                {
                    if (selectedItem.Type == "Folder")
                    {
                        string folderName=selectedItem.Name;
                        string destinationFolderPath = System.IO.Path.Combine(destination, folderName);
                        CopyFolder(System.IO.Path.Combine (source, folderName), destinationFolderPath);
                    }
                    else if (selectedItem.Type == "File")
                    {
                        string fileName = selectedItem.Name;
                        string destinationFilePath = System.IO.Path.Combine(destination, fileName);
                        CopyFile(System.IO.Path.Combine(source, fileName), destinationFilePath);
                    }
                    MessageBox.Show("Copy successsfully.");
                    Utilities.UpdateUI(destination, myListView1, myLabel1, myComboBox1);
                }
            }

            static void CopyFile(string sourceFilePath,string destFilePath)
            {
                try
                {
                    File.Copy(sourceFilePath, destFilePath, true);
                }
                catch(Exception ex)
                {
                    MessageBox.Show($"Error copying file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            static void CopyFolder(string sourceFolderPath,string destFolderPath)
            {
                try
                {
                    if (!Directory.Exists(destFolderPath))
                    {
                        Directory.CreateDirectory(destFolderPath);
                    }

                    foreach (var file in Directory.GetFiles(sourceFolderPath))
                    {
                        string fileName = System.IO.Path.GetFileName(file);
                        string destinationFilePath = System.IO.Path.Combine(destFolderPath, fileName);
                        File.Copy(file, destinationFilePath, true);
                    }
                    foreach (var folder in Directory.GetDirectories(sourceFolderPath))
                    {
                        string folderName = System.IO.Path.GetFileName(folder);
                        string destinationFolderPath = System.IO.Path.Combine(destFolderPath, folderName);
                        CopyFolder(folder, destinationFolderPath);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error copying folder: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }

            }
        }

        private void HandleMoveBtnClick(object sender, EventArgs e)
        {
            if (activePane == 1)
            {
                string source = History1[HistoryIndex1].FullName;
                string destination = History2[HistoryIndex2].FullName;
                var selectedItem = myListView1.SelectedItem as dynamic;

                if (selectedItem != null && source != destination)
                {
                    if (selectedItem.Type == "Folder")
                    {
                        string folderName = selectedItem.Name;
                        string destinationFolderPath = System.IO.Path.Combine(destination, folderName);
                        MoveFolder(System.IO.Path.Combine(source, folderName), destinationFolderPath);
                    }
                    else if (selectedItem.Type == "File")
                    {
                        string fileName = selectedItem.Name;
                        string destinationFilePath = System.IO.Path.Combine(destination, fileName);
                        MoveFile(System.IO.Path.Combine(source, fileName), destinationFilePath);
                    }
                    MessageBox.Show("Move successsfully.");
                    Utilities.UpdateUI(source, myListView1, myLabel1, myComboBox1);
                    Utilities.UpdateUI(destination, myListView2, myLabel2, myComboBox2);
                }
            }
            else if (activePane == 2)
            {
                string source = History2[HistoryIndex2].FullName;
                string destination = History1[HistoryIndex1].FullName;
                var selectedItem = myListView2.SelectedItem as dynamic;

                if (selectedItem != null && source != destination)
                {
                    if (selectedItem.Type == "Folder")
                    {
                        string folderName = selectedItem.Name;
                        string destinationFolderPath = System.IO.Path.Combine(destination, folderName);
                        MoveFolder(System.IO.Path.Combine(source, folderName), destinationFolderPath);
                    }
                    else if (selectedItem.Type == "File")
                    {
                        string fileName = selectedItem.Name;
                        string destinationFilePath = System.IO.Path.Combine(destination, fileName);
                        MoveFile(System.IO.Path.Combine(source, fileName), destinationFilePath);
                    }
                    MessageBox.Show("Move successsfully.");
                    Utilities.UpdateUI(destination, myListView1, myLabel1, myComboBox1);
                    Utilities.UpdateUI(source, myListView2, myLabel2, myComboBox2);
                }
            }

            static void MoveFile(string sourceFilePath, string destFolderPath)
            {
                try
                {
                    string fileName = System.IO.Path.GetFileName(sourceFilePath);
                    string destinationFilePath = System.IO.Path.Combine(destFolderPath, fileName);

                    File.Move(sourceFilePath, destFolderPath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error moving file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            static void MoveFolder(string sourceFolderPath, string destFolderPath)
            {
                try
                {
                    if (!Directory.Exists(destFolderPath))
                    {
                        Directory.CreateDirectory(destFolderPath);
                    }

                    string folderName = System.IO.Path.GetFileName(sourceFolderPath);
                    string destinationFolderPath = System.IO.Path.Combine(destFolderPath, folderName);

                    Directory.Move(sourceFolderPath, destinationFolderPath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error moving folder: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }

            }
        }

        private void HandleDelBtnClick(object sender, EventArgs e)
        {
            if (activePane == 1)
            {
                var selectedItem = myListView1.SelectedItem as dynamic;
                if (selectedItem != null)
                {
                    string curItemPath = System.IO.Path.Combine(History1[HistoryIndex1].FullName, selectedItem.Name);

                    if (selectedItem.Type == "Folder")
                    {
                        try
                        {
                            Directory.Delete(curItemPath,true);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error deleting folder!!!");
                        }
                    }
                    else if (selectedItem.Type == "File")
                    {
                        try
                        {
                            File.Delete(curItemPath);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error deleting file!!!");
                        }
                    }
                    MessageBox.Show("Delete successfully.");
                    Utilities.UpdateUI(History1[HistoryIndex1].FullName, myListView1, myLabel1, myComboBox1);
                }
            }
            else if(activePane == 2)
            {
                var selectedItem = myListView2.SelectedItem as dynamic;
                if (selectedItem != null)
                {
                    string curItemPath = System.IO.Path.Combine(History2[HistoryIndex2].FullName, selectedItem.Name);

                    if (selectedItem.Type == "Folder")
                    {
                        try
                        {
                            Directory.Delete(curItemPath, true);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Error deleting folder: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    else if (selectedItem.Type == "File")
                    {
                        try
                        {
                            File.Delete(curItemPath);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Error deleting file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    MessageBox.Show("Delete successfully.");
                    Utilities.UpdateUI(History2[HistoryIndex2].FullName, myListView2, myLabel2, myComboBox2);
                }
            }
        }

    }

    static class Utilities
    {
        public static void UpdateUI(string path, ListView listView, System.Windows.Controls.Label label, ComboBox comboBox)
        {
            UpdateListView(path, listView);
            UpdateComboBox(path, comboBox);
            UpdateLabel(path, label);
        }

        private static void UpdateListView(string drive, ListView listView)
        {
            listView.Items.Clear();
            if (drive != null && Directory.Exists(drive))
            {
                DirectoryInfo d = new DirectoryInfo(drive);

                foreach (var item in d.GetFileSystemInfos())
                {
                    listView.Items.Add(new
                    {
                        Name = item.Name,
                        Type = (item is DirectoryInfo) ? "Folder" : "File",
                        Size = (item is FileInfo) ? ((FileInfo)item).Length.ToString() : "",
                        Date = item.CreationTime.ToString(),
                    });

                }
            }
        }

        private static void UpdateLabel(string path, System.Windows.Controls.Label label)
        {
            label.Content = path; // will be xau
        }

        private static void UpdateComboBox(string path, ComboBox comboBox)
        {
            if (path != null)
            {
                string support = path;
                string curDrive = path.Substring(0, 3);
                for (int i = 0; i < comboBox.Items.Count; i++)
                {
                    if (curDrive == comboBox.Items[i].ToString())
                    {
                        comboBox.SelectedItem = comboBox.Items[i];
                        path = support;

                        break;
                    }
                }
            }
        }

    }

}

