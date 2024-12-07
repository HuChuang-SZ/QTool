using System;
using System.Collections.Generic;
using System.Linq;

namespace QTool
{
    public class PropertyItem
    {
        public PropertyItem(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException(nameof(path));
            }

            if (path.Last() == ']')
            {
                var startIndex = path.LastIndexOf("[");
                Name = path.Substring(0, startIndex);

                startIndex += 1;
                var array = path.Substring(startIndex, path.Length - startIndex - 1).Split(',');
                var objs = new List<object>();
                foreach (var item in array)
                {
                    var val = item.Trim();
                    if (int.TryParse(val, out int i))
                    {
                        objs.Add(i);
                    }
                    else if (long.TryParse(val, out long l))
                    {
                        objs.Add(l);
                    }
                    else
                    {
                        objs.Add(val);
                    }
                }
                Index = objs.ToArray();
            }
            else
            {
                Name = path;
            }
        }

        public string Name { get; }

        public object[] Index { get; }

        public object GetValue(object o)
        {
            if (o == null)
                return null;
            else
            {
                var prop = o.GetType().GetProperty(Name);
                if (prop == null)
                    return null;
                else
                {
                    var val = prop.GetValue(o);
                    if (Index != null)
                    {
                        if (val != null)
                        {
                            var type = val.GetType();
                            try
                            {
                                return type.GetMethod("Get").Invoke(val, Index);
                            }
                            catch
                            {
                                return null;
                            }
                        }
                        else
                        {
                            return null;
                        }
                    }
                    else
                    {
                        return val;
                    }
                }
            }
        }
    }
}