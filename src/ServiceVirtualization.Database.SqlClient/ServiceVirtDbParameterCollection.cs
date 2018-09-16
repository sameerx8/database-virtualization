using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;

namespace ServiceVirtualization.Database.SqlClient {
    public class ServiceVirtDbParameterCollection : DbParameterCollection {
        private List<DbParameter> _items;

        public ServiceVirtDbParameterCollection()  {
            _items = new List<DbParameter>();
        }

        override public int Count {
            get {
                return ((null != _items) ? _items.Count : 0);
            }
        }

        public void AddWithValue(string parameterName, object value) {
            CheckParameterNameArg(parameterName);

            if (ParameterExists(parameterName)) return;

            AddParameter(parameterName, value);
        }

        private void AddParameter(string parameterName, object value) {
            _items.Add(new ServiceVirtDbParameter {ParameterName = parameterName, Value = value});
        }

        private void AddParameter(string parameterName) {
            _items.Add(new ServiceVirtDbParameter {ParameterName = parameterName});
        }

        private static void CheckParameterNameArg(string parameterName) {
            if (string.IsNullOrWhiteSpace(parameterName)) throw new ArgumentNullException(nameof(parameterName));
        }

        private bool ParameterExists(string parameterName) {
            if (_items.Any(param =>
                param.ParameterName.Equals(parameterName, StringComparison.OrdinalIgnoreCase))) return true;
            return false;
        }

        public void Add(string parameterName) {
            CheckParameterNameArg(parameterName);

            if (ParameterExists(parameterName)) return;

            AddParameter(parameterName);
        }

        private List<DbParameter> InnerList {
            get {
                List<DbParameter> items = _items;

                if (null == items)
                {
                    items = new List<DbParameter>();
                    _items = items;
                }
                return items;
            }
        }


        override public object SyncRoot {
            get {
                return ((System.Collections.ICollection)InnerList).SyncRoot;
            }
        }

        override public int Add(object value)
        {
            ValidateType(value);
            Validate(-1, value);
            InnerList.Add((DbParameter)value);
            return Count - 1;
        }

        override public void AddRange(System.Array values)
        {

            foreach (object value in values)
            {
                ValidateType(value);
            }
            foreach (DbParameter value in values)
            {
                Validate(-1, value);
                InnerList.Add((DbParameter)value);
            }
        }

        private int CheckName(string parameterName)
        {
            int index = IndexOf(parameterName);
            if (index < 0)
            {
                throw new Exception("Parameter not found");
            }
            return index;
        }

        override public void Clear()
        {
            List<DbParameter> items = InnerList;

            if (null != items)
            {

                items.Clear();
            }
        }

        override public bool Contains(object value)
        {
            return (-1 != IndexOf(value));
        }

        public override bool Contains(string value) {
            throw new NotImplementedException();
        }

        override public void CopyTo(Array array, int index)
        {
            ((System.Collections.ICollection)InnerList).CopyTo(array, index);
        }

        override public System.Collections.IEnumerator GetEnumerator()
        {
            return ((System.Collections.ICollection)InnerList).GetEnumerator();
        }

        override protected DbParameter GetParameter(int index)
        {
            RangeCheck(index);
            return InnerList[index];
        }

        override protected DbParameter GetParameter(string parameterName)
        {
            int index = IndexOf(parameterName);
            if (index < 0)
            {
                throw new Exception("parameter not found");
            }
            return InnerList[index];
        }

        private static int IndexOf(System.Collections.IEnumerable items, string parameterName)
        {
            if (null != items)
            {
                int i = 0;

                foreach (DbParameter parameter in items)
                {
                    if (parameterName == parameter.ParameterName)
                    {
                        return i;
                    }
                    ++i;
                }
                i = 0;

                foreach (DbParameter parameter in items)
                {
                    if (0 == parameterName.CompareTo( parameter.ParameterName))
                    {
                        return i;
                    }
                    ++i;
                }
            }
            return -1;
        }

        override public int IndexOf(string parameterName)
        {
            return IndexOf(InnerList, parameterName);
        }

        override public int IndexOf(object value)
        {
            if (null != value)
            {
                ValidateType(value);

                List<DbParameter> items = InnerList;

                if (null != items)
                {
                    int count = items.Count;

                    for (int i = 0; i < count; i++)
                    {
                        if (value == items[i])
                        {
                            return i;
                        }
                    }
                }
            }
            return -1;
        }

        override public void Insert(int index, object value)
        {

            ValidateType(value);
            Validate(-1, (DbParameter)value);
            InnerList.Insert(index, (DbParameter)value);
        }

        private void RangeCheck(int index)
        {
            if ((index < 0) || (Count <= index))
            {
                throw new Exception ("RangeCheck");
            }
        }

        override public void Remove(object value)
        {
            ValidateType(value);
            int index = IndexOf(value);
            if (-1 != index)
            {
                RemoveIndex(index);
            }
        }

        override public void RemoveAt(int index)
        {
            RangeCheck(index);
            RemoveIndex(index);
        }

        override public void RemoveAt(string parameterName)
        {
            int index = CheckName(parameterName);
            RemoveIndex(index);
        }

        private void RemoveIndex(int index)
        {
            List<DbParameter> items = InnerList;
            Debug.Assert((null != items) && (0 <= index) && (index < Count), "RemoveIndex, invalid");
            DbParameter item = items[index];
            items.RemoveAt(index);
        }

        private void Replace(int index, object newValue)
        {
            List<DbParameter> items = InnerList;
            Debug.Assert((null != items) && (0 <= index) && (index < Count), "Replace Index invalid");
            ValidateType(newValue);
            Validate(index, newValue);
            DbParameter item = items[index];
            items[index] = (DbParameter)newValue;
        }

        override protected void SetParameter(int index, DbParameter value)
        {
            RangeCheck(index);
            Replace(index, value);
        }

        override protected void SetParameter(string parameterName, DbParameter value)
        {
            int index = IndexOf(parameterName);
            if (index < 0)
            {
                throw new Exception("SetParameter - param not found");
            }
            Replace(index, value);
        }

        private void Validate(int index, object value)
        {
        }

        private void ValidateType(object value)
        {
        }
    }
}