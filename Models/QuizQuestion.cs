namespace CybersecurityAwarenessBotPart2.Models
{
    public class QuizQuestion
    {
        public string QuestionText { get; set; } = "";

        // Two options = True/False question, four options = multiple choice.
        public List<string> Options { get; set; } = new List<string>();

        // 0 = first option, 1 = second option, etc.
        public int CorrectOptionIndex { get; set; }

        public string Explanation { get; set; } = "";
    }
}
