using System;
using System.Collections.Generic;
using System.Linq;
using QcmBackend.Application.Common.Interfaces;

namespace QcmBackend.Application.Common.Sorting
{
    public class SortableProperties<T> : IPropertiesConfigurator
    {
        private readonly Dictionary<string, List<string>> _roleBasedProperties = [];
        private readonly HashSet<string> _properties = new(StringComparer.OrdinalIgnoreCase);
        public Dictionary<string, (bool IsManualSort, List<string>? Roles)> AllProperties { get; } = new(StringComparer.OrdinalIgnoreCase);

        public SortableProperties<T> Add(string path, bool isManualSort = false, List<string>? roles = null)
        {
            AllProperties[path] = (isManualSort, roles);

            if (roles != null)
            {
                foreach (string role in roles)
                {
                    if (!_roleBasedProperties.TryGetValue(role, out List<string>? value))
                    {
                        value = [];
                        _roleBasedProperties[role] = value;
                    }

                    value.Add(path);
                }
            }
            else
            {
                _ = _properties.Add(path);
            }

            return this;
        }

        public IEnumerable<string> GetDescriptiveProperties()
        {
            return AllProperties.Select(p =>
            {
                string roles = p.Value.Roles != null ? $" (Roles: {string.Join(", ", p.Value.Roles)})" : "";

                return $"{p.Key}{roles}";
            });
        }

        public bool IsManualSort(string path)
        {
            if (AllProperties.TryGetValue(path, out (bool IsManualSort, List<string>? Roles) value))
            {
                return value.IsManualSort;
            }

            return false;
        }

        public HashSet<string> GetValidSorts(IEnumerable<string> sortables, IEnumerable<string> roles)
        {
            HashSet<string> allowedSorts = new(_properties, StringComparer.OrdinalIgnoreCase);

            foreach (string role in roles)
            {
                if (_roleBasedProperties.TryGetValue(role, out List<string>? properties))
                {
                    foreach (string property in properties)
                    {
                        _ = allowedSorts.Add(property);
                    }
                }
            }

            HashSet<string> requestedSorts = new(sortables, StringComparer.OrdinalIgnoreCase);

            return allowedSorts.Where(requestedSorts.Contains).ToHashSet(StringComparer.OrdinalIgnoreCase);
        }
    }
}
