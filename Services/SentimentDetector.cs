namespace CybersecurityAwarenessBotPart2.Services
{
    /// <summary>
    /// Detects the user's mood from keywords so the bot can adjust its tone (Part 2).
    /// </summary>
    public class SentimentDetector
    {
        public string DetectSentiment(string input)
        {
            string text = input.ToLower();

            if (ContainsAny(text, "worried", "scared", "afraid", "anxious", "nervous",
                                  "concerned", "unsafe", "sad"))
                return "worried";

            if (ContainsAny(text, "frustrated", "annoyed", "confused", "angry",
                                  "don't understand", "dont understand", "overwhelmed"))
                return "frustrated";

            if (ContainsAny(text, "curious", "wondering", "want to learn", "want to know"))
                return "curious";

            if (ContainsAny(text, "happy", "great", "awesome", "excited", "thank"))
                return "positive";

            return "neutral";
        }

        private static bool ContainsAny(string text, params string[] words)
        {
            foreach (string word in words)
            {
                if (text.Contains(word)) return true;
            }
            return false;
        }
    }
}
