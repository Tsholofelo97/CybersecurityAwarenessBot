 sharp
using System.Windows;

namespace CybersecurityAwarenessBotPart2
{
    public partial class MainWindow : Window
    {
        private readonly ResponseHandler responseHandler;
        private readonly SentimentDetector sentimentDetector;
        private readonly MemoryManager memory;

        public MainWindow()
        {
            InitializeComponent();

            responseHandler = new ResponseHandler();
            sentimentDetector = new SentimentDetector();
            memory = new MemoryManager();

            ChatDisplay.Text = "Bot: Welcome to the Cybersecurity Awareness Bot!\n";
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            string userText = UserInput.Text.Trim();

            if (string.IsNullOrWhiteSpace(userText))
            {
                ChatDisplay.AppendText("Bot: Please type a message before sending.\n");
                return;
            }

            ChatDisplay.AppendText("\nYou: " + userText + "\n");

            string sentiment = sentimentDetector.DetectSentiment(userText);

            if (sentiment == "negative")
            {
                ChatDisplay.AppendText(
                    "Bot: I understand your concern. Let me help you.\n");
            }
            else if (sentiment == "positive")
            {
                ChatDisplay.AppendText(
                    "Bot: I'm glad to hear that!\n");
            }

            string response = responseHandler.GetResponse(userText);

            ChatDisplay.AppendText("Bot: " + response + "\n");

            UserInput.Clear();
            UserInput.Focus();
        }
    }
}
