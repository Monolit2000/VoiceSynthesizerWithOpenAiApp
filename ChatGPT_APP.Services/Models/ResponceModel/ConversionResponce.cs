using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatGPT_APP.Models
{
    public class ConversionResponce
    {
        public Stream? VoiceFileSream { get; set; } = null;
        public string Text { get; set; }
        public int PartNumber { get; set; } 

        public ConversionResponce(Stream VoiceFileSream, string Text, int PartNumber)
        {
            this.VoiceFileSream = VoiceFileSream;
            this.Text = Text;
            this.PartNumber = PartNumber;
        }
        public ConversionResponce(string Text)
        {
           this.Text = Text;
        }
    }
}
