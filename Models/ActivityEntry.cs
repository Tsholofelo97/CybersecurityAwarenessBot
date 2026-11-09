namespace CybersecurityAwarenessBotPart2.Models
{
    public class ActivityEntry
    {
        public DateTime Time { get; set; }
        public string Description { get; set; } = "";

        // This is what the ListBox shows for each entry.
        public override string ToString()
        {
            return $"{Time:HH:mm:ss}  -  {Description}";
        }
    }
}
