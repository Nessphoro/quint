using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Quintessence.Logic
{
    enum SharedFileState
    {
        Synced,Syncing,Error
    }
    class SharedFile
    {

        public SharedFile()
        {
            Files = new ObservableCollection<SharedFile>();
            State = SharedFileState.Syncing;
        }

        public string Name
        {
            get;
            set;
        }

        public bool IsFolder
        {
            get;
            set;
        }

        public string LocalLocation
        {
            get;
            set;
        }

        public SharedFileState State
        {
            get;
            set;
        }

        public ObservableCollection<SharedFile> Files
        {
            get;
            set;
        }

        public SharedFile Self
        {
            get
            {
                return this;
            }
        }
    }
}
