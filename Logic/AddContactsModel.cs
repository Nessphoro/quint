using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Quintessence.Logic
{
    class AddContactsModel : INotifyPropertyChanged
    {
        internal ContactsModel caller;
        public AddContactsModel(ContactsModel model)
        {
            caller = model;
        }

        string email = "";

        public string Email
        {
            get { return email; }
            set
            {
                email = value;
                NotifiyChange("Email");
            }
        }

        public ICommand Close
        {
            get
            {
                return new CloseCommand(caller.Window);
            }
        }

        public ICommand Add
        {
            get
            {
                return new AddContactFinalCommand(this);
            }
        }

        #region Notify Change
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifiyChange(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
        #endregion
    }

    class CloseCommand : ICommand
    {
        Window model;

        public CloseCommand(Window model)
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
            model.Close();
        }
    }
    class AddContactFinalCommand : ICommand
    {
        AddContactsModel model;

        public AddContactFinalCommand(AddContactsModel model)
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
            model.caller.AddContactExecute(new Contact() { Name = model.Email });
            model.caller.Window.Close();
        }
    }
}
