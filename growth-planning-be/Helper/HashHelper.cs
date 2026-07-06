using System.Security.Cryptography;
using System.Text;

namespace growth_planning_be.Helper;

public static class HashHelper
{
    public static string ConvertToMD5(string input)
    {
        var inputBytes = Encoding.UTF8.GetBytes(input);
        var hashBytes = MD5.HashData(inputBytes);

        var sb = new StringBuilder();
        foreach (var t in hashBytes)
        {
            sb.Append(t.ToString("X2"));
        }
        return sb.ToString();
    }
}