using System.Text.RegularExpressions;

namespace ServiceContracts.Helpers;
public static class ImageRemoveHelper
{
    public static string ExtractRootPublicIdFromUrl(string url)
    {
        var regex = new Regex(@"\/upload\/(?:v\d+\/)?([^\/]+?)(?:\.\w+)?$");
        var match = regex.Match(url);

        if (match.Success && match.Groups.Count > 1)
        {
            return match.Groups[1].Value;
        }

        return null;
    }
}