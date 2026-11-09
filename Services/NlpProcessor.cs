using System.Text.RegularExpressions;

namespace CybersecurityAwarenessBotPart2.Services
{
    /// <summary>
    /// NLP simulation: cleans the message, looks for keywords/phrases and
    /// works out what the user wants (their "intent"). Different phrasings
    /// lead to the same action. No machine learning - just string processing and regex.
    /// </summary>
    public class NlpProcessor
    {
        public IntentResult DetectIntent(string rawInput)
        {
            string input = rawInput.Trim().ToLower();

            // Reminders are checked first so "remind me to check my tasks"
            // creates a reminder instead of opening the task list.
            if (ContainsAny(input, "remind me", "set a reminder", "add a reminder", "reminder to"))
            {
                return new IntentResult
                {
                    Intent = UserIntent.AddTaskWithReminder,
                    TaskTitle = ExtractTaskTitle(input),
                    ReminderDate = ExtractReminderDate(input)
                };
            }

            // "add a task", "create a new task", "make task", "new task" ...
            if (Regex.IsMatch(input, @"\b(add|create|make|new|save)\s+(a\s+)?(new\s+)?task\b"))
            {
                return new IntentResult
                {
                    Intent = UserIntent.AddTask,
                    TaskTitle = ExtractTaskTitle(input),
                    ReminderDate = ExtractReminderDate(input)
                };
            }

            if (ContainsAny(input, "activity log", "show activity", "my activity", "recent actions",
                                   "what have you done", "show log"))
            {
                return new IntentResult { Intent = UserIntent.ShowActivityLog };
            }

            if (ContainsAny(input, "quiz", "test my knowledge", "test me", "cybersecurity game",
                                   "mini-game", "mini game", "play a game"))
            {
                return new IntentResult { Intent = UserIntent.StartQuiz };
            }

            if (ContainsAny(input, "show tasks", "show my tasks", "view tasks", "list tasks", "my tasks",
                                   "what tasks", "open tasks", "pending tasks"))
            {
                return new IntentResult { Intent = UserIntent.ShowTasks };
            }

            // Not a Part 3 command - let the Part 2 chatbot answer it.
            return new IntentResult { Intent = UserIntent.CybersecurityQuestion };
        }

        /// <summary>
        /// Removes the command words and date words, leaving just the task.
        /// "Remind me to update my password tomorrow" -> "Update my password"
        /// </summary>
        public string ExtractTaskTitle(string input)
        {
            string title = input.Trim();

            // Remove the command at the start.
            title = Regex.Replace(title,
                @"^.*?\b" +   // skip words like "please" or "I need you to" before the command

                @"((add|create|make|save)\s+(a\s+)?(new\s+)?task|new\s+task|" +
                @"remind\s+me|set\s+a\s+reminder|add\s+a\s+reminder)" +
                @"(\s+(to|for|about))?\s*[-:]?\s*",
                "", RegexOptions.IgnoreCase);

            // Remove date wording at the end.
            title = Regex.Replace(title,
                @"\s+(tomorrow|today|next week|in a week|in\s+\d+\s+(days?|weeks?))\s*[.!]?$",
                "", RegexOptions.IgnoreCase);

            title = title.Trim().TrimEnd('.', '!', '?');

            if (title.Length == 0)
                return "";

            // Capitalise the first letter for a neat display.
            return char.ToUpper(title[0]) + title.Substring(1);
        }

        /// <summary>
        /// Understands simple date phrases: today, tomorrow, next week, in a week,
        /// in 3 days, in 2 weeks. Returns null if no date is mentioned.
        /// </summary>
        public DateTime? ExtractReminderDate(string input)
        {
            string text = input.ToLower();

            if (text.Contains("tomorrow")) return DateTime.Today.AddDays(1);
            if (text.Contains("today")) return DateTime.Today;
            if (text.Contains("next week") || text.Contains("in a week")) return DateTime.Today.AddDays(7);

            Match match = Regex.Match(text, @"in\s+(\d+)\s+(day|days|week|weeks)\b");
            if (match.Success)
            {
                int amount = Convert.ToInt32(match.Groups[1].Value);
                bool weeks = match.Groups[2].Value.StartsWith("week");
                return DateTime.Today.AddDays(weeks ? amount * 7 : amount);
            }

            return null;
        }

        /// <summary>
        /// Creates a helpful description for tasks added from the chat,
        /// based on keywords in the title.
        /// </summary>
        public string SuggestDescription(string title)
        {
            string text = title.ToLower();

            if (ContainsAny(text, "2fa", "two-factor", "two factor", "authentication"))
                return "Turn on two-factor authentication so a stolen password alone can't unlock your account.";
            if (text.Contains("password"))
                return "Replace weak or reused passwords with strong, unique passphrases.";
            if (text.Contains("privacy"))
                return "Review your account privacy settings to make sure your personal data is protected.";
            if (ContainsAny(text, "update", "software", "antivirus"))
                return "Install the latest security updates to fix known vulnerabilities.";
            if (ContainsAny(text, "backup", "back up"))
                return "Back up important files so you can recover them after ransomware or device loss.";

            return "Cybersecurity task created from the chat.";
        }

        private static bool ContainsAny(string input, params string[] phrases)
        {
            foreach (string phrase in phrases)
            {
                if (input.Contains(phrase)) return true;
            }
            return false;
        }
    }
}
