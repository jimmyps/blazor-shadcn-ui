using System.Text.Json;
using System.Text.Json.Serialization;

namespace BlazorUI.Components.Old.Chart;

/// <summary>
/// Wrapper for raw JavaScript code that should be emitted unquoted in JSON serialization.
/// Used for ECharts formatter functions and other JavaScript expressions.
/// 
/// WARNING: The code is executed in the browser. Only use with trusted, developer-provided code.
/// Never pass user input directly to JsRaw.
/// </summary>
public sealed class JsRaw
{
    /// <summary>
    /// Gets the raw JavaScript code.
    /// </summary>
    public string Code { get; }
    
    /// <summary>
    /// Creates a new instance of JsRaw with the specified JavaScript code.
    /// </summary>
    /// <param name="code">The JavaScript code (e.g., "function (params) { return params.value; }")</param>
    public JsRaw(string code)
    {
        Code = code ?? throw new ArgumentNullException(nameof(code));
    }
    
    /// <summary>
    /// Implicit conversion from string to JsRaw.
    /// </summary>
    public static implicit operator JsRaw(string code) => new(code);
    
    /// <summary>
    /// Returns the raw JavaScript code.
    /// </summary>
    public override string ToString() => Code;
}

/// <summary>
/// JSON converter for JsRaw that emits the JavaScript code without quotes.
/// </summary>
public sealed class JsRawJsonConverter : JsonConverter<JsRaw>
{
    public override JsRaw? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // JsRaw is write-only for serialization purposes
        throw new NotSupportedException("JsRaw deserialization is not supported.");
    }
    
    public override void Write(Utf8JsonWriter writer, JsRaw value, JsonSerializerOptions options)
    {
        if (value == null)
        {
            writer.WriteNullValue();
            return;
        }
        
        // Write the raw JavaScript code without quotes
        // This is done by writing it as a raw JSON value
        writer.WriteRawValue(value.Code);
    }
}
