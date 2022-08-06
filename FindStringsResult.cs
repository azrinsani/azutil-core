namespace AzUtil.Core
{
    public class FindStringsResult
    {
        public FindStringsResult(bool success, FindStringMatch[] matches, FindStringResultPart[] parts)
        {
            Success = success;
            Matches = matches;
            Parts = parts;
            foreach (var match in Matches)
            {
                MatchScore += match.MatchScore;
            }
        }
        public bool Success { get; }
        public int MatchScore { get;  }
        public FindStringResultPart[] Parts { get; }
        public FindStringMatch[] Matches { get; }        
    }
}