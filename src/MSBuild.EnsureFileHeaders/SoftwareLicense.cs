namespace JustinWritesCode.GitHub;
using System.Runtime.Serialization.Json;

using System;
using System.Collections.Generic;

using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

/// <summary>
/// License Content
/// </summary>
public partial class SoftwareLicense
{
    [JsonPropertyName("_links")]
    public Links? Links { get; set; }

    [JsonPropertyName("content")]
    public string? Content { get; set; }

    [JsonPropertyName("download_url")]
    public Uri? DownloadUrl { get; set; }

    [JsonPropertyName("encoding")]
    public string? Encoding { get; set; }

    [JsonPropertyName("git_url")]
    public Uri? GitUrl { get; set; }

    [JsonPropertyName("html_url")]
    public Uri? HtmlUrl { get; set; }

    [JsonPropertyName("license")]
    public LicenseSimple? License { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("path")]
    public string? Path { get; set; }

    [JsonPropertyName("sha")]
    public string? Sha { get; set; }

    [JsonPropertyName("size")]
    public long Size { get; set; }

    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("url")]
    public Uri? Url { get; set; }
}

/// <summary>
/// License Simple
/// </summary>
public partial class LicenseSimple
{
    [JsonPropertyName("html_url")]
    public Uri? HtmlUrl { get; set; }

    [JsonPropertyName("key")]
    public string? Key { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("node_id")]
    public string? NodeId { get; set; }

    [JsonPropertyName("spdx_id")]
    public string? SpdxId { get; set; }

    [JsonPropertyName("url")]
    public Uri? Url { get; set; }
}

public partial class Links
{
    [JsonPropertyName("git")]
    public Uri? Git { get; set; }

    [JsonPropertyName("html")]
    public Uri? Html { get; set; }

    [JsonPropertyName("self")]
    public Uri? Self { get; set; }
}

public partial class SoftwareLicense
{
    public static SoftwareLicense FromJson(string json) => JsonSerializer.Deserialize<SoftwareLicense>(json/*, JustinWritesCode.GitHub.Converter.Settings(*/)!;
}

public static class Serialize
{
    public static string ToJson(this SoftwareLicense self) => JsonSerializer.Serialize(self/*, JustinWritesCode.GitHub.Converter.Settings*/);
}

// internal static class Converter
// {
//     public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
//     {
//         MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
//         DateParseHandling = DateParseHandling.None,
//         Converters =
//         {
//             new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
//         },
//     };
// }
