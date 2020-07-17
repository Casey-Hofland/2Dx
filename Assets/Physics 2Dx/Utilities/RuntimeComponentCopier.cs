// Modified from an implementation by vexe.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Physics2DxSystem.Utilities
{
    public static class RuntimeComponentCopier
    {
        private const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Default | BindingFlags.DeclaredOnly;

        /// <summary>Gets a Copy of another Component</summary>
        /// <param name="comp">The Component to copy to.</param>
        /// <param name="other">The Component to copy from.</param>
        /// <param name="ignore">The properties to ignore when copying.</param>
        /// <returns>The new Component.</returns>
        public static T GetCopyOf<T>(this Component comp, T other, params string[] ignore) where T : Component
        {
            Type type = comp.GetType();
            if(type != other.GetType()) return null; // type mis-match

            Type derivedType = typeof(T);
            bool hasDerivedType = derivedType != type;

            IEnumerable<PropertyInfo> pinfos = type.GetProperties(bindingFlags);
            if(hasDerivedType)
            {
                pinfos = pinfos.Concat(derivedType.GetProperties(bindingFlags));
            }
            pinfos = from property in pinfos
                     where !property.CustomAttributes.Any(attribute => attribute.AttributeType == typeof(ObsoleteAttribute))
                     where Array.IndexOf(ignore, property.Name) == -1
                     select property;
            foreach(var pinfo in pinfos)
            {
                if(pinfo.CanWrite)
                {
                    if(pinfos.Any(e => e.Name == $"shared{char.ToUpper(pinfo.Name[0])}{pinfo.Name.Substring(1)}")) // Make sure that properties which have a "shared..." value are not copied to prevent leaking objects.
                    {
                        continue;
                    }

                    try
                    {
                        pinfo.SetValue(comp, pinfo.GetValue(other));
                    }
                    catch { } // In case of NotImplementedException being thrown. For some reason specifying that exception didn't seem to catch it, but not specifying anything does catch it.
                }
            }

            IEnumerable<FieldInfo> finfos = type.GetFields(bindingFlags);
            if(hasDerivedType)
            {
                finfos = finfos.Concat(derivedType.GetFields(bindingFlags));
            }
            finfos = from field in finfos
                     where field.CustomAttributes.Any(attribute => attribute.AttributeType == typeof(ObsoleteAttribute))
                     where !ignore.Contains(field.Name)
                     select field;
            foreach(var finfo in finfos)
            {
                if(finfos.Any(e => e.Name == $"shared{char.ToUpper(finfo.Name[0])}{finfo.Name.Substring(1)}")) // Make sure that fields which have a "shared..." value are not copied to prevent leaking objects.
                {
                    continue;
                }

                finfo.SetValue(comp, finfo.GetValue(other));
            }

            return comp as T;
        }

        /// <summary>Add a copy of a Component to a GameObject.</summary>
        /// <param name="toAdd">The Component to copy from.</param>
        /// <param name="ignore">The properties to ignore when copying.</param>
        /// <returns>The new Component.</returns>
        public static T AddComponent<T>(this GameObject go, T toAdd, params string[] ignore) where T : Component
        {
            return go.AddComponent(toAdd.GetType()).GetCopyOf(toAdd, ignore);
        }
    }
}

