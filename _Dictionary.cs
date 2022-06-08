using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static System.Collections.Generic.Dictionary<string, string>;

namespace Dictionary
{
    public class _Dictionary
    {
        private struct Entry

        {
            public int hashCode;
            public int next;
            public string key;
            public string value;
        }
        private int[] buckets;
        private Entry[] entries;
        private int count;
        private int version;
        private int freeList;
        private int freeCount;
        private KeyCollection keys;
        private ValueCollection values;
        private IEqualityComparer<string> comparer;
        public _Dictionary() : this(0, null) { }
        public _Dictionary(int capacity) : this(capacity, null) { }
        public _Dictionary(IEqualityComparer<string> comparer) : this(0, comparer) { }
        public _Dictionary(int capacity, IEqualityComparer<string> comparer)
        {
            if (capacity < 0) throw new ArgumentOutOfRangeException();
            if (capacity > 0) Initialize(capacity);
            this.comparer = comparer ?? EqualityComparer<string>.Default;
        }
        public _Dictionary(IDictionary<string, string> dictionary) : this(dictionary, null) { }
        public _Dictionary(IDictionary<string, string> dictionary, IEqualityComparer<string> comparer) :
        this(dictionary != null ? dictionary.Count : 0, comparer)
        {
            if (dictionary == null)
            {
                throw new ArgumentNullException();
            }
            foreach (KeyValuePair<string, string> pair in dictionary
            {
                Add(pair.Key, pair.Value);
            }
        }
        public IEqualityComparer<string> Comparer
        {
            get
            {
                return comparer;
            }
        }
        public bool ContainsKey(string key)
        {
            return FindEntry(key) >= 0;
        }
        public bool ContainsValue(string value)
        {
            if (value == null)
            {
                for (int i = 0; i < count; i++)
                {
                    if (entries[i].hashCode >= 0 && entries[i].value == null) return true;
                }
            }
            else
            {
                EqualityComparer<string> c = EqualityComparer<string>.Default;
                for (int i = 0; i < count; i++)
                {
                    if (entries[i].hashCode >= 0 && c.Equals(entries[i].value, value)) return true;
                }
            }
            return false;
        }
        public int Count
        {
            get { return count - freeCount; }
        }
        public void Clear()
        {
            if (count > 0)
            {
                for (int i = 0; i < buckets.Length; i++) buckets[i] = -1;
                Array.Clear(entries, 0, count);
                freeList = -1;
                count = 0;
                freeCount = 0;
                version++;
            }
        }
        public KeyCollection Keys
        {
            get
            {
                if (keys == null) keys = new KeyCollection(this);
                return keys;
            }
        }
        public ValueCollection Values
        {
            get
            {
                if (values == null) values = new ValueCollection(this);
                return values;
            }
        }
        public void _CopyTo(KeyValuePair[] array, int index)
        {
            if (array == null)
            {
                throw new ArgumentNullException();
            }
            int count = this.count;
            Entry[] entries = this.entries;
            for (int i = 0; i < count; i++)
            {
                if (entries[i].hashCode >= 0)
                {
                    array[index++] = new KeyValuePair(entries[i].key, entries[i].value);
                }
            }
        }
        public bool Remove(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException();
            }

            if (buckets != null)
            {
                int hashCode = comparer.GetHashCode(key) & 0x7FFFFFFF;
                int bucket = hashCode % buckets.Length;
                int last = -1;
                for (int i = buckets[bucket]; i >= 0; last = i, i = entries[i].next)
                {
                    if (entries[i].hashCode == hashCode && comparer.Equals(entries[i].key, key))
                    {
                        if (last < 0)
                        {
                            buckets[bucket] = entries[i].next;
                        }
                        else
                        {
                            entries[last].next = entries[i].next;
                        }
                        entries[i].hashCode = -1;
                        entries[i].next = freeList;
                        entries[i].key = default(string);
                        entries[i].value = default(string);
                        freeList = i;
                        freeCount++;
                        version++;
                        return true;
                    }
                }
            }
            return false;
        }
        public string this[string key]

        {
            get
            {
                int i = FindEntry(key);
                if (i >= 0) return entries[i].value;
                throw new KeyNotFoundException();
            }
            set
            {
                Insert(key, value, false);
            }
        }
        public void Add(string key, string value)
        {
            Insert(key, value, true);
        }
        private void Resize()
        {
            Resize(HashHelpers.ExpandPrime(count), false);
        }
        private void Resize(int newSize, bool forceNewHashCodes)

