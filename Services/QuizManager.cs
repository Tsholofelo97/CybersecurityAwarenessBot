using CybersecurityAwarenessBotPart2.Models;

namespace CybersecurityAwarenessBotPart2.Services
{
    /// <summary>
    /// Runs the cybersecurity quiz: 12 questions (multiple choice + true/false),
    /// one at a time, with immediate feedback and a score.
    /// </summary>
    public class QuizManager
    {
        private readonly Random random = new Random();
        private List<QuizQuestion> questions;

        public int CurrentIndex { get; private set; }
        public int Score { get; private set; }
        public bool QuizStarted { get; private set; }
        public int TotalQuestions => questions.Count;
        public QuizQuestion CurrentQuestion => questions[CurrentIndex];

        public QuizManager()
        {
            questions = BuildQuestions();
        }

        public void StartQuiz()
        {
            // Shuffle the order so each attempt feels fresh.
            questions = questions.OrderBy(q => random.Next()).ToList();
            CurrentIndex = 0;
            Score = 0;
            QuizStarted = true;
        }

        public QuizAnswerResult CheckAnswer(int selectedIndex)
        {
            bool correct = selectedIndex == CurrentQuestion.CorrectOptionIndex;
            if (correct) Score++;

            string correctAnswer = CurrentQuestion.Options[CurrentQuestion.CorrectOptionIndex];

            return new QuizAnswerResult
            {
                IsCorrect = correct,
                Feedback = correct
                    ? "✔ Correct! " + CurrentQuestion.Explanation
                    : $"✘ Not quite. The answer is \"{correctAnswer}\". " + CurrentQuestion.Explanation
            };
        }

        // Returns false when there are no more questions.
        public bool MoveNext()
        {
            if (CurrentIndex < questions.Count - 1)
            {
                CurrentIndex++;
                return true;
            }

            QuizStarted = false;
            return false;
        }

        public string GetFinalComment()
        {
            double percentage = (double)Score / TotalQuestions * 100;

            if (percentage >= 80) return "Excellent! You're a cybersecurity pro. 🛡";
            if (percentage >= 60) return "Good work! Review the topics you missed to get even stronger.";
            return "Keep learning! Ask me about the topics you missed, then try again.";
        }

        private static List<QuizQuestion> BuildQuestions()
        {
            return new List<QuizQuestion>
            {
                new QuizQuestion
                {
                    QuestionText = "You get an unexpected email asking for your password. What should you do?",
                    Options = new List<string> { "Reply with your password", "Click the link to check", "Report it as phishing", "Forward it to friends" },
                    CorrectOptionIndex = 2,
                    Explanation = "Legitimate organisations never ask for your password by email. Reporting it helps stop the scam."
                },
                new QuizQuestion
                {
                    QuestionText = "Which password is the strongest?",
                    Options = new List<string> { "Password123", "Tsholo1997", "purple-Taxi-river-42!", "qwerty" },
                    CorrectOptionIndex = 2,
                    Explanation = "Long passphrases mixing random words, numbers and symbols are much harder to crack than names or dates."
                },
                new QuizQuestion
                {
                    QuestionText = "True or False: Public Wi-Fi is always safe for online banking.",
                    Options = new List<string> { "True", "False" },
                    CorrectOptionIndex = 1,
                    Explanation = "Public Wi-Fi can be monitored. Use mobile data or a VPN for sensitive activity."
                },
                new QuizQuestion
                {
                    QuestionText = "What is the main purpose of two-factor authentication (2FA)?",
                    Options = new List<string> { "To make passwords shorter", "To add a second verification step", "To remove the need for passwords", "To share logins faster" },
                    CorrectOptionIndex = 1,
                    Explanation = "2FA means a stolen password alone isn't enough to get into your account."
                },
                new QuizQuestion
                {
                    QuestionText = "What is social engineering?",
                    Options = new List<string> { "Manipulating people into revealing information", "Designing social media logos", "Repairing computer hardware", "Building social networks" },
                    CorrectOptionIndex = 0,
                    Explanation = "Social engineering attacks people, not technology, using trust, fear or urgency."
                },
                new QuizQuestion
                {
                    QuestionText = "Someone calls claiming to be your bank and asks for the OTP they just sent you. What should you do?",
                    Options = new List<string> { "Read them the OTP", "Hang up and call the bank's official number", "Send the OTP by SMS instead", "Give only half the OTP" },
                    CorrectOptionIndex = 1,
                    Explanation = "Banks never ask for your OTP. Hang up and contact them through their official number."
                },
                new QuizQuestion
                {
                    QuestionText = "True or False: Reusing one very strong password on every account is safe.",
                    Options = new List<string> { "True", "False" },
                    CorrectOptionIndex = 1,
                    Explanation = "If one site is breached, attackers try that password on all your other accounts."
                },
                new QuizQuestion
                {
                    QuestionText = "Which email attachment should make you most cautious?",
                    Options = new List<string> { "An expected PDF from your lecturer", "An unexpected invoice.exe file", "A photo you sent yourself", "A document from your own drive" },
                    CorrectOptionIndex = 1,
                    Explanation = "Unexpected executable files are a common way to deliver malware."
                },
                new QuizQuestion
                {
                    QuestionText = "What does a password manager help you do?",
                    Options = new List<string> { "Store and generate unique passwords", "Post passwords online", "Turn off account security", "Avoid passwords completely" },
                    CorrectOptionIndex = 0,
                    Explanation = "A password manager lets you use a strong, unique password for every account."
                },
                new QuizQuestion
                {
                    QuestionText = "Why should you review your social media privacy settings?",
                    Options = new List<string> { "To control who sees your personal information", "To make every post public", "To get more spam", "To accept all tracking" },
                    CorrectOptionIndex = 0,
                    Explanation = "Limiting what strangers can see reduces the details scammers can use against you."
                },
                new QuizQuestion
                {
                    QuestionText = "Why are software security updates important?",
                    Options = new List<string> { "They only change colours", "They fix known security weaknesses", "They delete your files", "They make phishing impossible" },
                    CorrectOptionIndex = 1,
                    Explanation = "Updates patch vulnerabilities that attackers already know how to exploit."
                },
                new QuizQuestion
                {
                    QuestionText = "True or False: A message can be a scam even if it shows a real company's logo.",
                    Options = new List<string> { "True", "False" },
                    CorrectOptionIndex = 0,
                    Explanation = "Logos are easy to copy. Always check the sender's address and the link's real destination."
                }
            };
        }
    }
}
