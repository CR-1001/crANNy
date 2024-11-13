/* 
    This file is part of crANNy. Copyright (C) 2017 Christian Rauch.
    Distributed under terms of the GPL3 license.
*/

namespace CRAI.Common
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.IO;
    using System.Security.Cryptography;


    public static class FileSystem
    {
        private static volatile Regex _RegexIllegalPathCharacters;

        public static String CleanUpStringForPath(
            String input,
            String replacement,
            String replacementIfEmpty,
            bool appendHashPostfixIfReplacedAnything)
        {
            if (String.IsNullOrEmpty(input))
            {
                return replacementIfEmpty;
            }

            if (_RegexIllegalPathCharacters == null) // Double-Checked-Locking
            {
                lock (typeof(FileSystem))
                {
                    if (_RegexIllegalPathCharacters == null)
                    {
                        String regexPatternIllegalPathCharacters
                            = new String(Path.GetInvalidFileNameChars())
                            + new String(Path.GetInvalidPathChars());

                        _RegexIllegalPathCharacters
                            = new Regex(
                                String.Format("[{0}]", Regex.Escape(regexPatternIllegalPathCharacters)),
                                RegexOptions.Compiled);
                    }
                }

            }

            var output = _RegexIllegalPathCharacters.Replace(input, replacement);

            if (String.IsNullOrWhiteSpace(output))
            {
                output = replacementIfEmpty;
            }

            if (appendHashPostfixIfReplacedAnything && input != output)
            {
                using (var hashAlgorithm = new SHA1Managed())
                {
                    byte[] hash = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(input));

                    var hashChain
                        = hash.Any()
                        ? hash.Select(h => h.ToString()).Aggregate((h1, h2) => h1 + h2)
                        : "0";

                    output = String.Format("{0}_{1}", output, hashChain);
                }
            }

            return output;
        }

    }
}
