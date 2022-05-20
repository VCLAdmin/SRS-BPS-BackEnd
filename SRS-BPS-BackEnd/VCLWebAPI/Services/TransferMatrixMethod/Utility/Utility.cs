using System.ComponentModel;
using System.Text;

namespace VCLWebAPI.Services.TransferMatrixMethod.Utility
{
    public static class Utility
    {
        public static string ToString<T>(T obj)
        {
            PropertyDescriptorCollection coll = TypeDescriptor.GetProperties(obj);
            StringBuilder builder = new StringBuilder();
            builder.Append("{ ");
            foreach (PropertyDescriptor pd in coll)
            {
                builder.Append(string.Format("{0}:{1} ", pd.Name, pd.GetValue(obj).ToString()));
            }
            builder.Append("}");
            return builder.ToString();
        }

        public static string ToString<T>(T[] objs)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("[ ");
            foreach (var obj in objs)
            {
                builder.Append(ToString<T>(obj) + " ");
            }
            builder.Append("]");
            return builder.ToString();
        }
    }
}