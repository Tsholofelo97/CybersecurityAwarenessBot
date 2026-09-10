```csharp
using System;

namespace CybersecurityAwarenessBot
{
    /// <summary>
    /// Controls the main interaction between the user and the chatbot.
    /// </summary>
    public class Chatbot
    {
        private UserProfile user = new UserProfile();
        private ResponseHandler responseHandler = new ResponseHandler();

        /// <summary>
        /// Starts the chatbot and displays the welcome information.
        /// </summary>
        public void Start()
        {
            // Play the voice greeting.
            VoiceGreeting voice = new VoiceGreeting();
            voice.PlayGreeting();

            // Display the chatbot logo.
            AsciiArt art = new AsciiArt();
            art.DisplayLogo();

            // Ask the user for their name.
            GetUserName();

            Console.ForegroundColor = ConsoleColor.Green;

            Console.WriteLine();
            Console.WriteLine($"Welcome {user.Name}!");
            Console.WriteLine("I am your Cybersecurity Awareness Assistant.");
            Console.WriteLine();

            Console.WriteLine("You can ask me about:");
            Console.WriteLine("- Password Safety");
            Console.WriteLine("- Phishing");
            Console.WriteLine("- Safe Browsing");
            Console.WriteLine("- Malware");
            Console.WriteLine("- Social Engineering");
            Console.WriteLine("- My Purpose");
            Console.WriteLine();

            Console.ResetColor();

            StartConversation();
        }

        /// <summary>
        /// Validates and stores the user's name.
        /// </summary>
        private void GetUserName()
        {
            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("Please enter your name: ");
                Console.ResetColor();

                string? name = Console.ReadLine();

                if (!string.IsNullOrWhiteSpace(name))
                {
                    user.Name = name.Trim();
                    break;
                }

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("I did not catch your name. Please try again.");
                Console.ResetColor();
            }
        }

        /// <summary>
        /// Handles the conversation between the user and the chatbot.
        /// </summary>
        private void StartConversation()
        {
            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write($"{user.Name}> ");
                Console.ResetColor();

                string? question = Console.ReadLine();

                // Validate empty input.
                if (string.IsNullOrWhiteSpace(question))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Please type a question.");
                    Console.ResetColor();
                    continue;
                }

                question = question.Trim();

                // Allow the user to exit the chatbot.
                if (question.Equals("exit", StringComparison.OrdinalIgnoreCase))
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine($"Goodbye {user.Name}. Stay safe online!");
                    Console.ResetColor();
                    break;
                }

                // Get an appropriate response.
                string response = responseHandler.GetResponse(question);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Bot> {response}");
                Console.ResetColor();
                Console.WriteLine();
            }
        }
    }
}
```
