using System;
using System.Collections.Generic;

namespace CybersecurityAwarenessBotPart2
{
    public class ResponseHandler
    {
        private Random random = new Random();

        private Dictionary<string, List<string>> responses =
            new Dictionary<string, List<string>>()
        {
            {
                "password",
                new List<string>()
                {
                    "Use strong unique passwords.",
                    "Never share your password.",
                    "Use a password manager."
                }
            },

            {
                "phishing",
                new List<string>()
                {
                    "Be careful of suspicious emails.",
                    "Check sender addresses carefully.",
                    "Never click unknown links."
                }
            },

            {
                "browsing",
                new List<string>()
                {
                    "Only visit trusted websites.",
                    "Check for HTTPS.",
                    "Keep your browser updated."
                }
            }
        };

        public string GetResponse(string input)
        {
            input = input.ToLower();

            foreach (var keyword in responses.Keys)
            {
                if (input.Contains(keyword))
                {
                    List<string> list = responses[keyword];
                    return list[random.Next(list.Count)];
                }
            }

            return "I'm not sure I understand. Can you try rephrasing?";
        }
    }
}