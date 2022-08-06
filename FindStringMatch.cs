namespace AzUtil.Core
{
    public class FindStringMatch
    {
        public FindStringMatch(int startPos, int endPos, bool isStartOfSentence = false, bool isStartOfWord = false, int matchScore = 0, bool isCompleteWordMatch = false)
        {
            StartPos = startPos;
            EndPos = endPos;
            IsStartOfWord = isStartOfWord;
            MatchScore = matchScore;
            IsStartOfSentence = isStartOfSentence;
            IsCompleteWordMatch = isCompleteWordMatch;
        }
        public int StartPos { get; }
        public int EndPos { get; }
        public bool IsStartOfSentence { get; }
        public bool IsStartOfWord { get;  }
        public bool IsCompleteWordMatch { get; }
        public int MatchScore { get; set; }
    }
}
