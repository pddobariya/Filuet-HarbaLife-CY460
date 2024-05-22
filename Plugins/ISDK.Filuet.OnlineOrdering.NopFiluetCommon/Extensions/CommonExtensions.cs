using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Extensions
{
    //common miscellaneous extension methods
    public static class CommonExtensions
    {
        #region Methods

        public static T GetAttributeFrom<T>(this object instance, string propertyName) where T : Attribute
        {
            return instance.GetType().GetAttributeFrom<T>(propertyName);
        }

        public static T GetAttributeFrom<T>(this Type type, string propertyName) where T : Attribute
        {
            try
            {
                Type attrType = typeof(T);
                PropertyInfo property = type.GetProperty(propertyName);
                if (property != null)
                {
                    return (T)property.GetCustomAttributes(attrType, false).First();
                }
                FieldInfo field = type.GetField(propertyName);
                return field == null ? null : (T)field.GetCustomAttributes(attrType, false).First();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static T GetEnumAttribute<T>(this Enum enumValue)
            where T : Attribute
        {
            return enumValue.GetType()
                            .GetMember(enumValue.ToString())
                            .First()
                            .GetCustomAttribute<T>();
        }

        public static string GetEnumDisplayAttributeName(this Enum enumValue) 
        {
            DisplayAttribute attr = enumValue.GetEnumAttribute<DisplayAttribute>();
            return attr != null ? attr.Name : null;
        }

        public static string GetEnumDisplayAttributeDescription(this Enum enumValue)
        {
            DisplayAttribute attr = enumValue.GetEnumAttribute<DisplayAttribute>();
            return attr != null ? attr.Description : null;
        }

        public static string GetEnumDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes = fi.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];

            if (attributes != null && attributes.Any())
            {
                return attributes.First().Description;
            }

            return value.ToString();
        }

        #endregion
    }
}