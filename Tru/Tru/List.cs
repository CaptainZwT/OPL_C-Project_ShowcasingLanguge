using System.Collections;

namespace Utility {
    public class List<T> : IEnumerable
    {
        private T[] _elements;
        public int Count {get; private set;}

        public List() {
            this._elements = new T[10];
            this.Count = 0;
        }

        public T this[int index] {
            get {
                if (index >= this.Count) {
                    throw new System.IndexOutOfRangeException();
                } else {
                    return this._elements[index];
                }
            }
            set {this._elements[index] = value;}
        }

        public void Add(T elem) {
            if (this.Count + 1 >= this._elements.Length) {
                T[] newArray = new T[ this.Count * 2 ];
                this._elements.CopyTo(newArray, 0);
                this._elements = newArray;
                System.Console.WriteLine("Resize");

            }

            this._elements[this.Count] = elem;
            this.Count += 1;
        }

        public T[] ToArray() {
            T[] newArray = new T[ this.Count ];
            for (int i = 0; i < this.Count; i++) {
                newArray[i] = this._elements[i];
            }
            return newArray;
        }

        // public override bool Equals(object obj)
        // {
        //     if (obj is List<T> list && this.Count == list.Count) {
        //         for (int i = 0; i < this.Count; i++) {
        //             if (!this._elements[i].Equals(list._elements[i])) return false;
        //         }
        //         return true;
        //     };
        //     return false;
        // }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this._elements.GetEnumerator();
        }
    }
}