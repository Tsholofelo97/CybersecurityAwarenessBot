```csharp
using System;
namespace CybersecurityAwarenessBot
{
    /// <summary>
    /// Handles cybersecurity questions and provides appropriate responses.
    /// </summary>
    public class ResponseHandler
    {
        public string GetResponse(string userInput)
        {
            // Validate that the user entered something.
            if (string.IsNullOrWhiteSpace(userInput))
            {
                return "Please enter a question so I can help you.";
            }

            string input = userInput.ToLower().Trim();

            if (input.Contains("password"))
            {
                return "Use strong, unique passwords with uppercase letters, lowercase letters, numbers and symbols. Avoid using the same password for multiple accounts.";
            }
            else if (input.Contains("phishing"))
            {
                return "Phishing is a scam that tricks people into revealing sensitive information. Avoid suspicious links and never share your passwords.";
            }
            else if (input.Contains("browsing") || input.Contains("safe browsing"))
            {
                return "Only visit trusted websites, check website addresses carefully and avoid clicking suspicious links.";
            }
            else if (input.Contains("malware"))
            {
                return "Malware is malicious software that can damage your device or steal information. Keep your software updated and use trusted security software.";
            }
            else if (input.Contains("social engineering"))
            {
                return "Social engineering involves manipulating people into giving away confidential information. Always verify unexpected requests.";
            }
            else if (input.Contains("purpose"))
            {
                return "My purpose is to teach cybersecurity awareness and help users stay safer online.";
            }
            else if (input.Contains("how are you"))
            {
                return "I am doing well and ready to help you stay safe online.";
            }

            // Default response for questions the chatbot does not recognise.
            return "I did not understand that. Please ask me about passwords, phishing, malware, social engineering or safe browsing.";
        }
    }
}
```
