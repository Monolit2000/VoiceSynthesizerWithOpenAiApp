using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatGPT_APP.Services.Models.RequestModel
{
    public class ConversionRequest
    {
        public long id { get; set; } = 1;
        public Stream VoiceFileSream { get; set; }
        public string AudioPath { get; set; }
        public string? VoiceName { get; set; }
        public int Duration { get; set; }

        public ConversionRequest(string audioPath, int duration , string voiceName = null) : this(File.OpenRead(audioPath), duration, audioPath ,voiceName )
        {
        }

        public ConversionRequest(Stream voiceFileSream, int duration, string audioPath = null, string voiceName = null )
        {
            VoiceFileSream = voiceFileSream;
            AudioPath = audioPath;
            VoiceName = voiceName;
            Duration = duration;    
        }
    }
}
