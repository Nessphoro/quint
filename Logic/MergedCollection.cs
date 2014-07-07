using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Quintessence.Logic
{
    class MergedCollection<T>:IList<T>
    {
        enum MergingOperation
        {
            Add,Remove
        }
        class MergingCommand
        {
            public T Item;
            public MergingOperation Operation;
        }

        HashSet<T> RemovedItems;
        ObservableCollection<T> readOnly;
        List<T> writeBack;

        Queue<MergingCommand> restoringCommands;

        public MergedCollection(ObservableCollection<T> source)
        {
            readOnly = source;
            writeBack = new List<T>();
            RemovedItems = new HashSet<T>();
            restoringCommands=new Queue<MergingCommand>();
        }

        public int IndexOf(T item)
        {
            throw new NotImplementedException();
        }

        public void Insert(int index, T item)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        public T this[int index]
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public T FirstOrDefault(Func<T,bool> comp)
        {
            T first1 = readOnly.FirstOrDefault(comp);
            T first2 = writeBack.FirstOrDefault(comp);
            T first3 = RemovedItems.FirstOrDefault(comp);

            if (first3 != null)
                return default(T);
            else
            {
                if (first1 != null)
                    return first1;
                else if (first2 != null)
                    return first2;
                else
                    return default(T);
            }

        }

        public void Add(T item)
        {
            if(RemovedItems.Contains(item))
            {
                RemovedItems.Remove(item);
                restoringCommands.Enqueue(new MergingCommand() { Item = item, Operation = MergingOperation.Add });
                return;
            }
            writeBack.Add(item);
            restoringCommands.Enqueue(new MergingCommand() { Item = item, Operation = MergingOperation.Add });
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(T item)
        {
            if (RemovedItems.Contains(item))
                return false;

            if(readOnly.Contains(item) || writeBack.Contains(item))
            {
                return true;
            }
            return false;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public int Count
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsReadOnly
        {
            get { throw new NotImplementedException(); }
        }

        public bool Remove(T item)
        {
            RemovedItems.Add(item);
            restoringCommands.Enqueue(new MergingCommand() { Item = item, Operation = MergingOperation.Remove });
            return true;
        }

        public ManualResetEvent Merge(Dispatcher dispatcher,ManualResetEvent syncronizer)
        {
            syncronizer.Reset();

            dispatcher.BeginInvoke(new Action(() =>
                {
                    while(restoringCommands.Count>0)
                    {
                        MergingCommand c = restoringCommands.Dequeue();
                        switch(c.Operation)
                        {
                            case MergingOperation.Remove:
                                readOnly.Remove(c.Item);
                                break;
                            case MergingOperation.Add:
                                readOnly.Add(c.Item);
                                break;
                        }
                    }
                    mre.Set();
                }));

            return mre;
        }

        public IEnumerator<T> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
