using System.Text.RegularExpressions;

namespace CybersecurityAwarenessBotPart2.Services
{
    /// <summary>
    /// Part 2 chatbot brain: keyword recognition, random responses,
    /// conversation flow ("tell me more") and memory.
    /// </summary>
    public class ResponseHandler
    {
        private readonly Random random = new Random();
        private string lastResponse = "";

        // Each topic has several keywords that point to it.
        private readonly Dictionary<string, string[]> topicKeywords = new Dictionary<string, string[]>
        {
            { "passwords", new[] { "password", "passphrase" } },
            { "phishing", new[] { "phishing", "phish", "suspicious email", "fake email", "suspicious link" } },
            { "safe browsing", new[] { "browsing", "browser", "website", "https", "download" } },
            { "privacy", new[] { "privacy", "personal information", "personal data", "social media" } },
            { "scams", new[] { "scam", "fraud", "smishing", "vishing" } },
            { "malware", new[] { "malware", "virus", "ransomware", "spyware", "trojan" } },
            { "two-factor authentication", new[] { "2fa", "two-factor", "two factor", "mfa", "multi-factor" } },
            { "public Wi-Fi", new[] { "wifi", "wi-fi", "public network", "hotspot", "vpn" } }
        };

        // Several responses per topic so the answer is not always the same.
        private readonly Dictionary<string, List<string>> topicResponses = new Dictionary<string, List<string>>
        {
            { "passwords", new List<string>
                {
                    "Use a long passphrase of at least 12 characters, like three or four random words joined together.",
                    "Never reuse passwords. If one site is breached, attackers try the same password everywhere else.",
                    "A password manager can create and store strong, unique passwords so you don't have to remember them.",
                    "Never share your password, not even with someone claiming to be from IT support."
                } },
            { "phishing", new List<string>
                {
                    "Check the sender's real email address, not just the display name. Scammers copy logos and names easily.",
                    "Be wary of messages that create urgency, like 'your account will be closed today'. That pressure is a classic phishing trick.",
                    "Hover over a link before clicking to see where it really goes. If it doesn't match the company's real website, don't click.",
                    "Legitimate organisations will never ask for your password or OTP by email or SMS."
                } },
            { "safe browsing", new List<string>
                {
                    "Look for HTTPS and the padlock icon before entering personal details on a website.",
                    "Only download software from official websites or app stores.",
                    "Keep your browser and extensions updated, since updates patch security holes.",
                    "Be careful with pop-ups claiming your device is infected. They are often scams themselves."
                } },
            { "privacy", new List<string>
                {
                    "Review the privacy settings on your social media accounts and limit who can see your posts and personal details.",
                    "Think before sharing your location, ID number or birthday online. Scammers use these details for identity theft.",
                    "Check which apps have access to your camera, microphone and location, and remove access they don't need.",
                    "Use different email addresses for important accounts and for sign-ups, to reduce spam and data exposure."
                } },
            { "scams", new List<string>
                {
                    "If an offer sounds too good to be true, like a prize you never entered for, it's almost certainly a scam.",
                    "Scammers often pretend to be banks or delivery companies. Contact the company directly using their official number.",
                    "Never share a one-time PIN (OTP) with anyone. Your bank will never ask you for it."
                } },
            { "malware", new List<string>
                {
                    "Keep your operating system and antivirus updated to protect against malware.",
                    "Don't open unexpected attachments, especially .exe, .zip or macro-enabled Office files.",
                    "Back up your important files regularly so ransomware can't hold them hostage."
                } },
            { "two-factor authentication", new List<string>
                {
                    "Two-factor authentication (2FA) adds a second step, like a code on your phone, so a stolen password alone isn't enough.",
                    "Authenticator apps are safer than SMS codes for 2FA because SMS can be intercepted through SIM-swap fraud.",
                    "Turn on 2FA for your email first. Email is the key to resetting most of your other accounts."
                } },
            { "public Wi-Fi", new List<string>
                {
                    "Avoid online banking or shopping on public Wi-Fi. Use mobile data instead.",
                    "A VPN encrypts your traffic, which helps protect you on public Wi-Fi networks.",
                    "Turn off auto-connect so your device doesn't join unknown Wi-Fi networks by itself."
                } }
        };

