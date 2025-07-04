using System.Text.Json;
using static System.Reflection.BindingFlags;
using static System.Text.Json.JsonSerializer;

namespace Net10Auth.Shared.Infrastructure.Extensions;

public static class ObjectExtensions
{
    public static string ConvertToJson(this object objectToSerialize)
    {
        ArgumentNullException.ThrowIfNull(objectToSerialize);

        return Serialize(objectToSerialize, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
    }
    
    public static List<string> GetEnumValues<T>(string? prefix = null, string? suffix = null) where T 
        : Enum => (from object? value in Enum.GetValues(typeof(T)) select $"{prefix}{Enum.GetName(typeof(T), value)}{suffix}").ToList();

    public static List<string> GetBooleanPropertiesWithSuffix<T>(string suffix) =>
        typeof(T).GetProperties(Public | Instance)
            .Where(p => p.PropertyType == typeof(bool?) && p.Name.EndsWith(suffix))
            .Select(p => p.Name)
            .ToList();
    
    public static List<(string name, bool value)> GetBooleanProperties(this object instanceClass, string? withSuffix = null)
    {
        if (instanceClass == null) throw new ArgumentNullException(nameof(instanceClass), "The object cannot be null.");
        var type = instanceClass.GetType();
        var boolProps = withSuffix == null || withSuffix.IsNotNullOrWhiteSpace()
            ? type.GetProperties( NonPublic | Public  | Instance)
                .Where(p => p.PropertyType == typeof(bool))
            : type.GetProperties(NonPublic| Public | Instance)
                .Where(p => p.PropertyType == typeof(bool) && p.Name.EndsWith(withSuffix));
        return (from property in boolProps let value = (bool)property.GetValue(instanceClass) 
            select (property.Name, value)).ToList();
    }
    
    
    public static void SetBooleanProperties(this object instanceClass, Dictionary<string, bool>? dictionary, string? suffix = null)
    {
        if (instanceClass == null) throw new ArgumentNullException(nameof(instanceClass), "The object cannot be null.");
        var type = instanceClass.GetType();
        var boolProps = suffix == null || suffix.IsNotNullOrWhiteSpace()
            ? type.GetProperties(NonPublic| Public | Instance)
                .Where(p => p.PropertyType == typeof(bool))
            : type.GetProperties(NonPublic| Public | Instance)
                .Where(p => p.PropertyType == typeof(bool) && p.Name.EndsWith(suffix));
        foreach (var boolProp in boolProps)
            if (dictionary != null && dictionary.ContainsKey(boolProp.Name)) 
                    boolProp.SetValue(instanceClass, dictionary[boolProp.Name]);
    }
    
    public static void SetBooleanProperty(this object instanceClass, string name, string suffix, bool trueFalse = true)
    {
        var endsWith = $"{name}{suffix}";
        if (instanceClass == null) throw new ArgumentNullException(nameof(instanceClass), "The object cannot be null.");
        var type = instanceClass.GetType();
        var boolProps = type
            .GetProperties(Public | Instance).FirstOrDefault(p =>
                (p.PropertyType == typeof(bool?) || p.PropertyType == typeof(bool)) && p.Name.EndsWith(endsWith));

        if (boolProps == null) return;
        boolProps.SetValue(instanceClass, trueFalse);
    }
}


