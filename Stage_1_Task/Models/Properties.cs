
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Stage_1_Task.Models
{
    public class Properties
    {
        private Properties(string value)
        {
            Length = value.Length;
            Is_palindrome = IsPalindrome(value);
            Unique_characters = GetUniqueCharacterCount(value);
            Word_count = GetWordCount(value);
            Sha256_hash = GenerateSha256(value);
            Character_frequency_map = GenerateCharacterMapper(value);
        }

        public int Length { get; private set; }
        public bool Is_palindrome { get; private set; }
        public int Unique_characters { get; private set; }
        public int Word_count { get; private set; }
        public string Sha256_hash { get; private set; }
        public object Character_frequency_map { get; private set; }

        private bool IsPalindrome(string value)
        {
            var cleaned = value.ToLower();
            return cleaned.SequenceEqual(cleaned.Reverse());
        }

        private int GetUniqueCharacterCount(string value)
        {
            List<char> seenChar = new List<char>();
            int uniqueCharacterCount = 0;
            foreach (char ch in value)
            {
                if (!seenChar.Contains(ch))
                {
                    uniqueCharacterCount++;
                    seenChar.Add(ch);
                }
            }
            return uniqueCharacterCount;
        }

        private object GenerateCharacterMapper(string value)
        {
            Dictionary<char, int> keyValuePairs = new Dictionary<char, int>();
            foreach (char ch in value)
            {
                keyValuePairs[ch] = keyValuePairs.ContainsKey(ch)? keyValuePairs[ch] + 1: 1;
            }
            return keyValuePairs;
        }

        private string? GenerateSha256(string value)
        {
            using SHA256 sha256 = SHA256.Create();
            return Convert.ToHexString(sha256.ComputeHash(Encoding.UTF8.GetBytes(value))).ToLower();
        }

        private int GetWordCount(string value)
        {
            return Regex.Matches(value, @"\b[\w'-_]+\b").Count();
        }

        public static Properties Create(string value) => new Properties(value);
    }
}
