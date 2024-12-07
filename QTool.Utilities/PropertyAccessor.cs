using System;

namespace QTool
{
    public class PropertyAccessor
    {
        private readonly PropertyItem[] _properties;
        public PropertyAccessor(string propertyPath)
        {
            if (string.IsNullOrWhiteSpace(propertyPath)) throw new ArgumentNullException(nameof(propertyPath));
            string[] array = propertyPath.Split('.');

            _properties = new PropertyItem[array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                _properties[i] = new PropertyItem(array[i]);
            }
        }

        public object GetValue(object obj)
        {
            var o = obj;
            foreach (var prop in _properties)
            {
                o = prop.GetValue(o);
            }

            return o;
        }
    }
}
