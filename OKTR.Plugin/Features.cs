using System.Globalization;
using System.Speech.Synthesis;

namespace OKTR.Plugin
{
    public static class Features
    {
        /// <summary>
        /// Play a sound using the voice assistant
        /// </summary>
        /// <param name="phrase">What it will say</param>
        /// <param name="gender">What gender</param>
        /// <param name="age">What age</param>
        /// <param name="whichCulture">Which culture to use Ex: en-US,pt-BR and etc</param>
        public static void PlayPhrase(string phrase, VoiceGender gender = VoiceGender.Female,
            VoiceAge age = VoiceAge.Adult, string whichCulture = "en-US")
        {
            using (var playphrase = new SpeechSynthesizer())
            {
                playphrase.SelectVoiceByHints(gender, age, -2, new CultureInfo(whichCulture));
                playphrase.SetOutputToDefaultAudioDevice();
                playphrase.Speak(phrase);
            }
        }
    }
}
