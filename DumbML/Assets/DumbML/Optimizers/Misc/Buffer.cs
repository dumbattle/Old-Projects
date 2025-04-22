using System;

namespace DumbML {
    public class Buffer<T> {
        protected T[] _buffer;
        protected int _oldest = 0;
        protected int size = 0;

        public T this[int i] {
            get {
                return _buffer[(_oldest + i) % maxSize];
            }
        }

        public int maxSize; 


        public Buffer(int maxSize) {
            this.maxSize = maxSize;
            _buffer = new T[maxSize];
        }

        public virtual void Add (T item) {
            _buffer[_oldest] = item;

            _oldest = (_oldest + 1) % maxSize;
            size++;
        }
        protected Random rng = new Random();
        public virtual T[] GetRandom(int count) {
            int max = size.Clamp(0, _buffer.Length);

            T[] result = new T[count];

            for (int i = 0; i < count; i++) {
                result[i] = GetRandom();
            }

            return result;
        }

        public virtual T GetRandom() {
            int max = size.Clamp(0, _buffer.Length);
            return _buffer[rng.Next(0, max)];
        }

    }



    public class PrioritizedBuffer<T> : Buffer<T> {
        public float[] tree;
        Func<T, float> GetPriority;
        public int TreeSize { get => tree.Length; }


        public PrioritizedBuffer(int maxSize, Func<T, float> GetPriority) : this(maxSize) {
            this.GetPriority = GetPriority;
        }

        public PrioritizedBuffer(int maxSize) : base(maxSize) {
            int powOf2 = -1;

            maxSize = maxSize * 2 - 1;
            while (maxSize > 0) {
                powOf2++;
                maxSize /= 2;
            }

            maxSize = 1;

            for (int i = 0; i < powOf2; i++) {
                maxSize *= 2;
            }


            tree = new float[2 * maxSize - 1];
        }

        public override void Add(T item) {
            if (GetPriority == null) {
                throw new InvalidOperationException("Priority buffer GetPriority is not set.  Please give the priority explicitly.");
            }
            Add(item, GetPriority(item));
        }
        public void Add(T item, float priority) {
            _buffer[_oldest] = item;

            int index = _oldest + (tree.Length - 1) / 2;
            float change = priority - tree[index];

            tree[index] = priority;
            PropagateUp(index, change);


            _oldest = (_oldest + 1) % maxSize;
            size++;
        }

        void PropagateUp (int index, float change) {
            int parent = (index - 1) / 2;

            tree[parent] += change;
            if (parent != 0) {
                PropagateUp(parent, change);
            }
        }



        public override T GetRandom() {
            float dart = (float)rng.NextDouble() * tree[0];

            int treeIndex = Retrieve(0, dart);
            int dataIndex = treeIndex - (TreeSize - 1) / 2;

            dataIndex = dataIndex.Clamp(0, maxSize);

            return _buffer[dataIndex];
        }

        int Retrieve(int index, float dart) {
            int left = 2 * index + 1;
            int right = left + 1;


            int maxIndex = (TreeSize - 1) / 2 + maxSize;
            if (left >= maxIndex) {
                return index;
            }

            if (right >= maxIndex) {
                return left;
            }


            if (dart < tree[left]) {
                return Retrieve(left, dart);
            }
            else {
                return Retrieve(right, dart - tree[left]);
            }
        }

    }

}