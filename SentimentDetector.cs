namespace CybersecurityAwarenessBotPart2
{
    public class SentimentDetector
    {
        public string DetectSentiment(string input)
        {
            input = input.ToLower();

            if (input.Contains("sad") ||
                input.Contains("worried"))
            {
                return "negative";
            }

            if (input.Contains("happy") ||
                input.Contains("great"))
            {
                return "positive";
            }

            return "neutral";
        }
    }
}