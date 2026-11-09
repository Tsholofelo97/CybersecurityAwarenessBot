namespace CybersecurityAwarenessBotPart2.Services
{
    /// <summary>The actions the NLP simulation can recognise.</summary>
    public enum UserIntent
    {
        AddTask,
        AddTaskWithReminder,
        StartQuiz,
        ShowTasks,
        ShowActivityLog,
        CybersecurityQuestion   // anything else goes to the Part 2 chatbot
    }
}
