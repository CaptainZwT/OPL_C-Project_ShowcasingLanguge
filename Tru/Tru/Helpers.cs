using System.Collections;

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
    }
}