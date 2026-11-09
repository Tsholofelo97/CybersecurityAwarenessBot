namespace CybersecurityAwarenessBotPart2.Services
{
    /// <summary>
    /// Remembers details about the user during the conversation (Part 2 memory feature).
    /// </summary>
    public class MemoryManager
    {
        public string? UserName { get; set; }

        // The topic discussed most recently - used for "tell me more".
        public string? LastTopic { get; set; }

        // A topic the user said they are interested in.
        public string? FavouriteTopic { get; set; }
    }
}
