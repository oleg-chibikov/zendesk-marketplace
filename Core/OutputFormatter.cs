using System.Collections;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using OlegChibikov.ZendeskInterview.Marketplace.Contracts;

[assembly: InternalsVisibleTo("OlegChibikov.ZendeskInterview.Marketplace.Tests")]

namespace OlegChibikov.ZendeskInterview.Marketplace.Core
{
    public sealed class OutputFormatter : IOutputFormatter
    {
        internal const string NoData = "No Data";
        const int LeftColumnWidth = 30;
        const int Indentation = 3;

        public string ListProperties(object? obj, int prefixSpaces = 0)
        {
            if (obj == null)
            {
                return NoData;
            }

            var props = obj.GetType().GetProperties();
            var sb = new StringBuilder();
            foreach (var p in props.OrderBy(PropertyInfoExtensions.IsCustomType))
            {
                sb.AppendLine();
                var spacesNeeded = LeftColumnWidth - p.Name.Length - 1;
                if (prefixSpaces > 0)
                {
                    sb.Append(new string(' ', prefixSpaces));
                }

                sb.Append(p.Name).Append(':');

                // TODO: This can be optimized by storing a dictionary of Property getters in memory for each possible object type that we need to display
                var value = p.GetValue(obj, null);
                var isCustomType = p.IsCustomType();

                if (!isCustomType || value == null)
                {
                    if (spacesNeeded > 0)
                    {
                        sb.Append(new string(' ', spacesNeeded));
                    }
                }
                else
                {
                    sb.AppendLine();
                }

                if (value != null && p.IsCollection())
                {
                    var enumerable = (IEnumerable)value;
                    value = string.Join(", ", enumerable.Cast<string>());
                }

                if (isCustomType)
                {
                    // Recursive call for the related entity
                    sb.Append(ListProperties(value, prefixSpaces + Indentation));
                    sb.AppendLine();
                }
                else
                {
                    if (value == null || (value is string str && string.IsNullOrWhiteSpace(str)))
                    {
                        sb.Append(NoData);
                    }
                    else
                    {
                        sb.Append(value);
                    }
                }
            }

            return sb.ToString();
        }
    }
}