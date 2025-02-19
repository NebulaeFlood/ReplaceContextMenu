using System;
using System.Diagnostics;
using Verse;

namespace NoCrowdedContextMenu
{
    public struct FloatMenuKey : IEquatable<FloatMenuKey>, IExposable
    {
        private string _declaringTypeName;
        private string _methodName;
        private string _namespace;

        private int _hashCode;


        public FloatMenuKey(Type declaringType, string methodName)
        {
            _declaringTypeName = declaringType.Name;
            _methodName = methodName;
            _namespace = declaringType.Namespace ?? string.Empty;

            _hashCode = _declaringTypeName.GetHashCode() ^ _methodName.GetHashCode() ^ _namespace.GetHashCode();
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
                && _declaringTypeName == other._declaringTypeName
                && _methodName == other._methodName
                && _namespace == other._namespace;
        }

        public bool Equals(FloatMenuKey other)
        {
            return _declaringTypeName == other._declaringTypeName
                && _methodName == other._methodName
                && _namespace == other._namespace;
        }

        public void ExposeData()
        {
            Scribe_Values.Look(ref _declaringTypeName, "DeclaringType");
            Scribe_Values.Look(ref _methodName, "Method");
            Scribe_Values.Look(ref _namespace, "_namespace");
            Scribe_Values.Look(ref _hashCode, "HashCode");
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(_namespace))
            {
                return $"{_declaringTypeName}.{_methodName}";
            }
            else
            {
                return $"{_namespace}.{_declaringTypeName}.{_methodName}";
            }
        }
    }
}
