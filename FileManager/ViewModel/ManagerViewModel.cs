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

namespace FileManager.ViewModel
{
    public class ManagerViewModel : INotifyPropertyChanged
    {
        private List<DriveInfo> drivers = new List<DriveInfo>();
        public ObservableCollection<IModel> ElementsOfDirectory { get; set; }

        private string _curretPath = "C:/";
        private string _infoFolderOrFile;
        private IModel _selectedItem;
         


        public ManagerViewModel()
        {
            drivers = DriveInfo.GetDrives().ToList();
            ElementsOfDirectory = new ObservableCollection<IModel>();
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
            get { return _selectedItem; }
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
        private async void OpenFolders(string path)
        {
            _curretPath = path;
            await GetFilesAndFolder(_curretPath);
        }
        private async Task<string> GetFilesAndFolder(string path)
        {
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
                catch
                {

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
                    $"Size: {_selectedItem.Size}\n" +
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
            _curretPath = Directory.GetParent(_curretPath).ToString();
            await GetFilesAndFolder(_curretPath);
        }


        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
