using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Braille.Kernal
{
    public sealed class brailleUtil
    {
        #region Dictionaries

        private static Dictionary<char, byte> brailleCharToByte = new Dictionary<char, byte>()
        {
            {'a', 0b00000001},
            {'b', 0b00000011},
            {'c', 0b00001001},
            {'d', 0b00011001},
            {'e', 0b00010001},
            {'f', 0b00001011},
            {'g', 0b00011011},
            {'h', 0b00010011},
            {'i', 0b00001010},
            {'j', 0b00011010},
            {'k', 0b00000101},
            {'l', 0b00000111},
            {'m', 0b00001101},
            {'n', 0b00011101},
            {'o', 0b00010101},
            {'p', 0b00001111},
            {'q', 0b00011111},
            {'r', 0b00010111},
            {'s', 0b00001110},
            {'t', 0b00011110},
            {'u', 0b00100101},
            {'v', 0b00100111},
            {'w', 0b00111010},
            {'x', 0b00101101},
            {'y', 0b00111101},
            {'z', 0b00110101},

            {'A', 0b01000001},
            {'B', 0b01000011},
            {'C', 0b01001001},
            {'D', 0b01011001},
            {'E', 0b01010001},
            {'F', 0b01001011},
            {'G', 0b01011011},
            {'H', 0b01010011},
            {'I', 0b01001010},
            {'J', 0b01011010},
            {'K', 0b01000101},
            {'L', 0b01000111},
            {'M', 0b01001101},
            {'N', 0b01011101},
            {'O', 0b01010101},
            {'P', 0b01001111},
            {'Q', 0b01011111},
            {'R', 0b01010111},
            {'S', 0b01001110},
            {'T', 0b01011110},
            {'U', 0b01100101},
            {'V', 0b01100111},
            {'W', 0b01111010},
            {'X', 0b01101101},
            {'Y', 0b01111101},
            {'Z', 0b01110101},

            {'1', 0b00100001},
            {'2', 0b00100011},
            {'3', 0b00101001},
            {'4', 0b00111001},
            {'5', 0b00110001},
            {'6', 0b00101011},
            {'7', 0b00111011},
            {'8', 0b00110011},
            {'9', 0b00101010},
            {'0', 0b00111100},

            {'!', 0b00010110},
            {'(', 0b00100110},
            {')', 0b00110100},
            {'*', 0b00010100},
            {',', 0b00000010},
            {'-', 0b00100100},
            {'.', 0b00000100},
            {':', 0b00010010},
            {';', 0b00000110},
            {'?', 0b00100010},

            { ' ', 0b00000000}
        };

        private static Dictionary<byte, char> brailleByteToChar = new Dictionary<byte, char>()
        {
            {0b00000001,'a'},
            {0b00000011,'b'},
            {0b00001001,'c'},
            {0b00011001,'d'},
            {0b00010001,'e'},
            {0b00001011,'f'},
            {0b00011011,'g'},
            {0b00010011,'h'},
            {0b00001010,'i'},
            {0b00011010,'j'},
            {0b00000101,'k'},
            {0b00000111,'l'},
            {0b00001101,'m'},
            {0b00011101,'n'},
            {0b00010101,'o'},
            {0b00001111,'p'},
            {0b00011111,'q'},
            {0b00010111,'r'},
            {0b00001110,'s'},
            {0b00011110,'t'},
            {0b00100101,'u'},
            {0b00100111,'v'},
            {0b00111010,'w'},
            {0b00101101,'x'},
            {0b00111101,'y'},
            {0b00110101,'z'},

            {0b01000001,'A'},
            {0b01000011,'B'},
            {0b01001001,'C'},
            {0b01011001,'D'},
            {0b01010001,'E'},
            {0b01001011,'F'},
            {0b01011011,'G'},
            {0b01010011,'H'},
            {0b01001010,'I'},
            {0b01011010,'J'},
            {0b01000101,'K'},
            {0b01000111,'L'},
            {0b01001101,'M'},
            {0b01011101,'N'},
            {0b01010101,'O'},
            {0b01001111,'P'},
            {0b01011111,'Q'},
            {0b01010111,'R'},
            {0b01001110,'S'},
            {0b01011110,'T'},
            {0b01100101,'U'},
            {0b01100111,'V'},
            {0b01111010,'W'},
            {0b01101101,'X'},
            {0b01111101,'Y'},
            {0b01110101,'Z'},

            {0b00100001,'1'},
            {0b00100011,'2'},
            {0b00101001,'3'},
            {0b00111001,'4'},
            {0b00110001,'5'},
            {0b00101011,'6'},
            {0b00111011,'7'},
            {0b00110011,'8'},
            {0b00101010,'9'},
            {0b00111100,'0'},

            {0b00010110,'!'},
            {0b00100110,'('},
            {0b00110100,')'},
            {0b00010100,'*'},
            {0b00000010,','},
            {0b00100100,'-'},
            {0b00000100,'.'},
            {0b00010010,':'},
            {0b00000110,';'},
            {0b00100010,'?'},

            {0b00000000,' '}
        };

        private static Dictionary<byte, char> en = new Dictionary<byte, char>()
        {
        };

        private static Dictionary<byte, char> de = new Dictionary<byte, char>()
        {
            {0b00011100,'ä'},
            {0b00101010,'ö'},
            {0b00110011,'ü'},

            {0b01011100,'Ä'},
            {0b01101010,'Ö'},
            {0b01110011,'Ü'},
        };

        private static Dictionary<byte, char> dk = new Dictionary<byte, char>()
        {
            {0b00100001,'å'},
            {0b01100001,'Å'},
        };

        #endregion Dictionaries

        #region Methods

        private static bool Extend(IEnumerable<brailleLang> extentions)
        {
            foreach (brailleLang lang in extentions)
            {
                Dictionary<byte, char> dic = new Dictionary<byte, char>();
                var field = typeof(brailleUtil).GetField(lang.ToString()).GetValue(null);

                if (field.GetType().IsAssignableFrom(typeof(Dictionary<byte, char>)))
                    dic = field as Dictionary<byte, char>;
                else return false;

                foreach (KeyValuePair<byte, char> kvP in dic)
                {
                    brailleByteToChar.Append(kvP);
                    brailleCharToByte.Add(kvP.Value, kvP.Key);
                }
            }
            return true;
        }

        public static byte Conversion(char c, brailleType type = brailleType.sixDot)
        {
            byte result;

            if (brailleCharToByte.ContainsKey(c)) result = brailleCharToByte[c];
            else result = 0b00000000; // throw new Exception("Braille type not recognised");

            if (type == brailleType.sixDot && result >= 0b01000000) throw new Exception($"This charecter '{c.ToString()}' is not part of six dot braille");
            return result;
        }

        public static char Conversion(byte b, brailleType type = brailleType.sixDot, brailleLang lang = brailleLang.en)
        {
            if (brailleByteToChar.ContainsKey(b)) return brailleByteToChar[b];
            else return '?';
        }

        #endregion Methods
    }

    public sealed class brailleChar : IbrailleChar
    {
        #region header

        public string TypeName => "Brailer Charecter";
        public string TypeDescription => "A single Braille charecter";

        public bool IsValid
        {
            get
            {
                if (Value <= 0b11111111 && Value >= 0b00000000)
                {
                    if (this.type == brailleType.sixDot && this.Value >= 0b01000000) return false;
                    return true;
                }
                return false;
            }
        }

        #endregion header

        #region variables

        public byte Value { get; }
        public brailleType type { get; private set; }

        #endregion variables

        #region constructors

        public brailleChar(bool[] listChar)
        {
            if (listChar.Length == 6) this.type = brailleType.sixDot;
            else if (listChar.Length == 8) this.type = brailleType.eightDot;
            else throw new Exception("Not a valid length of booleans for a Braille charecter");

            byte b = 0b00000000;

            for (int i = 0; i < listChar.Length; ++i)
            {
                if (listChar[i]) b += (byte)Math.Pow(2, i);
            }
            this.Value = b;
        }

        public brailleChar(byte b, brailleType type)
        {
            this.type = type;
            this.Value = b;
        }

        public brailleChar(char c)
        {
            int start = 10240;
            int end = 10495;
            int steps = end - start;
            Int16 cInt = Convert.ToInt16(c);

            if (Enumerable.Range(start, steps).Contains(cInt))
            {
                cInt -= 0b10100000000000;
                this.Value = (byte)cInt;
            }
        }

        #endregion constructors

        #region public methods

        public bool dot1
        {
            get
            {
                return (this.Value & (1 << 1 - 1)) != 0;
            }
        }

        public bool dot2
        {
            get
            {
                return (this.Value & (1 << 2 - 1)) != 0;
            }
        }

        public bool dot3
        {
            get
            {
                return (this.Value & (1 << 3 - 1)) != 0;
            }
        }

        public bool dot4
        {
            get
            {
                return (this.Value & (1 << 4 - 1)) != 0;
            }
        }

        public bool dot5
        {
            get
            {
                return (this.Value & (1 << 5 - 1)) != 0;
            }
        }

        public bool dot6
        {
            get
            {
                return (this.Value & (1 << 6 - 1)) != 0;
            }
        }

        public bool dot7
        {
            get
            {
                return (this.Value & (1 << 7 - 1)) != 0;
            }
        }

        public bool dot8
        {
            get
            {
                return (this.Value & (1 << 8 - 1)) != 0;
            }
        }

        public bool IsSpace
        {
            get
            {
                if (Value == 0) return true;
                else return false;
            }
        }

        public override string ToString()
        {
            return brailleUtil.Conversion(this.Value).ToString();
        }

        public override bool Equals(object obj)
        {
            if (typeof(object).IsAssignableFrom(typeof(brailleChar)))
            {
                var o = obj as brailleChar;
                return o.Value == this.Value;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #endregion public methods
    }

    public sealed class brailleWord : IbrailleWord
    {
        /// <summary>
        /// List of charecter that make up the word
        /// </summary>
        public List<brailleChar> CharList
        { get; private set; }

        /// <summary>
        /// The count of charecter is a word
        /// </summary>
        public int Count { get => CharList.Count(); }

        /// <summary>
        /// Initialise an empty instance of a word
        /// </summary>
        public brailleWord()
        {
            CharList = new List<brailleChar>();
        }

        /// <summary>
        /// Construct a braille word from another word
        /// </summary>
        /// <param name="word">word as brailleWord</param>
        public brailleWord(brailleWord word)
        {
            CharList = new List<brailleChar>(word.CharList);
        }

        /// <summary>
        /// Construct a word from a list of charecters
        /// </summary>
        /// <param name="word">List of type brailleChar</param>
        public brailleWord(List<brailleChar> word)
        {
            CharList = word;
        }

        /// <summary>
        /// Add a chrecter to the word
        /// </summary>
        /// <param name="item">Charecter of type brailleChar</param>
        public void Add(brailleChar item)
        {
            CharList.Add(item);
        }

        /// <summary>
        /// String resentation of the word
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var text = string.Empty;
            foreach (brailleChar c in CharList) text += c.ToString();
            return text;
        }

        /// <summary>
        /// Clears the contence of the word
        /// </summary>
        public void Clear()
        {
            CharList.Clear();
        }

        /// <summary>
        /// Returns the list of charecters the word is made up of
        /// </summary>
        /// <returns>List of type brailleChar</returns>
        public List<brailleChar> ToChar() => this.CharList;
    }

    public abstract class braillFont : IbrailleFont
    {
        /// <summary>
        /// Gets the name of the type of the implementation.
        /// </summary>
        public string TypeName => "Braille Font"; //string TypeName { get; }

        /// <summary>
        /// Gets a description of the type of the implementation.
        /// </summary>
        public string TypeDescription => "A Braille representaion of a string";  //string TypeDescription { get; }

        /// <summary>
        /// Gets a value indicating whether or not the current value is valid.
        /// </summary>
        public virtual bool IsValid => true; //bool IsValid { get; }

        /// <summary>
        /// Creates a string description of the current instance value
        /// </summary>
        /// <returns></returns>
        public override string ToString() => $"Braille: {Str}";

        /// <summary>
        /// Gets a string describing the state of "invalidness". If the instance is valid,
        ///  then this property should return Nothing or String.Empty.
        /// </summary>
        public string IsValidWhyNot => "I don't know";

        /// <summary>
        /// Spacing between the Braille charecters
        /// </summary>
        public double Spacing { get; private set; } = 2.5;

        /// <summary>
        /// Spacing between the individual cells
        /// </summary>
        public double Cell { get; private set; } = 6;

        /// <summary>
        /// Line Spacing
        /// </summary>
        public double LineSpacing { get; private set; } = 10;

        /// <summary>
        /// Braille as text string
        /// </summary>
        public string Str { get; private set; } = "";

        /// <summary>
        /// The type of Braille 6 or 8 point
        /// </summary>
        public brailleType Type { get; private set; } = 0;

        /// <summary>
        /// Braille as a list of Braille charecters of type brailleChar
        /// </summary>
        public List<brailleChar> Braille { get; private set; } = new List<brailleChar>();

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="str">Sting to be converted to Braille</param>
        /// <param name="spacing">Point spacing</param>
        /// <param name="cell">Cell spacing</param>
        /// <param name="lineSpacing">Line Spacing</param>
        /// <param name="type">The Braille type</param>
        public braillFont(string str, brailleType type = brailleType.sixDot, double spacing = 2.5, double cell = 6, double lineSpacing = 10)
        {
            this.Spacing = spacing;
            this.Cell = cell;
            this.LineSpacing = lineSpacing;
            this.Str = str;

            var bStr = new List<brailleChar>();

            if (type == brailleType.sixDot)
            {
                bStr.Add(new brailleChar(0b111111, brailleType.sixDot));
                foreach (char c in str)
                {
                    if (Char.IsUpper(c))
                    {
                        bStr.Add(new brailleChar(0b100000, brailleType.sixDot));
                        bStr.Add(new brailleChar(brailleUtil.Conversion(Char.ToLower(c)), brailleType.sixDot));
                    }
                    if (Char.IsDigit(c))
                    {
                        bStr.Add(new brailleChar(0b111100, brailleType.sixDot));
                        bStr.Add(new brailleChar(brailleUtil.Conversion(Char.ToLower(c)), brailleType.sixDot));
                    }
                    if (Char.IsWhiteSpace(c))
                    {
                        bStr.Add(new brailleChar(0b000000, brailleType.sixDot));
                    }
                    if (Char.IsLower(c))
                    {
                        bStr.Add(new brailleChar(brailleUtil.Conversion(c), brailleType.sixDot));
                    }
                }
            }
            if (type == brailleType.eightDot)
            {
                bStr.Add(new brailleChar(0b11111111, brailleType.eightDot));
                foreach (char c in str)
                {
                    bStr.Add(new brailleChar(brailleUtil.Conversion(c, brailleType.eightDot), brailleType.eightDot));
                }
            }

            this.Braille = bStr;
        }

        public List<brailleWord> extractWords(List<brailleChar> brailleChars)
        {
            var words = new List<brailleWord>();
            var word = new brailleWord();

            foreach (brailleChar c in brailleChars)
            {
                if (!c.IsSpace)
                {
                    word.Add(c);
                }
                else
                {
                    words.Add(new brailleWord(word));
                    word.Clear();
                }
            }
            words.Add(new brailleWord(word));

            return words;
        }

        public double wordLength(brailleWord word)
        {
            return word.Count * this.Cell;
        }

        public List<brailleChar> wordsToCharecters(List<brailleWord> words)
        {
            List<brailleChar> line = new List<brailleChar>();
            if (words.Count == 0) return line;

            foreach (brailleWord word in words)
            {
                line.AddRange(word.ToChar());
                line.Add(new brailleChar(0b00000000, this.Type));
            }
            line.RemoveAt(line.Count - 1);
            return line;
        }
    }

    public interface IbrailleFont
    {
        List<brailleChar> Braille { get; }
        brailleType Type { get; }
        double Spacing { get; }
        double Cell { get; }
        double LineSpacing { get; }
        string Str { get; }
    }

    public interface IbrailleWord
    {
        /// <summary>
        /// List of charecter that make up the word
        /// </summary>
        List<brailleChar> CharList
        { get; }

        /// <summary>
        /// The count of charecter is a word
        /// </summary>
        int Count
        { get; }

        /// <summary>
        /// String representation of the word
        /// </summary>
        /// <returns></returns>
        string ToString();

        /// <summary>
        /// Clears the contence of the word
        /// </summary>
        void Clear();

        /// <summary>
        /// Returns the list of charecters the word is made up of
        /// </summary>
        /// <returns>List of type brailleChar</returns>
        List<brailleChar> ToChar();
    }

    public interface IbrailleChar
    {
        byte Value { get; }
        brailleType type { get; }
    }

    public enum brailleType
    {
        [Description("Six dot Braille")]
        sixDot = 0,

        [Description("Eight dot Braille")]
        eightDot = 1,
    }

    public enum brailleLang
    {
        [Description("Standad English")]
        en = 0,

        [Description("Standad Danish")]
        dk = 1,

        [Description("Standad Danish")]
        de = 2,
    }
}