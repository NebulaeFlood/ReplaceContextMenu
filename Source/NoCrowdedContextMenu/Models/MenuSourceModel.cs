using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace NoCrowdedContextMenu.Models
{
    public struct MenuSourceModel : IEquatable<MenuSourceModel>, IExposable
    {
        public MenuSourceModel(Type declaringType, string methodName)
        {
            _declaringTypeName = declaringType.Name;
            _methodName = methodName;
            _namespace = declaringType.Namespace ?? string.Empty;

            _hashCode = _declaringTypeName.GetHashCode() ^ _methodName.GetHashCode() ^ _namespace.GetHashCode();
        }


        public static bool TryCreate(out MenuSourceModel key)
        {
            if (new StackTrace().GetFrame(4) is StackFrame frame)
            {
                var method = frame.GetMethod();

                key = new MenuSourceModel(method.DeclaringType, method.Name);
                return true;
            }
            else
            {
                key = default;
                return false;
            }
        }


        //------------------------------------------------------
        //
        //  Public Methods
        //
        //------------------------------------------------------

        #region Public Methods

        public override int GetHashCode()
        {
            return _hashCode;
        }

        public override bool Equals(object obj)
        {
            return obj is MenuSourceModel other
                && _declaringTypeName == other._declaringTypeName
                && _methodName == other._methodName
                && _namespace == other._namespace;
        }

        public bool Equals(MenuSourceModel other)
        {
            return _declaringTypeName == other._declaringTypeName
                && _methodName == other._methodName
                && _namespace == other._namespace;
        }

        public void ExposeData()
        {
            Scribe_Values.Look(ref _declaringTypeName, "DeclaringType");
            Scribe_Values.Look(ref _methodName, "Method");
            Scribe_Values.Look(ref _namespace, "Namespace");
            Scribe_Values.Look(ref _hashCode, "HashCode");
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(_namespace))
            {
                return $"{_declaringTypeName}.{_methodName}";
            }

            return $"{_namespace}.{_declaringTypeName}.{_methodName}";
        }

        #endregion


        //------------------------------------------------------
        //
        //  Private Fields
        //
        //------------------------------------------------------

        #region Private Fields

        private string _declaringTypeName;
        private string _methodName;
        private string _namespace;

        private int _hashCode;

        #endregion
    }
}
