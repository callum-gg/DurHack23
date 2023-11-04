using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace DurHack_2023
{
    public class Process
    {
        public async Task<List<ParsedWord>> ProcessImage(string image)
        {
            OCR charRec = new OCR();
            (string text, List<ParsedWord> words) = await charRec.ReadImage(image);

            SpellCheck spellCheck = new SpellCheck();
            List<Mistake> mistakes = await spellCheck.CheckSpell(text);

            foreach (Mistake mistake in mistakes)
            {
                int indexCount = 0;
                foreach (ParsedWord word in words)
                {
                    if (indexCount == mistake.Index)
                    {
                        word.Corrections = mistake.Corrections;
                        break;
                    }
                    else
                    {
                        indexCount += word.Text.Length + 1;
                    }
                }

            }

            return words;
        }
    }
}
