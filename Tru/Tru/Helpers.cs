namespace Utility {
    public static class Helpers
    {
        /// Returns true if all the objects in the 2 arrays are equal to each other.
        public static bool ArrayEquals<T>(T[] a, T[] b)
        {
            if (a.Length == b.Length) {
                for (int i = 0; i < a.Length; i++) {
                    if ( !a[i].Equals(b[i]) ) {
                        return false;
                    }
                }
                return true;
            };
            return false;
        }

        public delegate U Conversion<T, U>(T elem);

        /// Returns a new array by passing each element of array to conv.
        public static U[] ArrayMap<T, U>(T[] array, Conversion<T, U> conv) {
            U[] newArray = new U[array.Length];
            for (int i = 0; i < array.Length; i++) {
                newArray[i] = conv(array[i]);
            }
            return newArray;
        }
    }
}