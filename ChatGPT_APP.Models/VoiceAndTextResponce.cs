using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace ChatGPT_APP.Models
{
    public class VoiceAndText
    {
        public Guid Id { get; set; }
        public DateTime DateTime { get; set; }
        public Stream? VoiceFileSream { get; set; } = null;
        public string Text { get; set; } 
        public int Duration { get; set; }

        public VoiceAndText(Stream VoiceFileSream, string Text, int PartNumber, Guid id, DateTime dateTime, Stream? voiceFileSream, string text, int duration)
        {
            this.VoiceFileSream = VoiceFileSream;
            this.Text = Text;
            Id = id;
            DateTime = dateTime;
            this.VoiceFileSream = voiceFileSream;
            this.Text = text;
            Duration = duration;
        }
    }
}