        public string GetResponse(string input, MemoryManager memory)
        {
            string text = input.ToLower().Trim();

            // 1. Memory recall
            if (ContainsAny(text, "what's my name", "what is my name", "do you remember me", "what do you remember"))
            {
                string reply = memory.UserName != null
                    ? $"Of course! Your name is {memory.UserName}."
                    : "I don't know your name yet.";
                if (memory.FavouriteTopic != null)
                    reply += $" You told me you're interested in {memory.FavouriteTopic}.";
                return reply;
            }

            // 2. Remember the user's interest, e.g. "I'm interested in privacy"
            Match interest = Regex.Match(text,
                @"(interested in|favourite topic is|favorite topic is|i care about|i want to learn about)\s+(.+)");
            if (interest.Success)
            {
                string? topic = FindTopic(interest.Groups[2].Value);
                if (topic != null)
                {
                    memory.FavouriteTopic = topic;
                    memory.LastTopic = topic;
                    return $"Great! I'll remember that you're interested in {topic}. Here's a tip: {GetTip(topic)}";
                }
            }

            // 3. Keyword recognition for cybersecurity topics
            string? found = FindTopic(text);
            if (found != null)
            {
                memory.LastTopic = found;
                string tip = GetTip(found);

                if (found == memory.FavouriteTopic && memory.UserName != null)
                    tip += $" Since {found} is a topic you care about, {memory.UserName}, it's worth reviewing this regularly.";

                return tip;
            }

            // 4. Conversation flow - follow-up on the last topic
            if (ContainsAny(text, "tell me more", "more info", "explain more", "another tip",
                                  "give me more", "elaborate", "what else", "more"))
            {
                if (memory.LastTopic != null)
                    return $"Here's more on {memory.LastTopic}: {GetTip(memory.LastTopic)}";

                return "Happy to tell you more! Which topic would you like: passwords, phishing, privacy, scams, malware or 2FA?";
            }

            // 5. General conversation
            if (ContainsAny(text, "how are you"))
                return "I'm running smoothly and ready to help you stay safe online! What would you like to know?";

            if (ContainsAny(text, "your purpose", "who are you", "what are you"))
                return "I'm a Cybersecurity Awareness Assistant. I teach you how to stay safe online, track your security tasks, quiz you and keep a log of what we've done.";

            if (ContainsAny(text, "thank"))
                return "You're welcome! Stay safe online.";

            if (Regex.IsMatch(text, @"\b(help|what can you do|what can i ask)\b"))
                return GetHelpText();

            // 6. Unknown input
            return "I'm not sure I understand. Could you rephrase? You can ask about passwords, phishing, safe browsing, " +
                   "privacy, scams, malware, 2FA or public Wi-Fi, or type 'help' to see everything I can do.";
        }

        public static string GetHelpText()
        {
            return "Here's what I can do:\n" +
                   "  • Answer questions about passwords, phishing, safe browsing, privacy, scams, malware, 2FA and public Wi-Fi\n" +
                   "  • \"Add a task to enable two-factor authentication\"\n" +
                   "  • \"Remind me to update my password tomorrow\" or \"... in 3 days\"\n" +
                   "  • \"Show my tasks\"\n" +
                   "  • \"Start the quiz\" or \"Test my knowledge\"\n" +
                   "  • \"What have you done for me?\" to see the activity log\n" +
                   "  • \"Tell me more\" after any tip";
        }

        // Returns the topic name if any of its keywords appear in the text.
        private string? FindTopic(string text)
        {
            foreach (KeyValuePair<string, string[]> pair in topicKeywords)
            {
                if (ContainsAny(text, pair.Value))
                    return pair.Key;
            }
            return null;
        }

        // Picks a random tip, avoiding the exact tip given last time.
        private string GetTip(string topic)
        {
            List<string> options = topicResponses[topic];
            string tip;
            do
            {
                tip = options[random.Next(options.Count)];
            } while (tip == lastResponse && options.Count > 1);

            lastResponse = tip;
            return tip;
        }

        private static bool ContainsAny(string text, params string[] phrases)
        {
            foreach (string phrase in phrases)
            {
                if (text.Contains(phrase)) return true;
            }
            return false;
        }
    }
}
