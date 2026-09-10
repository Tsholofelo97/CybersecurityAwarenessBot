using System;
using System.IO;
using System.Media;

namespace CybersecurityAwarenesssBot
{
    public class VoiceGreeting
    {
        public void PlayGreeting()
        {
            try
            {
                string filePath = Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "Audio",
                    "greeting.wav");

                if (File.Exists(filePath))
                {
                    SoundPlayer player = new SoundPlayer(filePath);
                    player.PlaySync();
                }
                else
                {
                    Console.WriteLine("Greeting audio file not found.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Audio error: " + ex.Message);
            }
        }
    }
}