using System;
using System.Text.RegularExpressions;

namespace growth_planning_be.Extensions;

public static partial class StringExtensions
{
    public static string ToSnakeCase(this string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return string.Empty;
        var result = WordSeparatorRegex().Replace(input, "_");
        result = CamelCaseBoundaryRegex().Replace(result, "$1_$2");

        return result.ToLower();
    }

    [GeneratedRegex(@"[\s\-]+")]
    private static partial Regex WordSeparatorRegex();

    [GeneratedRegex(@"([a-z])([A-Z])")]
    private static partial Regex CamelCaseBoundaryRegex();

    /// <summary>
    /// Chuyển chuỗi thành List<string> bằng cách tách theo ký tự phân cách.
    /// </summary>
    /// <param name="input">Chuỗi đầu vào</param>
    /// <param name="separator">Ký tự phân cách (ví dụ: ',', ';')</param>
    /// <returns>List<string> sau khi tách</returns>
    public static List<string> ToListByDelimiter(this string? input, char separator = ',')
    {
        if (string.IsNullOrEmpty(input) || string.IsNullOrWhiteSpace(input))
            return [];


        return input
            .Split(separator)
            .Select(item => item.Trim())
            .Where(item => !string.IsNullOrEmpty(item))
            .ToList();
    }

    /// <summary>
    /// Chuyển chuỗi thành List<int> bằng cách tách theo dấu phân cách.
    /// </summary>
    /// <param name="input">Chuỗi đầu vào, có thể null hoặc rỗng</param>
    /// <param name="separator">Ký tự phân cách, mặc định là ','</param>
    /// <returns>List<int> sau khi chuyển đổi thành công</returns>
    public static List<int> ToIntList(this string? input, char separator = ',')
    {
        if (input == null || string.IsNullOrWhiteSpace(input))
            return [];

        return input
            .Split(separator)
            .Select(item => {
                var ok = int.TryParse(item.Trim(), out int value);
                return new { ok, value };
            })
            .Where(x => x.ok)
            .Select(x => x.value)
            .ToList();
    }
}
