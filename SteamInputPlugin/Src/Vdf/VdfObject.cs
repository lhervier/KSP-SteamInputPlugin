using System;
using System.Collections;
using System.Collections.Generic;

namespace com.github.lhervier.ksp.Vdf
{
    /// <summary>
    /// A parsed VDF block: an ordered map from key to value, where a value is a string
    /// (leaf), a nested <see cref="VdfObject"/>, or a <see cref="VdfArray"/> (a key that
    /// appeared several times). Typed accessors mirror the previous Dictionary helpers:
    /// a missing key yields an empty block / empty array / empty string rather than
    /// throwing; a present key of the wrong kind throws.
    /// </summary>
    public sealed class VdfObject : IEnumerable<KeyValuePair<string, object>>
    {
        private readonly Dictionary<string, object> _entries = new Dictionary<string, object>();

        public int Count
        {
            get { return _entries.Count; }
        }

        public ICollection<string> Keys
        {
            get { return _entries.Keys; }
        }

        public bool ContainsKey(string key)
        {
            return _entries.ContainsKey(key);
        }

        /// <summary>
        /// Add a key/value pair, collapsing a repeated key into a <see cref="VdfArray"/>.
        /// A repeated key that mixes string and block values is rejected as invalid VDF.
        /// </summary>
        /// <param name="key">The entry key.</param>
        /// <param name="value">A string or a <see cref="VdfObject"/>.</param>
        internal void Add(string key, object value)
        {
            if (!_entries.TryGetValue(key, out object existing))
            {
                _entries[key] = value;
                return;
            }

            if (existing is VdfArray array)
            {
                RejectKindMismatch(key, array.IsObjectArray, value);
                array.Append(value);
                return;
            }

            RejectKindMismatch(key, existing is VdfObject, value);
            _entries[key] = new VdfArray(new[] { existing, value });
        }

        private static void RejectKindMismatch(string key, bool existingIsBlock, object value)
        {
            bool valueIsBlock = value is VdfObject;
            if (existingIsBlock != valueIsBlock)
            {
                throw new VdfParseException("Repeated key '" + key + "' mixes string and block values", 0, 0);
            }
        }

        /// <summary>Nested block for <paramref name="key"/>, or an empty block if absent.</summary>
        public VdfObject GetObject(string key)
        {
            if (!_entries.TryGetValue(key, out object value))
            {
                return new VdfObject();
            }
            if (value is VdfObject block)
            {
                return block;
            }
            throw new InvalidOperationException("Expected block for key '" + key + "', got " + value.GetType().Name);
        }

        /// <summary>String value for <paramref name="key"/>, or "" if absent.</summary>
        public string GetString(string key)
        {
            if (!_entries.TryGetValue(key, out object value))
            {
                return "";
            }
            if (value is string str)
            {
                return str;
            }
            throw new InvalidOperationException("Expected string for key '" + key + "', got " + value.GetType().Name);
        }

        /// <summary>
        /// Values for <paramref name="key"/> as an array: the array itself when the key was
        /// repeated, a single-element array when it appeared once, or an empty array if absent.
        /// </summary>
        public VdfArray GetArray(string key)
        {
            if (!_entries.TryGetValue(key, out object value))
            {
                return new VdfArray();
            }
            if (value is VdfArray array)
            {
                return array;
            }
            return new VdfArray(new[] { value });
        }

        /// <summary>True (with the value) if <paramref name="key"/> is present and is a string.</summary>
        public bool TryGetString(string key, out string value)
        {
            value = null;
            if (!_entries.TryGetValue(key, out object raw) || !(raw is string str))
            {
                return false;
            }
            value = str;
            return true;
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return _entries.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _entries.GetEnumerator();
        }
    }
}
