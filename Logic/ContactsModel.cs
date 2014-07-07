using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Quintessence.Logic
{

    class ContactsModel:INotifyPropertyChanged
    {
        Dictionary<long, ObservableCollection<SharedFile>> cache = new Dictionary<long, ObservableCollection<SharedFile>>();
        internal FilesystemWorker fs;
        Window parentWindow;
        public ContactsModel(Window parent)
        {
            this.parentWindow = parent;
            fs = new FilesystemWorker("Pavlo Malynin");
            fs.OnFileChanged += fs_OnFileChanged;
            //TODO: Fetch Contacts
            _contacts = new ObservableCollection<Contact>();
            _contacts.Add(new Contact() { Name = "Pavlo Malynin",Id=0 });
            _contacts.Add(new Contact() { Name = "xxxHelloKittyxxx",Id=1 });
            _contacts.Add(new Contact() { Name = "Vanilla Face",Id=2 });

            foreach(Contact contact in _contacts)
            {
                cache.Add(contact.Id, new ObservableCollection<SharedFile>(fs.GetLocalSharedFiles(contact)));
            }

            _path = new ObservableCollection<PathElement>();

            AddContact = new AddContactCommand(this);
        }
        ManualResetEvent syncer = new ManualResetEvent(true);
        void fs_OnFileChanged(object sender, WorkerNotification e)
        {
            syncer.WaitOne();
            syncer.Reset();
            parentWindow.Dispatcher.BeginInvoke(new Action(() =>
            {
                string[] oldTokens = e.RelativePath.Split('\\');
                string[] newTokens = e.NewRelativePath.Split('\\');
                if (e.NewRelativePath == "")
                {
                    //Delete file
                }
                else if (e.RelativePath == "")
                {
                    SharedFile sf = new SharedFile();
                    sf.Name = newTokens[newTokens.Length - 1];
                    sf.LocalLocation = e.LocalPath;
                    sf.IsFolder = e.IsDirectory;
                    if (newTokens.Length > 1)
                    {
                        SharedFile initial;
                        SharedFile parent = initial = cache[e.Contact.Id].FirstOrDefault(f => f.Name.Equals(newTokens[0]));
                        if (parent == null)
                        {
                            parent = new SharedFile();
                            parent.Name = newTokens[0];
                            parent.IsFolder = true;
                            cache[e.Contact.Id].Add(parent);
                        }
                        for (int i = 1; i < oldTokens.Length; i++)
                        {
                            parent = parent.Files.First(f => f.Name == oldTokens[i]);
                            if (parent == null)
                            {
                                parent = new SharedFile();
                                parent.Name = newTokens[0];
                                parent.IsFolder = true;
                                parent.Files.Add(parent);
                            }
                        }
                        parent.Files.Add(sf);
                        cache[e.Contact.Id].Remove(initial);
                        cache[e.Contact.Id].Add(initial);
                        NotifiyChange("SharedFiles");
                    }
                    else
                    {
                        if (cache[e.Contact.Id].FirstOrDefault(f => f.Name == sf.Name) != default(SharedFile))
                        {
                            return;
                        }
                        cache[e.Contact.Id].Add(sf);
                        NotifiyChange("SharedFiles");
                    }
                }
                else if (e.RelativePath != e.NewRelativePath && !e.Desyncronized)
                {
                    SharedFile initial = cache[e.Contact.Id].First(f => f.Name == oldTokens[0]);
                    for (int i = 1; i < oldTokens.Length; i++)
                    {
                        initial = initial.Files.First(f => f.Name == oldTokens[i]);
                    }
                    initial.Name = newTokens[newTokens.Length - 1];
                    initial.LocalLocation = e.LocalPath;
                    cache[e.Contact.Id].Remove(initial);
                    cache[e.Contact.Id].Add(initial);
                    NotifiyChange("SharedFiles");
                    //Rename
                }
                else if (e.Desyncronized)
                {
                    //File changed
                }
            }));
            syncer.Set();
        }

        ObservableCollection<Contact> _contacts;

        public IList<Contact> Contacts
        {
            get
            {
                return (from contact in _contacts where contact.Name.ToLower().Contains(_searchTerm.ToLower()) select contact).ToList();
            }
        }

        ObservableCollection<SharedFile> _files;

        public ObservableCollection<SharedFile> SharedFiles
        {
            get
            {
                return _files;
            }
        }

        ObservableCollection<PathElement> _path;

        public ObservableCollection<PathElement> PathModel
        {
            get
            {
                return _path;
            }
        }

        string _searchTerm="";

        public string SearchTerm
        {
            get
            {
                return _searchTerm;
            }
            set
            {
                _searchTerm = value;
                NotifiyChange("SearchTerm");
                NotifiyChange("Contacts");
            }
        }

        internal bool addingAFriend = false;

        #region Commands
        public ICommand AddContact
        {
            get;
            internal set; 
        }
        
        public ICommand  CreateGroup
        {
            get
            {
                return new CreateGroupCommand(this);
            }
        }

        public ICommand DropFile
        {
            get
            {
                return new DropFileCommand(this);
            }
        }
        
        public ICommand SelectContact
        {
            get
            {
                return new SelectContactCommand(this);
            }
        }

        public ICommand DeleteFile
        {
            get
            {
                return new DeleteItemCommand(this);
            }
        }

        public ICommand SelectPath
        {
            get
            {
                return new SelectPathCommand(this);
            }
        }

        public ICommand FileDoubleClick
        {
            get
            {
                return new FileDoubleClickCommand(this);
            }
        }

        public ICommand GoBack
        {
            get
            {
                return new GoBackCommand(this);
            }
        }

        public ICommand GoForward
        {
            get
            {
                return new GoForwardCommand(this);
            }
        }

        public ICommand OpenExplorer
        {
            get
            {
                return new OpenExplorerCommand();
            }
        }
        #endregion

        internal AddContact Window;

        internal Contact CurrentContact;

        public void LoadFiles(Contact contact)
        {
            if (contact == null)
                return;
            CurrentContact = contact;
            _files = cache[contact.Id];

            HistoryBack.Clear();
            HistoryForward.Clear();
            _path.Clear();
            _path.Add(new PathElement() { IsRoot = true, Name = contact.Name, ElementImage = new System.Windows.Media.Imaging.BitmapImage(new Uri("/Resources/Buttons/triangle_right.png", UriKind.Relative)) });

            NotifiyChange("PathModel");
            NotifiyChange("SharedFiles");
        }

        public void AddContactExecute(Contact contact)
        {
            _contacts.Add(contact);
            NotifiyChange("Contacts");
        }

        public Stack<PathElement> HistoryBack = new Stack<PathElement>();
        public Stack<PathElement> HistoryForward = new Stack<PathElement>();

        public void ChangeFileContext(PathElement file)
        {
            if (file.IsRoot)
            {
                _files = cache[CurrentContact.Id];
            }
            else if (file.File == null)
                return;
            else if (file.File.IsFolder)
            {
                _files = file.File.Files;
            }
            NotifiyChange("SharedFiles");
        }

        #region Notify Change
        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifiyChange(string property)
        {
            if(PropertyChanged!=null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
        #endregion
    }

    #region Command Implementations
    class AddContactCommand:ICommand
    {
        ContactsModel model;

        public AddContactCommand(ContactsModel model)
        {
            this.model = model;
        }

        public bool CanExecute(object parameter)
        {
            return !model.addingAFriend;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            this.model.addingAFriend = true;
            CanExecuteChanged(this, EventArgs.Empty);

            model.Window = new AddContact();
            model.Window.DataContext = new AddContactsModel(model);
            model.Window.Closed += new EventHandler((obj, ev) =>
                {
                    this.model.addingAFriend = false;
                    CanExecuteChanged(this, EventArgs.Empty);
                });
            model.Window.Show();

        }
    }

    class CreateGroupCommand : ICommand
    {
        ContactsModel model;

        public CreateGroupCommand(ContactsModel model)
        {
            this.model = model;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            System.Collections.IList items = (System.Collections.IList)parameter;
            Contact[] array = items.Cast<Contact>().ToArray();
            if(array.Length<2)
            {
                ErrorDialog errd = new ErrorDialog();
                errd.DataContext = new {Error="You need to select at least 2 contacts to create a group",Close=new CloseCommand(errd)};
                errd.ShowDialog();
            }
        }
    }

    class DropFileCommand:ICommand
    {
        ContactsModel model;
        public DropFileCommand(ContactsModel model)
        {
            this.model = model;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            string[] files = parameter as string[];
            List<SharedFile> sfiles = new List<SharedFile>();
            foreach(string file in files)
            {

                sfiles.Add(GetFile(file));
            }
            model.fs.CopyToShared(sfiles.ToArray(), model.CurrentContact);
            foreach (SharedFile sf in sfiles)
                model.SharedFiles.Add(sf);
        }

        private SharedFile GetFile(string file)
        {
            bool isFile = false;
            string name = "";

            SharedFile sf = new SharedFile();

            FileInfo fi = new FileInfo(file);
            if (fi.Exists)
            {
                isFile = true;
                name = fi.Name;
            }
            else
            {
                DirectoryInfo di = new DirectoryInfo(file);
                name = di.Name;
                foreach(DirectoryInfo d in di.GetDirectories())
                {
                    sf.Files.Add(GetFile(d.FullName));
                }
                foreach (FileInfo f in di.GetFiles())
                {
                    sf.Files.Add(GetFile(f.FullName));
                }
            }

            
            sf.IsFolder = !isFile;
            sf.LocalLocation = file;
            sf.Name = name;
            return sf;
        }
    }

    class DeleteItemCommand:ICommand
    {
        ContactsModel model;
        public DeleteItemCommand(ContactsModel model)
        {
            this.model = model;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            SharedFile sf = parameter as SharedFile;
            if(sf.IsFolder)
            {
                Directory.Delete(sf.LocalLocation,true);
            }
            else
            {
                File.Delete(sf.LocalLocation);
            }
            model.SharedFiles.Remove(sf);
            
        }
    }

    class SelectContactCommand:ICommand
    {
        ContactsModel model;
        public SelectContactCommand(ContactsModel model)
        {
            this.model = model;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            model.LoadFiles(parameter as Contact);
        }
    }

    class SelectPathCommand:ICommand
    {

        ContactsModel model;
        public SelectPathCommand(ContactsModel model)
        {
            this.model = model;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;

        static bool processing = false;
        public void Execute(object parameter)
        {
            if (processing || parameter==null)
                return;
            processing = true;

            List<PathElement> tree = new List<PathElement>();
            PathElement original = parameter as PathElement;
            PathElement pe = parameter as PathElement;
            if(pe==model.PathModel.Last())
            {
                processing = false;
                return;
            }
            model.HistoryForward.Push(pe);
            model.HistoryBack.Clear();

            tree.Add(pe);
            pe = pe.Parent;
            while(pe!=null)
            {
                tree.Add(pe);
                pe = pe.Parent;
            }

            tree.Reverse();
            model.PathModel.Clear();
            foreach(PathElement element in tree)
            {
                model.PathModel.Add(element);
            }
            model.NotifiyChange("PathModel");
            model.ChangeFileContext(original);
            
            processing = false;
        }
    }

    class FileDoubleClickCommand:ICommand
    {
        ContactsModel model;
        public FileDoubleClickCommand(ContactsModel model)
        {
            this.model = model;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            SharedFile file = parameter as SharedFile;
            if(file.IsFolder)
            {
                PathElement pe = new PathElement();
                pe.File = file;
                pe.Name = file.Name;
                pe.Parent = model.PathModel.Last();
                pe.ElementImage = new System.Windows.Media.Imaging.BitmapImage(new Uri("/Resources/Buttons/triangle_right.png", UriKind.Relative));
                model.HistoryForward.Clear();
                model.HistoryBack.Push(model.PathModel.Last());
                model.PathModel.Add(pe);
                model.NotifiyChange("PathModel");
                model.ChangeFileContext(pe);
            }
            else
            {
                System.Diagnostics.Process.Start(file.LocalLocation);
            }
        }
    }

    class GoBackCommand:ICommand
    {
        ContactsModel model;

        public GoBackCommand(ContactsModel model)
        {
            this.model = model;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            if(model.HistoryBack.Count==0)
            {
                return;
            }
            else
            {
                PathElement current = model.PathModel.Last();
                model.HistoryForward.Push(current);
                PathElement pe= model.HistoryBack.Peek();

                List<PathElement> tree = new List<PathElement>();

                tree.Add(pe);
                pe = pe.Parent;
                while (pe != null)
                {
                    tree.Add(pe);
                    pe = pe.Parent;
                }

                tree.Reverse();
                model.PathModel.Clear();
                foreach (PathElement element in tree)
                {
                    model.PathModel.Add(element);
                }
                model.NotifiyChange("PathModel");
                model.ChangeFileContext(model.HistoryBack.Pop());
            }
        }
    }

    class GoForwardCommand : ICommand
    {
        ContactsModel model;

        public GoForwardCommand(ContactsModel model)
        {
            this.model = model;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            if (model.HistoryForward.Count == 0)
            {
                return;
            }
            else
            {
                PathElement current = model.PathModel.Last();
                model.HistoryBack.Push(current);
                PathElement pe = model.HistoryForward.Peek();

                List<PathElement> tree = new List<PathElement>();

                tree.Add(pe);
                pe = pe.Parent;
                while (pe != null)
                {
                    tree.Add(pe);
                    pe = pe.Parent;
                }

                tree.Reverse();
                model.PathModel.Clear();
                foreach (PathElement element in tree)
                {
                    model.PathModel.Add(element);
                }
                model.NotifiyChange("PathModel");
                model.ChangeFileContext(model.HistoryForward.Pop());
            }
        }
    }

    class OpenExplorerCommand:ICommand
    {

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            SharedFile sf = parameter as SharedFile;
            System.Diagnostics.Process.Start("Explorer","/select,"+sf.LocalLocation);
        }
    }
    #endregion
}
