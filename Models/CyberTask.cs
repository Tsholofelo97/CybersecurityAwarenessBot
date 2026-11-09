namespace CybersecurityAwarenessBotPart2.Models
{
    /// <summary>
    /// One cybersecurity task. Each object matches one row in the cyber_tasks table.
    /// </summary>
    public class CyberTask
    {
        public int TaskId { get; set; }
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";

        // Nullable because a reminder is optional (NULL in MySQL).
        public DateTime? ReminderDate { get; set; }
        public bool IsCompleted { get; set; }

        // Read-only helper properties used by the DataGrid columns.
        public string Status => IsCompleted ? "Completed" : "Pending";

        public string ReminderText =>
            ReminderDate.HasValue ? ReminderDate.Value.ToString("dd MMM yyyy") : "No reminder";
    }
}
