using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Windows.UI.Xaml.Data;

namespace AutocompleteControl
{
    public class LimitedCollectionView : ICollectionView, IEnumerable
    {
        public int Limit { get; set; }

        public LimitedCollectionView(IEnumerable list)
            : base(list)
        {
            Limit = int.MaxValue;
        }

        public override int Count { get { return Math.Min(base.Count, Limit); } }

        public override bool MoveCurrentToLast()
        {
            return base.MoveCurrentToPosition(Count - 1);
        }

        public override bool MoveCurrentToNext()
        {
            if (base.CurrentPosition == Count - 1)
                return base.MoveCurrentToPosition(base.Count);
            else 
                return base.MoveCurrentToNext();
        }

        public override bool MoveCurrentToPrevious()
        {
            if (base.IsCurrentAfterLast)
                return base.MoveCurrentToPosition(Count - 1);
            else
                return base.MoveCurrentToPrevious();
        }

        public override bool MoveCurrentToPosition(int position)
        {
            if (position < Count)
                return base.MoveCurrentToPosition(position);
            else
                return base.MoveCurrentToPosition(base.Count);
        }

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            do
            {
                yield return CurrentItem;
            } while (MoveCurrentToNext());
        }

        #endregion

        public Windows.Foundation.Collections.IObservableVector<object> CollectionGroups
        {
            get { throw new NotImplementedException(); }
        }

        public event EventHandler<object> CurrentChanged;

        public event CurrentChangingEventHandler CurrentChanging;

        public object CurrentItem
        {
            get { throw new NotImplementedException(); }
        }

        public int CurrentPosition
        {
            get { throw new NotImplementedException(); }
        }

        public bool HasMoreItems
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsCurrentAfterLast
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsCurrentBeforeFirst
        {
            get { throw new NotImplementedException(); }
        }

        public Windows.Foundation.IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
        {
            throw new NotImplementedException();
        }

        public bool MoveCurrentTo(object item)
        {
            throw new NotImplementedException();
        }

        public bool MoveCurrentToFirst()
        {
            throw new NotImplementedException();
        }

        public event Windows.Foundation.Collections.VectorChangedEventHandler<object> VectorChanged;

        public int IndexOf(object item)
        {
            throw new NotImplementedException();
        }

        public void Insert(int index, object item)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        public object this[int index]
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

        public void Add(object item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(object item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(object[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool IsReadOnly
        {
            get { throw new NotImplementedException(); }
        }

        public bool Remove(object item)
        {
            throw new NotImplementedException();
        }

        IEnumerator<object> IEnumerable<object>.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
