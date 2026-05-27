using System.Collections;
using System.Collections.Generic;

namespace com.github.lhervier.ksp.Vdf
{
    /// <summary>
    /// An ordered sequence of VDF values, produced when the same key appears several
    /// times at the same level. All items share the same kind: either all strings or
    /// all <see cref="VdfObject"/> blocks (mixing the two is rejected at parse time).
    /// </summary>
    public sealed class VdfArray : IEnumerable<object>
    {
        private readonly List<object> _items;

        public VdfArray()
        {
            _items = new List<object>();
        }

        internal VdfArray(IEnumerable<object> items)
        {
            _items = new List<object>(items);
        }

        public int Count
        {
            get { return _items.Count; }
        }

        public object this[int index]
        {
            get { return _items[index]; }
        }

        /// <summary>True if this array holds blocks; false if it holds strings (or is empty).</summary>
        public bool IsObjectArray
        {
            get { return _items.Count > 0 && _items[0] is VdfObject; }
        }

        internal void Append(object value)
        {
            _items.Add(value);
        }

        /// <summary>The items typed as blocks. Assumes an object array.</summary>
        public IEnumerable<VdfObject> Objects()
        {
            foreach (var item in _items)
            {
                yield return (VdfObject)item;
            }
        }

        /// <summary>The items typed as strings. Assumes a string array.</summary>
        public IEnumerable<string> Strings()
        {
            foreach (var item in _items)
            {
                yield return (string)item;
            }
        }

        public IEnumerator<object> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _items.GetEnumerator();
        }
    }
}
