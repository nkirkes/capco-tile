using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Reflection;
using System.ComponentModel;

namespace System.Web.Mvc
{
    public static class EnumExtensions
    {
        public static SelectList ToSelectList(this Enum enumeration)
        {
            return enumeration.ToSelectList(false, true);
        }
        
        public static SelectList ToSelectList(this Enum enumeration, bool useSelectedValues, bool sort = true)
        {
            var values = new Dictionary<string, string>();
            foreach (Enum e in Enum.GetValues(enumeration.GetType()))
            {
                values.Add(e.ToString(), e.ToFriendlyString());
            }
            values = sort ? values.OrderBy(x => x.Value).ToDictionary(k => k.Key, v => v.Value) : values;
            SelectList list;
            if (useSelectedValues)
            {
                list = new SelectList(values, "key", "value", values[enumeration.ToString()]);
            }
            else
            {
                list = new SelectList(values, "key", "value");
            }
            return list;
        }

        public static string ToFriendlyString(this Enum value)
        {
            // Get the Description attribute value for the enum value
            if (value != null)
            {
                string stringVal = value.ToString();
                if (!stringVal.Contains(','))
                {
                    FieldInfo fi = value.GetType().GetField(value.ToString());
                    DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
                    if (attributes.Length > 0)
                    {
                        return attributes[0].Description;
                    }
                    else
                    {
                        return value.ToString();
                    }
                }
                else
                {
                    return "Null";
                }
            }
            return null;
        }
    }
}