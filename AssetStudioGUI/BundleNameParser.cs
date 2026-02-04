using System;
using System.Text.RegularExpressions;

namespace AssetStudioGUI
{
    /// <summary>
    /// Helper class to parse bundle filenames and extract character and variant information
    /// Expected format: art_live2d_characters_{charactername}_{variantid}.bundle
    /// </summary>
    public class BundleNameParser
    {
        public string CharacterName { get; private set; }
        public string VariantId { get; private set; }
        public string FullIdentifier { get; private set; }
        public bool IsValid { get; private set; }

        // Regex patterns for different bundle types
        private static readonly Regex ArtPattern = new Regex(@"art_live2d_characters_([^_]+)_(\d+)\.bundle", RegexOptions.IgnoreCase);
        private static readonly Regex AnimationPattern = new Regex(@"build_animations2d_characters_([^_]+)_(\d+)\.bundle", RegexOptions.IgnoreCase);
        private static readonly Regex PrefabPattern = new Regex(@"build_prefabs_live2d_characters_char2d_([^_]+)_(\d+)\.bundle", RegexOptions.IgnoreCase);

        public BundleNameParser(string filename)
        {
            IsValid = false;
            
            if (string.IsNullOrEmpty(filename))
                return;

            // Try to match any of the patterns
            Match match = ArtPattern.Match(filename);
            if (!match.Success)
                match = AnimationPattern.Match(filename);
            if (!match.Success)
                match = PrefabPattern.Match(filename);

            if (match.Success && match.Groups.Count >= 3)
            {
                CharacterName = match.Groups[1].Value;
                VariantId = match.Groups[2].Value;
                FullIdentifier = $"{CharacterName}_{VariantId}";
                IsValid = true;
            }
        }

        /// <summary>
        /// Check if this bundle belongs to the specified character and variant
        /// </summary>
        public bool MatchesCharacter(string characterName, string variantId)
        {
            if (!IsValid)
                return false;

            return string.Equals(CharacterName, characterName, StringComparison.OrdinalIgnoreCase) &&
                   string.Equals(VariantId, variantId, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Check if this bundle belongs to the same character/variant as another parser
        /// </summary>
        public bool MatchesParser(BundleNameParser other)
        {
            if (!IsValid || !other.IsValid)
                return false;

            return MatchesCharacter(other.CharacterName, other.VariantId);
        }

        /// <summary>
        /// Try to parse a filename and return the parser if successful
        /// </summary>
        public static bool TryParse(string filename, out BundleNameParser parser)
        {
            parser = new BundleNameParser(filename);
            return parser.IsValid;
        }

        public override string ToString()
        {
            return IsValid ? FullIdentifier : "Invalid";
        }
    }
}