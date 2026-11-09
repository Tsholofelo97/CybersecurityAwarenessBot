using CybersecurityAwarenessBotPart2.Models;

namespace CybersecurityAwarenessBotPart2.Services
{
    /// <summary>
    /// Keeps a list of meaningful actions (tasks, reminders, quiz, NLP commands).
    /// </summary>
    public class ActivityLogger
    {
        private readonly List<ActivityEntry> entries = new List<ActivityEntry>();

        public void Log(string description)
        {
            entries.Add(new ActivityEntry { Time = DateTime.Now, Description = description });
        }

        // Newest entries first.
        public List<ActivityEntry> GetRecent(int count)
        {
            return entries.AsEnumerable().Reverse().Take(count).ToList();
        }

        public int TotalCount => entries.Count;
    }
}
