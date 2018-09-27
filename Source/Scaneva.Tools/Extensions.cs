#region Copyright (C)
// ---------------------------------------------------------------------------------------------------------------
//  <copyright file="Extensions.cs" company="Scaneva">
// 
//  Copyright (C) 2018 Roche Diabetes Care GmbH (Christoph Pieper)
// 
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program. If not, see http://www.gnu.org/licenses/.
//  </copyright>
//  <summary>
//  Url: https://github.com/Scaneva
//  </summary>
// --------------------------------------------------------------------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.ComponentModel;
using System.IO;

namespace Scaneva.Tools
{
    public static class Extensions
    {
        public static string getHelpText(this Type t)
        {
            // Check if Helpfile exists
            if (File.Exists(Path.Combine(Properties.Settings.Default.HelpFolder, t.Name + ".rtf")))
            {
                return File.ReadAllText(Path.Combine(Properties.Settings.Default.HelpFolder, t.Name + ".rtf"));
            }
            return null;
        }

        public static string getDescription(this Type t)
        {
            // check if DescriptionAttribute is present
            var att = t.GetCustomAttributes(typeof(DescriptionAttribute), true).FirstOrDefault() as DescriptionAttribute;
            if (att != null)
            {
                return att.Description;
            }
            return null;
        }

        public static string toHexString(this byte[] ba)
        {
            if (ba == null)
            {
                return null;
            }

            StringBuilder hex = new StringBuilder(ba.Length * 2);
            hex.Append("[");
            foreach (byte b in ba)
            {
                if (hex.Length > 1)
                {
                    hex.Append(", ");
                }
                hex.AppendFormat("{0:x2}", b);
            }
            hex.Append("]");

            return hex.ToString();
        }

        public static string toExtendedString(this object message)
        {
            return toExtendedString2(message, "");
        }

        private static string toExtendedString2(object message, string prefix)
        {
            string retStr = "";

            if (message != null)
            {
                Type type = message.GetType();

                retStr += type.ToString();

                if (type.GetFields().Count() > 0)
                {
                    retStr += ": ";
                }

                // Iterate all Fields in Class or Struct
                // Decode each Field
                int count = 0;
                foreach (FieldInfo fi in type.GetFields())
                {
                    if (count > 0)
                    {
                        retStr += ", ";
                    }

                    object value = fi.GetValue(message);

                    if (value != null)
                    {
                        if ((fi.FieldType.IsArray) && (fi.FieldType.GetElementType().Equals(typeof(byte))))
                        {
                            retStr += prefix + fi.Name + " = " + ((byte[])fi.GetValue(message)).toHexString();
                        }
                        else if ((fi.FieldType.IsClass) && (!fi.FieldType.IsPrimitive) && (!fi.FieldType.IsEnum) && (!fi.FieldType.Equals(typeof(string))) && (fi.FieldType.GetFields().Count() > 0))
                        {
                            retStr += toExtendedString2(fi.GetValue(message), prefix + fi.Name + ".");
                        }
                        else
                        {
                            retStr += prefix + fi.Name + " = " + fi.GetValue(message).ToString();
                        }
                    }
                    else
                    {
                        retStr += "null";
                    }
                    count++;
                }

                // Iterate all Properties
                List<PropertyInfo> pis = type.GetProperties().Where(pi => !pi.GetCustomAttributes<BrowsableAttribute>().Contains(BrowsableAttribute.No)).ToList();
                foreach (PropertyInfo fi in pis)
                {
                    if (count > 0)
                    {
                        retStr += ", ";
                    }

                    object value = fi.GetValue(message);

                    if (value != null)
                    {
                        if ((fi.PropertyType.IsArray) && (fi.PropertyType.GetElementType().Equals(typeof(byte))))
                        {
                            retStr += prefix + fi.Name + " = " + ((byte[])fi.GetValue(message)).toHexString();
                        }
                        else if ((fi.PropertyType.IsClass) && (!fi.PropertyType.IsPrimitive) && (!fi.PropertyType.IsEnum) && (!fi.PropertyType.Equals(typeof(string))) && ((fi.PropertyType.GetFields().Count() > 0) || (fi.PropertyType.GetProperties().Count() > 0)))
                        {
                            retStr += toExtendedString2(fi.GetValue(message), prefix + fi.Name + ".");
                        }
                        else
                        {
                            retStr += prefix + fi.Name + " = " + fi.GetValue(message).ToString();
                        }
                    }
                    else
                    {
                        retStr += "null";
                    }
                    count++;
                }


            }

            return retStr;
        }

        public static T[,] To2DArray<T>(this IList<IList<T>> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            int max = source.Select(l => l).Max(l => l.Count());

            var result = new T[source.Count, max];

            for (int i = 0; i < source.Count; i++)
            {
                for (int j = 0; j < source[i].Count(); j++)
                {
                    result[i, j] = source[i][j];
                }
            }

            return result;
        }
    }
}
