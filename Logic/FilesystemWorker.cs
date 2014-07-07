using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quintessence.Logic
{
    class WorkerNotification:EventArgs
    {
        public Contact Contact { get; set; }
        public string RelativePath { get; set; }

        public string NewRelativePath { get; set; }

        public string LocalPath { get; set; }

        public bool Desyncronized { get; set; }

        public bool IsDirectory { get; set; }
    }
    class FilesystemWorker
    {
        DirectoryInfo IdentityDirectory;
        DirectoryInfo SharedDirectory;

        public event EventHandler<WorkerNotification> OnFileChanged;

        public FilesystemWorker(string localIdentity)
        {
            string folder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Quint\\Identities\\" + localIdentity;
            Directory.CreateDirectory(folder);

            IdentityDirectory = new DirectoryInfo(folder);
            SharedDirectory =  IdentityDirectory.CreateSubdirectory("Shared");
        }

        public void CopyToShared(SharedFile[] files,Contact recipient)
        {
            DirectoryInfo recipientDirectory = SharedDirectory.CreateSubdirectory(recipient.Id.ToString());
            Copy(files, recipientDirectory);
        }

        public SharedFile[] GetLocalSharedFiles(Contact recipient)
        {
            List<SharedFile> files = new List<SharedFile>();
            DirectoryInfo recipientDirectory = SharedDirectory.CreateSubdirectory(recipient.Id.ToString());
            FileSystemWatcher watcher = new FileSystemWatcher(recipientDirectory.FullName);
            watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            watcher.Renamed+=new RenamedEventHandler((obj,args)=>
                {
                    string oldPath = args.OldFullPath.Remove(0, recipientDirectory.FullName.Length).Remove(0,1);
                    string newPath = "";
                    bool desycned=true;
                    if(args.FullPath.StartsWith(recipientDirectory.FullName))
                    {
                        desycned=false;
                        newPath=args.FullPath.Remove(0, recipientDirectory.FullName.Length).Remove(0,1);
                    }
                    OnFileChanged(this, new WorkerNotification() { NewRelativePath = newPath,LocalPath=args.FullPath, RelativePath = oldPath, Contact = recipient, Desyncronized = desycned });
                    
                });
            watcher.Created += new FileSystemEventHandler((obj, args) =>
                {
                    
                    OnFileChanged(this, new WorkerNotification() {IsDirectory=Directory.Exists(args.FullPath),LocalPath=args.FullPath, NewRelativePath = args.FullPath.Remove(0,recipientDirectory.FullName.Length+1), RelativePath = "", Contact = recipient, Desyncronized = true });
                });
            watcher.IncludeSubdirectories = true;
            watcher.EnableRaisingEvents = true;
            foreach (DirectoryInfo dir in recipientDirectory.GetDirectories())
            {
                SharedFile sf = new SharedFile();
                sf.Name = dir.Name;
                sf.IsFolder = true;
                sf.LocalLocation = dir.FullName;
                sf.Files = new System.Collections.ObjectModel.ObservableCollection<SharedFile>(ProcessDirectory(dir));
                files.Add(sf);
            }
            foreach(FileInfo fi in recipientDirectory.GetFiles())
            {
                SharedFile sf = new SharedFile();
                sf.Name = fi.Name;
                sf.LocalLocation = fi.FullName;
                files.Add(sf);
            }
            return files.ToArray();
        }

        private SharedFile[] ProcessDirectory(DirectoryInfo di)
        {
            List<SharedFile> files = new List<SharedFile>();
            foreach (DirectoryInfo dir in di.GetDirectories())
            {
                SharedFile sf = new SharedFile();
                sf.Name = dir.Name;
                sf.IsFolder = true;
                sf.LocalLocation = dir.FullName;
                sf.Files = new System.Collections.ObjectModel.ObservableCollection<SharedFile>(ProcessDirectory(dir));
                files.Add(sf);
            }
            foreach (FileInfo fi in di.GetFiles())
            {
                SharedFile sf = new SharedFile();
                sf.Name = fi.Name;
                sf.LocalLocation = fi.FullName;
                files.Add(sf);
            }
            return files.ToArray();
        }

        private void Copy(SharedFile[] files, DirectoryInfo di)
        {
            foreach(SharedFile sf in files)
            {
                if(sf.IsFolder)
                {
                    DirectoryInfo sub =  di.CreateSubdirectory(sf.Name);
                    Copy(sf.Files.ToArray(), sub);
                    continue;
                }
                else
                {
                    File.Copy(sf.LocalLocation, di.FullName + "\\" + sf.Name);
                    sf.LocalLocation = di.FullName + "\\" + sf.Name;
                    continue;
                }
            }
        }
    }
}
