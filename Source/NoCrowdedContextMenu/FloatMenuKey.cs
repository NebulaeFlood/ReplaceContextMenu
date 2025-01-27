using System;
using System.Diagnostics;
using Verse;

namespace NoCrowdedContextMenu
{
    public struct FloatMenuKey : IEquatable<FloatMenuKey>, IExposable
    {
        internal string DeclaringTypeName;
        internal string MethodName;
        internal string Namespace;

        private int _hashCode;


        public FloatMenuKey(Type declaringType, string methodName)
        {
            DeclaringTypeName = declaringType.Name;
            MethodName = methodName;
            Namespace = declaringType.Namespace;

            _hashCode = DeclaringTypeName.GetHashCode() ^ MethodName.GetHashCode() ^ Namespace.GetHashCode();
        }


        public static bool TryCreateKey(out FloatMenuKey key)
        {
            if (new StackTrace().GetFrame(3) is StackFrame frame)
            {
                var method = frame.GetMethod();

                key = new FloatMenuKey(method.DeclaringType, method.Name);
                return true;
            }
            else
            {
                key = default;
                return false;
            }
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }

        public override bool Equals(object obj)
        {
            return obj is FloatMenuKey other
                && DeclaringTypeName == other.DeclaringTypeName
                && MethodName == other.MethodName;
        }

        public bool Equals(FloatMenuKey other)
        {
            return DeclaringTypeName == other.DeclaringTypeName
                && MethodName == other.MethodName;
        }

        public void ExposeData()
        {
            Scribe_Values.Look(ref DeclaringTypeName, "DeclaringType");
            Scribe_Values.Look(ref MethodName, "Method");
            Scribe_Values.Look(ref Namespace, "Namespace");
            Scribe_Values.Look(ref _hashCode, "HashCode");
        }
    }
}
