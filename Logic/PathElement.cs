using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Quintessence.Logic
{
    class PathElement
    {
        public string Name
        {
            get;
            set;
        }

        public BitmapImage ElementImage
        {
            get;
            set;
        }

        public bool IsRoot
        {
            get;
            set;
        }

        public SharedFile File
        {
            get;
            set;
        }

        public PathElement Parent
        {
            get;
            set;
        }
    }
}
