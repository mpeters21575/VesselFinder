using System;
using System.ComponentModel;
using System.Reflection;

namespace VesselFinder.Business.Mapper
{
    public interface IDescriptionAttributeMapper<in T> where T : class
    {
        void Map(T source, string text, string value);
    }
    
    public class DescriptionAttributeMapper<T> : IDescriptionAttributeMapper<T> where T : class
    {
        public void Map(T source, string text, string value)
        {
            if (string.IsNullOrWhiteSpace(text)) return;
            
            foreach (var property in source.GetType().GetProperties(BindingFlags.Public|BindingFlags.Instance))
            {
                var description = GetDescriptionFrom(property);
                
                if (!description.Equals(text)) continue;
                
                property.SetValue(source, Convert.ChangeType(value, property.PropertyType), null);
                break;
            }
        }

        private static string GetDescriptionFrom(ICustomAttributeProvider property)
        {
            var attribute = property.GetCustomAttributes(typeof(DescriptionAttribute), true)[0];
            var description = (DescriptionAttribute)attribute;
            
            return description.Description;
        }
    }
}