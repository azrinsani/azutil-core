namespace AzUtil.Core
{
    public struct FindStringResultPart
    {
        public FindStringResultPart(string text, bool highlight = false)
        {
            this.Highlight = highlight;
            this.Text = text;
        }

        public bool Highlight { get; set; } 
        public string Text { get; set; }
    }
}