        {

            int[] newBuckets = new int[newSize];

            for (int i = 0; i < newBuckets.Length; i++) newBuckets[i] = -1;

            Entry[] newEntries = new Entry[newSize];

            Array.Copy(entries, 0, newEntries, 0, count);

            if (forceNewHashCodes)

            {
                for (int i = 0; i < count; i++)
                {
                    if (newEntries[i].hashCode != -1)
                    {
                       newEntries[i].hashCode = (comparer.GetHashCode(newEntries[i].key) & 0x7FFFFFFF);
                    }
                }
            }
            for (int i = 0; i < count; i++)
            {
                if (newEntries[i].hashCode >= 0)
                {
                    int bucket = newEntries[i].hashCode % newSize;
                    newEntries[i].next = newBuckets[bucket];
                    newBuckets[bucket] = i;
                }
            }
            buckets = newBuckets;
            entries = newEntries;
        }
        private void Insert(string key, string value, bool add)
        {
            if (key == null)
            {
                throw new ArgumentNullException();
            }
            if (buckets == null) Initialize(0);
            int hashCode = comparer.GetHashCode(key) & 0x7FFFFFFF;
            int targetBucket = hashCode % buckets.Length;
            int collisionCount = 0;
            for (int i = buckets[targetBucket]; i >= 0; i = entries[i].next)
            {
                if (entries[i].hashCode == hashCode && comparer.Equals(entries[i].key, key))
                {
                    if (add)
                    {
                        throw new ArgumentException();
                    }
                    entries[i].value = value;
                    version++;
                    return;
                }
               collisionCount++;
            }
            int index;
            if (freeCount > 0)
            {
                index = freeList;
                freeList = entries[index].next;
                freeCount--;
            }
            else
            {
                if (count == entries.Length)
                { 
                    Resize();
                    targetBucket = hashCode % buckets.Length;
                }
                index = count;
                count++;
            }
            entries[index].hashCode = hashCode;
            entries[index].next = buckets[targetBucket];
            entries[index].key = key;
            entries[index].value = value;
            buckets[targetBucket] = index;
            version++;
        }
        private void Initialize(int capacity)

        {
            int size = HashHelpers.GetPrime(capacity);
            buckets = new int[size];
            for (int i = 0; i < buckets.Length; i++) buckets[i] = -1;
            entries = new Entry[size];
            freeList = -1;
        }
        private int FindEntry(string key)

        {
            if (key == null)
            {
                throw new ArgumentNullException();
            }
            if (buckets != null)
            {
                int hashCode = comparer.GetHashCode(key) & 0x7FFFFFFF;
                for (int i = buckets[hashCode % buckets.Length]; i >= 0; i = entries[i].next)
                {
                    if (entries[i].hashCode == hashCode && comparer.Equals(entries[i].key, key)) return i;
                }
            }
            return -1;
        }

        public class ValueCollection
        {
            private _Dictionary dictionary;
            public ValueCollection(_Dictionary dictionary)
            {
                if (dictionary == null)
                {
                    throw new ArgumentNullException();
                }
                this.dictionary = dictionary;
            }
            public Enumerator GetEnumerator()
            {
                return new Enumerator(dictionary);
            }
            public struct Enumerator : IEnumerator<string>
            {
                private _Dictionary dictionary;
                private int index;
                private int version;
                private string currentValue;

                public Enumerator(_Dictionary dictionary)
                {
                    this.dictionary = dictionary;
                    version = dictionary.version;
                    index = 0;
                    currentValue = default(string);
                }
                public void Dispose()
                {
                }
                public bool MoveNext()
                {
                    if (version != dictionary.version)
                    {
                        throw new InvalidOperationException();
                    }

                    while ((uint)index < (uint)dictionary.count)
                    {
                        if (dictionary.entries[index].hashCode >= 0)
                        {
                            currentValue = dictionary.entries[index].value;
                            index++;
                            return true;
                        }
                        index++;
                    }

                    index = dictionary.count + 1;
                    currentValue = default(string);
                    return false;
                }
                public string Current
                {
                    get
                    {
                        return currentValue;
                    }
                }
                Object System.Collections.IEnumerator.Current
                {
                    get
                    {
                        if (index == 0 || (index == dictionary.count + 1))
                        {
                            throw new InvalidOperationException();
                        }

                        return currentValue;
                    }
                }
                void System.Collections.IEnumerator.Reset()
                {
                    if (version != dictionary.version)
                    {
                        throw new InvalidOperationException();
                    }

                    index = 0;
                    currentValue = default(string);
                }
            }
        }
        public class KeyCollection
        {
            private _Dictionary dictionary;
            public KeyCollection(_Dictionary dictionary)
            {
                if (dictionary == null)
                {
                    throw new ArgumentNullException();
                }
                this.dictionary = dictionary;
            }
            public Enumerator GetEnumerator()
            {
                return new Enumerator(dictionary);
            }
            public struct Enumerator : IEnumerator<string>
            {
                private _Dictionary dictionary;
                private int index;
                private int version;
                private string currentKey;

                public Enumerator(_Dictionary dictionary)
                {
                    this.dictionary = dictionary;
                    version = dictionary.version;
                    index = 0;
                    currentKey = default(string);
                }
                public void Dispose()
                {
                }
                public bool MoveNext()
                {
                    if (version != dictionary.version)
                    {
                        throw new InvalidOperationException();
                    }

                    while ((uint)index < (uint)dictionary.count)
                    {
                        if (dictionary.entries[index].hashCode >= 0)
                        {
                            currentKey = dictionary.entries[index].key;
                            index++;
                            return true;
                        }
                        index++;
                    }

                    index = dictionary.count + 1;
                    currentKey = default(string);
                    return false;
                }
                public string Current
                {
                    get
                    {
                        return currentKey;
                    }
                }
                Object System.Collections.IEnumerator.Current
                {
                    get
                    {
                        if (index == 0 || (index == dictionary.count + 1))
                        {
                            throw new InvalidOperationException();
                        }

                        return currentKey;
                    }
                }
                void System.Collections.IEnumerator.Reset()
                {
                    if (version != dictionary.version)
                    {
                        throw new InvalidOperationException();
                    }
                    index = 0;
                    currentKey = default(string);
                }
            }
        }

    }
}


