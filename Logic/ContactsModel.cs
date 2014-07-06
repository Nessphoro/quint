using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Quintessence.Logic
{
    class ContactsModel:INotifyPropertyChanged
    {
        Dictionary<long, ObservableCollection<SharedFile>> cache = new Dictionary<long, ObservableCollection<SharedFile>>();
        public ContactsModel()
        {
            //TODO: Fetch Contacts
            _contacts = new ObservableCollection<Contact>();
            _contacts.Add(new Contact() { Name = "Pavlo Malynin",Id=0 });
            _contacts.Add(new Contact() { Name = "xxxHelloKittyxxx",Id=1 });
            _contacts.Add(new Contact() { Name = "Vanilla Face",Id=2 });

            ObservableCollection<SharedFile> pavFiles = new ObservableCollection<SharedFile>();
            pavFiles.Add(new SharedFile() { Name = "CS101", IsFolder = true, State = SharedFileState.Error });
            pavFiles.Add(new SharedFile() { Name = "BIO300", IsFolder = true, State = SharedFileState.Syncing });
            pavFiles.Add(new SharedFile() { Name = "ENGI100", IsFolder = true, State = SharedFileState.Synced });
            cache.Add(0, pavFiles);
            cache.Add(1, new ObservableCollection<SharedFile>());
            cache.Add(2, new ObservableCollection<SharedFile>());
            AddContact = new AddContactCommand(this);
        }

        ObservableCollection<Contact> _contacts;

        public ObservableCollection<Contact> Contacts
        {
            get
            {
                return _contacts;
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

        internal bool addingAFriend = false;

        public ICommand AddContact
        {
            get;
            internal set; 
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

        internal AddContact Window;

        public void LoadFiles(Contact contact)
        {
            _files = cache[contact.Id];
            NotifiyChange("SharedFiles");
        }

        #region Notify Change
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifiyChange(string property)
        {
            if(PropertyChanged!=null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
        #endregion
    }

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
            foreach(string file in files)
            {
                bool isFile = false;
                string name = "";

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
                }

                SharedFile sf = new SharedFile();
                sf.IsFolder = !isFile;
                sf.Name = name;
                model.SharedFiles.Add(sf);
            }
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
}
