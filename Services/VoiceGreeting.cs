using System.IO;
using System.Media;

namespace CybersecurityAwarenessBotPart2.Services
{
    /// <summary>
    /// Plays the recorded voice greeting when the app opens (Part 1 feature).
    /// The file must be a real .wav file at Audio/greeting.wav.
    /// </summary>
    public static class VoiceGreeting
    {
        // Kept as a field so the sound is not cut off while it is playing.
        private static SoundPlayer? player;

        public static bool PlayGreeting()
        {
            try
            {
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Audio", "greeting.wav");

                if (!File.Exists(path))
                    return false;

                player = new SoundPlayer(path);
                player.Play();
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Voice greeting could not play: " + ex.Message);
                return false;
            }
        }
    }
}
