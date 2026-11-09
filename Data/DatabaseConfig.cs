namespace CybersecurityAwarenessBotPart2.Data
{
    public static class DatabaseConfig
    {
        // Replace YOUR_PASSWORD with your own local MySQL password before running.
        // Do NOT push your real password to GitHub - keep the placeholder in the repo.
        public static string ConnectionString =
            "Server=localhost;Port=3306;" +
            "Database=CyberSecurityBotDB;" +
            "Uid=root;Pwd=cyber123;";
    }
}
