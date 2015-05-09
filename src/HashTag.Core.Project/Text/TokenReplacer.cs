using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HashTag.Text
{
    /// <summary>
    /// Replaces delimited token(s) within string with user defined values
    /// </summary>
    public class TokenReplacer
    {

        /// <summary>
        /// Default constructor
        /// </summary>
        public TokenReplacer()
        {
            Delimiters = new List<Delimiter>();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="replacementLookup">Action to lookup replacement value with a delimiter is found</param>
        /// <param name="delimiters">Paired delimiters that define token(s) within the target string</param>
        public TokenReplacer(Func<FoundToken, string> replacementLookup, params string[] delimiters)
            : this(delimiters)
        {
            ReplacementLookup = replacementLookup;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="delimiters">Paired non-null, non-empty delimiters that define token(s) within the target string</param>
        public TokenReplacer(params string[] delimiters)
            : this()
        {
            if (delimiters == null || delimiters.Length == 0)
                return;
            if (delimiters.Length % 2 != 0)
            {
                throw new ArgumentException("Delimiters must be in begin,end matching pairs");
            }
            int x = 0;
            while (x < delimiters.Length - 1)
            {
                Delimiters.Add(new Delimiter(delimiters[x], delimiters[x + 1]));
                x += 2;
            }
        }

        /// <summary>
        /// List of token delimiter pairs to identify within the target string
        /// </summary>
        public List<Delimiter> Delimiters { get; set; }

        /// <summary>
        /// Action to take when a token is found withing the target string
        /// </summary>
        public Func<FoundToken, string> ReplacementLookup { get; set; }

        /// <summary>
        /// Perform the actual replacement of tokens within the string
        /// </summary>
        /// <param name="inputString">String containing tokes to replace</param>
        /// <param name="tokenLookup">Using this lookup action instead of one configured at the class level</param>
        /// <returns>Transformed string or the same string if no tokens found to replace</returns>
        public string Execute(string inputString, Func<FoundToken, string> tokenLookup = null)
        {
            if (string.IsNullOrEmpty(inputString))
                return inputString;

            var useThisLookup = tokenLookup ?? ReplacementLookup;
            if (useThisLookup == null)
            {
                throw new InvalidOperationException(string.Format("Required ReplacementLookup({0}) is not initialized or not provided in method call.", typeof(FoundToken).Name));
            }

            var replacedString = inputString;
            var lastDelimiter = findLastDelimiter(replacedString);
            while (lastDelimiter != null)
            {
                string beginToken = lastDelimiter.Item1.Begin;
                string endToken = lastDelimiter.Item1.End;
                var startPos = lastDelimiter.Item2;

                var evt = new FoundToken();
                evt.Delimiter = lastDelimiter.Item1;
                var stopPos = replacedString.IndexOf(endToken, startPos);
                if (stopPos < 0)
                {
                    throw new InvalidOperationException(string.Format("Unable to find matching closing delimiter '{0}' for opening delimiter '{1}'",
                        endToken, beginToken));
                }
                evt.InternalToken = replacedString.Substring(startPos + beginToken.Length, stopPos - startPos - beginToken.Length);
                evt.OuterToken = beginToken + evt.InternalToken + endToken;

                var replacementValue = useThisLookup(evt);

                replacedString = replacedString.Remove(startPos, evt.OuterToken.Length);
                replacedString = replacedString.Insert(startPos, replacementValue);

                lastDelimiter = findLastDelimiter(replacedString);
            }

            return replacedString;

        }

        /// <summary>
        /// Convienience wrapper to replace tokens in a string
        /// </summary>
        /// <param name="inputString">String containing 0..n tokens to replace</param>
        /// <param name="replacementLookup">Action to perform when a token is discovered within a string</param>
        /// <param name="delimiters">Delimiters to use to identify tokens in string to replace</param>
        /// <returns>Transformed string or <paramref name="inputString"/> if no tokens found to replace</returns>
        public static string Replace(string inputString, Func<FoundToken, string> replacementLookup, params string[] delimiters)
        {
            TokenReplacer replacer = new TokenReplacer(replacementLookup, delimiters);
            return replacer.Execute(inputString);
        }

        private Tuple<Delimiter, int> findLastDelimiter(string replacedString)
        {
            if (Delimiters == null || Delimiters.Count == 0)
            {
                throw new InvalidOperationException("Delimiters collection must be set prior to attempting a replace operation");
            }
            var strIndex = -1;
            var delimiterIndex = -1;
            for (int x = 0; x < Delimiters.Count; x++)
            {
                var delim = Delimiters[x];
                var localStrIndex = replacedString.LastIndexOf(delim.Begin);
                if (localStrIndex > strIndex)
                {
                    strIndex = localStrIndex;
                    delimiterIndex = x;
                }
            }
            return delimiterIndex > -1 ? new Tuple<Delimiter, int>(Delimiters[delimiterIndex], strIndex) : null;
        }
        /// <summary>
        /// Details about the token that was discovered in the target string
        /// </summary>
        public class FoundToken
        {
            /// <summary>
            /// Delimiter (e.g. '$[',']' ) of token that was discovered
            /// </summary>
            public Delimiter Delimiter { get; set; }

            /// <summary>
            /// Token text without delimiters ( e.g. $[myToken] value is 'myToken' )
            /// </summary>
            public string InternalToken { get; set; }

            /// <summary>
            /// Token text with delimiters that was found (e.g. $[myToken] )
            /// </summary>
            public string OuterToken;
        }

        /// <summary>
        /// Defines beginning and ending character sequences for a token
        /// </summary>
        public class Delimiter
        {
            /// <summary>
            /// Default contructor
            /// </summary>
            public Delimiter()
            {
            }
            public Delimiter(string begin, string end)
            {
                Begin = begin;
                End = end;
            }

            private string _begin;
            /// <summary>
            /// Start of token delimiter (e.g. '$[' )
            /// </summary>
            public string Begin
            {
                get
                {
                    return _begin;
                }
                set
                {
                    if (string.IsNullOrEmpty(value))
                    {
                        throw new ArgumentException("Delimiter.Begin must not be null or empty");
                    }
                    _begin = value;
                }
            }

            private string _end;
            /// <summary>
            /// End of token delimiter (e.g. ']' )
            /// </summary>
            public string End
            {
                get
                {
                    return _end;
                }
                set
                {
                    if (string.IsNullOrEmpty(value))
                    {
                        throw new ArgumentException("Delimiter.End must not be null or empty");
                    }
                    _end = value;
                }
            }

            public override string ToString()
            {
                return string.Format("{0}myToken{1}", Begin, End);
            }
        } //class Delimiter

    }
}

/*
 * 
*/