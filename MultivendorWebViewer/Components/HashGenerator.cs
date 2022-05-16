using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MultivendorWebViewer.Components
{
    public static class HashHelper
    {
        public const int HashSeed = 13;

        public const int HashMultiplier = 29;

        //private static int GetHashCodeInternal(int key1, int key2)
        //{
        //    unchecked
        //    {
        //        var num = 0x7e53a269;
        //        num = (-1521134295 * num) + key1;
        //        num += (num << 10);
        //        num ^= (num >> 6);

        //        num = ((-1521134295 * num) + key2);
        //        num += (num << 10);
        //        num ^= (num >> 6);

        //        return num;
        //    }
        //}

        public static int GetHashCode<T>(IList<T> objects)
        {
            unchecked
            {
                int hash = HashSeed;
                object obj = objects[0];
                
                if (obj != null)
                {
                    hash = hash + obj.GetHashCode();
                }

                for (int index = 1; index < objects.Count; index++)
                {
                    obj = objects[index];
                    if (obj != null)
                    {
                        hash = (hash * HashMultiplier) + obj.GetHashCode();
                    }
                }
                return hash;
            }
        }

        public static int GetHashCode<T>(ICollection<T> objects)
        {
            unchecked
            {
                int hash = HashSeed;
                var enumerator = objects.GetEnumerator();
                if (enumerator.MoveNext() == true)
                {
                    var obj = enumerator.Current;
                    if (obj != null)
                    {
                        hash = hash + obj.GetHashCode();
                    }
                }

                while (enumerator.MoveNext() == true)
                {
                    var obj = enumerator.Current;
                    if (obj != null)
                    {
                        hash = (hash * HashMultiplier) + obj.GetHashCode();
                    }
                }

                return hash;
            }
        }

        public static int GetHashCodeNoNulls<T>(IList<T> objects)
        {
            unchecked
            {
                int hash = HashSeed;
                T obj = objects[0];
                hash = hash + obj.GetHashCode();

                for (int index = 1, count = objects.Count; index < count; index++)
                {
                    obj = objects[index];
                    hash = (hash * HashMultiplier) + obj.GetHashCode();
                }
                return hash;
            }
        }

        public static int GetHashCode<T1, T2>(T1 obj1, T2 obj2)
        {
            unchecked
            {
                int hash = HashSeed;
                if (obj1 != null) hash = hash + obj1.GetHashCode();
                if (obj2 != null) hash = (hash * HashMultiplier) + obj2.GetHashCode();
                return hash;
            }
        }

        public static int GetHashCode<T1, T2, T3>(T1 obj1, T2 obj2, T3 obj3)
        {
            unchecked
            {
                int hash = HashSeed;
                if (obj1 != null) hash = hash + obj1.GetHashCode();
                if (obj2 != null) hash = (hash * HashMultiplier) + obj2.GetHashCode();
                if (obj3 != null) hash = (hash * HashMultiplier) + obj3.GetHashCode();
                return hash;
            }
        }

        public static int GetHashCode<T1, T2, T3, T4>(T1 obj1, T2 obj2, T3 obj3, T4 obj4)
        {
            unchecked
            {
                int hash = HashSeed;
                if (obj1 != null) hash = hash + obj1.GetHashCode();
                if (obj2 != null) hash = (hash * HashMultiplier) + obj2.GetHashCode();
                if (obj3 != null) hash = (hash * HashMultiplier) + obj3.GetHashCode();
                if (obj4 != null) hash = (hash * HashMultiplier) + obj4.GetHashCode();
                return hash;
            }
        }

        public static int GetHashCode<T1, T2, T3, T4, T5>(T1 obj1, T2 obj2, T3 obj3, T4 obj4, T5 obj5)
        {
            unchecked
            {
                int hash = HashSeed;
                if (obj1 != null) hash = hash + obj1.GetHashCode();
                if (obj2 != null) hash = (hash * HashMultiplier) + obj2.GetHashCode();
                if (obj3 != null) hash = (hash * HashMultiplier) + obj3.GetHashCode();
                if (obj4 != null) hash = (hash * HashMultiplier) + obj4.GetHashCode();
                if (obj5 != null) hash = (hash * HashMultiplier) + obj5.GetHashCode();
                return hash;
            }
        }

        public static int GetHashCode<T1, T2, T3, T4, T5, T6>(T1 obj1, T2 obj2, T3 obj3, T4 obj4, T5 obj5, T6 obj6)
        {
            unchecked
            {
                int hash = HashSeed;
                if (obj1 != null) hash = hash + obj1.GetHashCode();
                if (obj2 != null) hash = (hash * HashMultiplier) + obj2.GetHashCode();
                if (obj3 != null) hash = (hash * HashMultiplier) + obj3.GetHashCode();
                if (obj4 != null) hash = (hash * HashMultiplier) + obj4.GetHashCode();
                if (obj5 != null) hash = (hash * HashMultiplier) + obj5.GetHashCode();
                if (obj6 != null) hash = (hash * HashMultiplier) + obj6.GetHashCode();
                return hash;
            }
        }

        public static int GetHashCode<T1, T2, T3, T4, T5, T6, T7>(T1 obj1, T2 obj2, T3 obj3, T4 obj4, T5 obj5, T6 obj6, T7 obj7)
        {
            unchecked
            {
                int hash = HashSeed;
                if (obj1 != null) hash = hash + obj1.GetHashCode();
                if (obj2 != null) hash = (hash * HashMultiplier) + obj2.GetHashCode();
                if (obj3 != null) hash = (hash * HashMultiplier) + obj3.GetHashCode();
                if (obj4 != null) hash = (hash * HashMultiplier) + obj4.GetHashCode();
                if (obj5 != null) hash = (hash * HashMultiplier) + obj5.GetHashCode();
                if (obj6 != null) hash = (hash * HashMultiplier) + obj6.GetHashCode();
                if (obj7 != null) hash = (hash * HashMultiplier) + obj7.GetHashCode();
                return hash;
            }
        }

        public static int GetHashCode<T1, T2, T3, T4, T5, T6, T7, T8>(T1 obj1, T2 obj2, T3 obj3, T4 obj4, T5 obj5, T6 obj6, T7 obj7, T8 obj8)
        {
            unchecked
            {
                int hash = HashSeed;
                if (obj1 != null) hash = hash + obj1.GetHashCode();
                if (obj2 != null) hash = (hash * HashMultiplier) + obj2.GetHashCode();
                if (obj3 != null) hash = (hash * HashMultiplier) + obj3.GetHashCode();
                if (obj4 != null) hash = (hash * HashMultiplier) + obj4.GetHashCode();
                if (obj5 != null) hash = (hash * HashMultiplier) + obj5.GetHashCode();
                if (obj6 != null) hash = (hash * HashMultiplier) + obj6.GetHashCode();
                if (obj7 != null) hash = (hash * HashMultiplier) + obj7.GetHashCode();
                if (obj8 != null) hash = (hash * HashMultiplier) + obj8.GetHashCode();
                return hash;
            }
        }

        public static int GetHashCodeNoNulls<T1, T2>(T1 obj1, T2 obj2)
        {
            unchecked
            {
                int hash = HashSeed + obj1.GetHashCode();
                hash = (hash * HashMultiplier) + obj2.GetHashCode();
                return hash;
            }
        }

        public static int GetHashCodeNoNulls<T1, T2, T3>(T1 obj1, T2 obj2, T3 obj3)
        {
            unchecked
            {
                int hash = HashSeed + obj1.GetHashCode();
                hash = (hash * HashMultiplier) + obj2.GetHashCode();
                hash = (hash * HashMultiplier) + obj3.GetHashCode();
                return hash;
            }
        }

        public static int GetHashCodeNoNulls<T1, T2, T3, T4>(T1 obj1, T2 obj2, T3 obj3, T4 obj4)
        {
            unchecked
            {
                int hash = HashSeed + obj1.GetHashCode();
                hash = (hash * HashMultiplier) + obj2.GetHashCode();
                hash = (hash * HashMultiplier) + obj3.GetHashCode();
                hash = (hash * HashMultiplier) + obj4.GetHashCode();
                return hash;
            }
        }

        public static int GetHashCodeNoNulls<T1, T2, T3, T4, T5, T6>(T1 obj1, T2 obj2, T3 obj3, T4 obj4, T5 obj5, T6 obj6)
        {
            unchecked
            {
                int hash = HashSeed + obj1.GetHashCode();
                hash = (hash * HashMultiplier) + obj2.GetHashCode();
                hash = (hash * HashMultiplier) + obj3.GetHashCode();
                hash = (hash * HashMultiplier) + obj4.GetHashCode();
                hash = (hash * HashMultiplier) + obj5.GetHashCode();
                hash = (hash * HashMultiplier) + obj6.GetHashCode();
                return hash;
            }
        }

        public static int CombineHashCode(int hash1, int hash2)
        {
            unchecked
            {
                int hash = HashSeed;
                hash = hash + hash1;
                hash = (hash * HashMultiplier) + hash2;
                return hash;
            }
        }

        public static int CombineHashCode(int hash1, int hash2, int hash3)
        {
            unchecked
            {
                int hash = HashSeed;
                hash = hash + hash1;
                hash = (hash * HashMultiplier) + hash2;
                hash = (hash * HashMultiplier) + hash3;
                return hash;
            }
        }

        public static int CombineHashCode(int hash1, int hash2, int hash3, int hash4)
        {
            unchecked
            {
                int hash = HashSeed;
                hash = hash + hash1;
                hash = (hash * HashMultiplier) + hash2;
                hash = (hash * HashMultiplier) + hash3;
                hash = (hash * HashMultiplier) + hash4;
                return hash;
            }
        }

        public static int CombineHashCode(int hash1, int hash2, int hash3, int hash4, int hash5)
        {
            unchecked
            {
                int hash = HashSeed;
                hash = hash + hash1;
                hash = (hash * HashMultiplier) + hash2;
                hash = (hash * HashMultiplier) + hash3;
                hash = (hash * HashMultiplier) + hash4;
                hash = (hash * HashMultiplier) + hash5;
                return hash;
            }
        }

        public static int CombineHashCode(int hash1, int hash2, int hash3, int hash4, int hash5, int hash6)
        {
            unchecked
            {
                int hash = HashSeed;
                hash = hash + hash1;
                hash = (hash * HashMultiplier) + hash2;
                hash = (hash * HashMultiplier) + hash3;
                hash = (hash * HashMultiplier) + hash4;
                hash = (hash * HashMultiplier) + hash5;
                hash = (hash * HashMultiplier) + hash6;
                return hash;
            }
        }

        public static int CombineHashCode(int hash1, int hash2, int hash3, int hash4, int hash5, int hash6, int hash7)
        {
            unchecked
            {
                int hash = HashSeed;
                hash = hash + hash1;
                hash = (hash * HashMultiplier) + hash2;
                hash = (hash * HashMultiplier) + hash3;
                hash = (hash * HashMultiplier) + hash4;
                hash = (hash * HashMultiplier) + hash5;
                hash = (hash * HashMultiplier) + hash6;
                hash = (hash * HashMultiplier) + hash7;
                return hash;
            }
        }

        public static int CombineHashCode(int hash1, int hash2, int hash3, int hash4, int hash5, int hash6, int hash7, int hash8)
        {
            unchecked
            {
                int hash = HashSeed;
                hash = hash + hash1;
                hash = (hash * HashMultiplier) + hash2;
                hash = (hash * HashMultiplier) + hash3;
                hash = (hash * HashMultiplier) + hash4;
                hash = (hash * HashMultiplier) + hash5;
                hash = (hash * HashMultiplier) + hash6;
                hash = (hash * HashMultiplier) + hash7;
                hash = (hash * HashMultiplier) + hash8;
                return hash;
            }
        }

        public static int CombineHashCode(int hash1, int hash2, int hash3, int hash4, int hash5, int hash6, int hash7, int hash8, int hash9)
        {
            unchecked
            {
                int hash = HashSeed;
                hash = hash + hash1;
                hash = (hash * HashMultiplier) + hash2;
                hash = (hash * HashMultiplier) + hash3;
                hash = (hash * HashMultiplier) + hash4;
                hash = (hash * HashMultiplier) + hash5;
                hash = (hash * HashMultiplier) + hash6;
                hash = (hash * HashMultiplier) + hash7;
                hash = (hash * HashMultiplier) + hash8;
                hash = (hash * HashMultiplier) + hash9;
                return hash;
            }
        }

        public static int CombineHashCode(int hash1, int hash2, int hash3, int hash4, int hash5, int hash6, int hash7, int hash8, int hash9, int hash10)
        {
            unchecked
            {
                int hash = HashSeed;
                hash = hash + hash1;
                hash = (hash * HashMultiplier) + hash2;
                hash = (hash * HashMultiplier) + hash3;
                hash = (hash * HashMultiplier) + hash4;
                hash = (hash * HashMultiplier) + hash5;
                hash = (hash * HashMultiplier) + hash6;
                hash = (hash * HashMultiplier) + hash7;
                hash = (hash * HashMultiplier) + hash8;
                hash = (hash * HashMultiplier) + hash9;
                hash = (hash * HashMultiplier) + hash10;
                return hash;
            }
        }

    }
}
