namespace azutil_core
{
    public class FindStringMatch
    {
        public FindStringMatch(int startPos, int endPos, bool isStartOfSentence = false, bool isStartOfWord = false, int matchScore = 0)
        {
            StartPos = startPos;
            EndPos = endPos;
            IsStartOfWord = isStartOfWord;
            MatchScore = matchScore;
            IsStartOfSentence = isStartOfSentence;
        }
        public int StartPos { get; }
        public int EndPos { get; }
        public bool IsStartOfSentence { get; }
        public bool IsStartOfWord { get;  }
        public int MatchScore { get; set; }
    }
}
