using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using JetBrains.Annotations;

namespace Net10Auth.Shared.Infrastructure.Extensions;

public static class StringExtensions
{
    
    public static T ToType<T>(this string jsonString) where T : class =>
        jsonString switch
        {
            null => throw new ArgumentNullException($"ToType: You cannot convert a null string to a Type"),
            "[]" => default,
            _ => JsonSerializer.Deserialize<T>(jsonString,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
        } ?? throw new InvalidOperationException();

    public static string AddUrlParameters(this string url, IDictionary<string, object?> parameters)
    {
        if (string.IsNullOrEmpty(url)) throw new ArgumentNullException(nameof(url), "Base URL cannot be null or empty.");

        var separator = url.Contains('?') ? "&" : "?";

        var sb = new StringBuilder(url);
        foreach (KeyValuePair<string, object?> parameter in parameters)
        {
            parameter.Deconstruct(out var key, out var value);
            string name = key;
            if (value != null)
            {
                object value2 = value;
                sb.Append($"{separator}{key}={Uri.EscapeDataString(value.ToString() ?? "")}");
                if (separator == "?") separator = "&";
            }
        }

        return sb.ToString();
    }
    
    // Extension method by Simon Painter
    public static string? ValueOrDefault(this string? @this, string defaultValue)
        => @this.IsNullOrWhiteSpace() ? defaultValue : @this;
        
    // Extension method by Simon Painter
    public static int ValueOrDefault(this string? @this, int defaultValue)
        => @this.IsNullOrWhiteSpace() || !int.TryParse(@this, out var parsedValue) ? defaultValue : parsedValue;
        
    // Modified Extension method by the ABP Framework
    [ContractAnnotation("null => true")]
    public static bool IsNullOrEmpty(this string @this) => string.IsNullOrEmpty(@this);

    // Modified Extension method by the ABP Framework
    [ContractAnnotation("null => true")]
    public static bool IsNotNullOrEmpty(this string @this) => string.IsNullOrEmpty(@this);

    // Modified Extension method by the ABP Framework
    [ContractAnnotation("null => true")]
    public static bool IsNullOrWhiteSpace(this string? @this) => string.IsNullOrWhiteSpace(@this);
        
        
    // Modified Extension method by the ABP Framework
    [ContractAnnotation("null => true")]
    public static bool IsNotNullOrWhiteSpace(this string? @this) => !string.IsNullOrWhiteSpace(@this);
        
    // Extension method by Symon Painter
    public static IEnumerable<IEnumerable<string>> Parser (this string input, string lineSplit, string fieldSplit) =>
        input.Split(new[] { lineSplit }, StringSplitOptions.RemoveEmptyEntries)
            .Select(line => line.Split(new[] { fieldSplit }, StringSplitOptions.RemoveEmptyEntries));
        
        
    // Modified Extension method by the ABP Framework
    [ContractAnnotation("null <= this:null")]
    public static string? ToSentenceCase(this string? @this) 
        => string.IsNullOrWhiteSpace(@this) ? @this : Regex.Replace(@this, "[a-z][A-Z]", m => m.Value[0] + " " + char.ToLowerInvariant(m.Value[1]));
        
        
    public static bool IsValidEmailAddress(this string emailAddress)
    {
        if (emailAddress.IsNullOrWhiteSpace()) return false;
        return  Regex.IsMatch(emailAddress,
            @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z",
            RegexOptions.IgnoreCase);
    }
 
    
}