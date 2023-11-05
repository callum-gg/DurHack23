using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DurHack_2023
{
    using System.Collections.Generic;

    public class Vertex
    {
        public int x { get; set; }
        public int y { get; set; }
    }

    public class BoundingPoly
    {
        public List<Vertex> vertices { get; set; }
    }

    public class Symbol
    {
        public BoundingPoly boundingBox { get; set; }
        public string text { get; set; }
    }

    public class Word
    {
        public BoundingPoly boundingBox { get; set; }
        public List<Symbol> symbols { get; set; }
    }

    public class Paragraph
    {
        public BoundingPoly boundingBox { get; set; }
        public List<Word> words { get; set; }
    }

    public class Block
    {
        public BoundingPoly boundingBox { get; set; }
        public List<Paragraph> paragraphs { get; set; }
        public string blockType { get; set; }
    }

    public class Page
    {
        public Property property { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public List<Block> blocks { get; set; }
    }

    public class FullTextAnnotation
    {
        public List<Page> pages { get; set; }
        public string text { get; set; }
    }

    public class DetectedLanguage
    {
        public string languageCode { get; set; }
        public double confidence { get; set; }
    }

    public class Property
    {
        public List<DetectedLanguage> detectedLanguages { get; set; }
    }

    public class TextAnnotation
    {
        public string locale { get; set; }
        public string description { get; set; }
        public BoundingPoly boundingPoly { get; set; }
    }

    public class Response
    {
        public List<TextAnnotation> textAnnotations { get; set; }
        public FullTextAnnotation fullTextAnnotation { get; set; }
    }

    public class Root
    {
        public List<Response> responses { get; set; }
    }

}
