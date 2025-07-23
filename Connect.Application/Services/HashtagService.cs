using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Connect.Application.Interfaces;

namespace Connect.Application.Service
{
    public class HashtagService : IHashtagService
    {
        public List<string> ExtractHashtags(string postText)
        {
            if (string.IsNullOrWhiteSpace(postText))
                return new List<string>();

            var hashtagPattern = new Regex(@"#\w+");
            var matches = hashtagPattern.Matches(postText)
                .Select(match => match.Value.TrimEnd('.', ',', '!', '?').ToLower())
                .Distinct()
                .ToList();

            return matches;
        }
    }
}