using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileManager.ViewModel.Command;
using System.Windows;
using System.IO;
using System.Collections.ObjectModel;
using FileManager.Model;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using FileManager.ViewModel.Services;
using FileManager.DB;

namespace FileManager.ViewModel
{
    public class ManagerViewModel : INotifyPropertyChanged
    {
        private List<DriveInfo> drivers = new List<DriveInfo>();
        public ObservableCollection<IModel> ElementsOfDirectory { get; set; }

        private string _curretPath = "";
        private string _infoFolderOrFile;
        private string _searchItem;

        private IModel _selectedItem;

        private DbWorker _dbWorker;

        public ManagerViewModel()
        {
            
            ElementsOfDirectory = new ObservableCollection<IModel>();
            
            _dbWorker = new DbWorker("Records.db");

            GetFilesAndFolder(_curretPath);
        }

        private RelayCommand _openFolder;
        public RelayCommand OpenFolder
        {
            get
            {
                return _openFolder ?? (_openFolder = new RelayCommand(o => OpenFolders(_selectedItem.Path)));
            }
        }

        private RelayCommand _backCommand;
        public RelayCommand BackCommand
        {
            get
            {
                return _backCommand ?? (_backCommand = new RelayCommand(o => BackFolder()));
            }
        }
        public IModel SelectedItem
        {
            get => _selectedItem;

            set
            {
                _selectedItem = value;
                OnPropertyChanged("SelectedItem");
                GetInformation();
            }

        }
        public string InformationItem
        {
            get => _infoFolderOrFile;

            set
            {
                _infoFolderOrFile = value;
                OnPropertyChanged("InformationItem");
                
            }
        }

        public string SearchItem
        {
            get => _searchItem;

            set
            {
                _searchItem = value;
                OnPropertyChanged("SearchItem");
                SearchItems();
            }
        }
        private async void SearchItems()
        {

            var searchItem = _searchItem;

            if (string.IsNullOrWhiteSpace(_searchItem))
            {
                _searchItem = string.Empty;
                await GetFilesAndFolder(_curretPath);
            }

            else
            {
                searchItem = searchItem.ToLowerInvariant();

                await GetFilesAndFolder(_curretPath);

                var ListElements = ElementsOfDirectory.Where(x => x.Name.ToLower().Contains(searchItem)).ToList();
                ElementsOfDirectory.Clear();
                ListElements.ForEach(x => ElementsOfDirectory.Add(x));

            }
        }

        private void GetDrives()
        {
            ElementsOfDirectory.Clear();

            drivers = DriveInfo.GetDrives().ToList();
            drivers.ForEach(x => ElementsOfDirectory.Add(new Folder {
                                                                         Icon = "/Images/drives.png",
                                                                         Path = x.Name,
                                                                         Name = x.Name,
                                                                         Type = "Disk"
            }));


        }

        private async void OpenFolders(string path)
        {
            
            string typeElement = ElementsOfDirectory.Where(x => x.Path == path).First().Type;
            if (typeElement == "Folder" || typeElement == "Disk")
            {
                _curretPath = path;
                await GetFilesAndFolder(_curretPath);
            }
            else if (typeElement == "File")
                await OpenFile(path);
        }
        private async Task<string> OpenFile(string path)
        {

            _dbWorker.AddRecord(ElementsOfDirectory.Where(x => x.Path == path).First().Name);

            System.Diagnostics.Process.Start(path);

            return "Sucsses";
        }


        private async Task<string> GetFilesAndFolder(string path)
        {
            if (string.IsNullOrEmpty(_curretPath))
            {
                GetDrives();
                return "Failed";
            }
                
            List<string> directories = Directory.GetDirectories(path).ToList();
            List<string> files = Directory.GetFiles(path).ToList();

            ElementsOfDirectory.Clear();

            foreach (var directorie in directories)
            {

                try
                {
                    ElementsOfDirectory.Add(new Folder
                    {
                        Name = new DirectoryInfo(directorie).Name,
                        Path = directorie,
                        CountOfFiles = (new DirectoryInfo(directorie).GetDirectories().Count() + new DirectoryInfo(directorie).GetFiles().Count()),
                        Icon = "/Images/folder.png",
                        Type = "Folder"
                    });
                }

                catch (Exception ex)
                {
                    Console.WriteLine("Can't open folder : " + ex.Message);
                }
            }

            foreach (var file in files)
            {

                try
                {
                    ElementsOfDirectory.Add(
                        new Model.File
                        {
                            Name = new FileInfo(file).Name,
                            path = file,
                            icon = "/Images/file.png",
                            size = new FileInfo(file).Length.ToString(),
                            Type = "File",
                        });
                }

                catch (Exception ex)
                {
                    Console.WriteLine("Can't open file : " + ex.Message);
                }
            }

            return "Sucsess";
        }
        private  void GetInformation()
        {
            if(_selectedItem != null && _selectedItem.Type == "Folder")
            {
                InformationItem = $"Type: {_selectedItem.Type} \n" +
                    $"Name : {_selectedItem.Name}\n" +
                    $"Size: {FolderWorkers.GetSize(new DirectoryInfo(_selectedItem.Path)).Result} МБ\n" +
                    $"Count Files: {ElementsOfDirectory.OfType<Folder>().Where(x => x.Name == _selectedItem.Name).First().CountOfFiles}";

            }

            if(_selectedItem != null && _selectedItem.Type == "File")
            {
                InformationItem = $"Type: {_selectedItem.Type} \n" +
                    $"Name : {_selectedItem.Name}\n" +
                    $"Directory Name : {new FileInfo(_selectedItem.Path).DirectoryName}\n" +
                    $"Size: {_selectedItem.Size}\n" +
                    $"Data Creation: {new FileInfo(_selectedItem.Path).CreationTime}\n" +
                    $"Last Write Time: {new FileInfo(_selectedItem.Path).LastWriteTime}";
            }
        }

        private async void BackFolder()
        {
            if (Directory.GetParent(_curretPath) != null)
            {
                _curretPath = Directory.GetParent(_curretPath).ToString();
                await GetFilesAndFolder(_curretPath);
            }
            else
            {
                _curretPath = null;
                GetFilesAndFolder(_curretPath);
            }
        }



        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
