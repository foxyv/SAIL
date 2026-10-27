using SAIL.entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAIL.utils
{
    public class SAILModDataEncoder
    {
        /// <summary>
        /// Write the mod data to a versioned string in the SAIL Mod Data Format.
        /// </summary>
        /// <param name="modData">The mod data to serialize to a string.</param>
        /// <returns>A string containing the SAIL mod data.</returns>
        public static string EncodeSAILModData(SAILModData modData)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(ImmutableModHeader(modData)).Append("\n");

            if(modData.version == 1)
            {
                sb.Append(Version1ModDataBody(modData));
            } else
            {
                throw new Exception("Unknown SAILModData Version: " + modData.version);
            }

            return sb.ToString();
        }

        public static SAILModData DecodeSAILModData(string encoded)
        {
            
            throw new Exception("Not Yet Implemented");
        }

        private static string Version1ModDataBody(SAILModData modData)
        {
            StringBuilder sb = new StringBuilder();
            // Append the title of the section
            sb.Append("## Begin SaveEntityMappings\n");

            sb.Append("| savedIndex | modGUID | entityIdentifier |");
            sb.Append("| ---------- | ------- | ---------------- |");

            // Sort the list by the index so it looks good ish
            var cmp = Comparer<SaveEntityMapping>.Create((a,b) => a.savedIndex.CompareTo(b.savedIndex));
            var sorted = new List<SaveEntityMapping>(modData.saveEntityMappings);
            sorted.Sort(cmp);

            // Append tabular values to the body of the section.
            foreach(SaveEntityMapping mapping in sorted)
            {
                // Escape any special characters that can introduce unexpected problems like:
                // tabs, newlines, carriage returns, and backslashes.
                string escapedGUID = EscapeSpecialCharacters(mapping.modGUID);
                string escapedEntityID = EscapeSpecialCharacters(mapping.entityIdentifier);

                // Append the tab delimited row.
                sb.Append("| ").Append(mapping.savedIndex).Append(" | ")
                    .Append(escapedGUID).Append(" | ")
                    .Append(escapedEntityID).Append(" |");
            }

            // Return the encoded data
            return sb.ToString();
        }


        /// <summary>
        /// Write the header of the mod data string that will be the same no matter what version we use.
        /// </summary>
        /// <param name="modData">The mod data we are creating the header for.</param>
        /// <returns>The mod data header for the SAIL Mod Data.</returns>
        private static string ImmutableModHeader(SAILModData modData)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("# SAIL_MOD_DATA\n");
            sb.Append("VERSION: ").Append(modData.version).Append("\n");            
            return sb.ToString();
        }

        private static string ImmutableModTrailer()
        {
            return "\n# END_SAIL_MOD_DATA";
        }

        /// <summary>
        /// Escape special characters in modGUIDs and entity IDs so they don't break our tables.
        /// </summary>
        /// <param name="value">The value to escape.</param>
        /// <returns>The value with special characters escaped using a backslash.</returns>
        public static string EscapeSpecialCharacters(string value)
        {
            StringBuilder sb = new StringBuilder();
            foreach(char c in value)
            {
                switch(c)
                {
                    case '\n':
                        sb.Append("\\n");
                        break;
                    case '\t':
                        sb.Append("\\t");
                        break;
                    case '\r':
                        sb.Append("\\r");
                        break;
                    case '|':
                        sb.Append("\\|");
                        break;
                    case '\\':
                        sb.Append("\\\\");
                        break;
                    default:
                        sb.Append(c);
                        break;
                }
            }

            return sb.ToString();
        }

        public static string UnescapeSpecialCharacters(string value)
        {
            Boolean escaped = false;

            StringBuilder sb = new StringBuilder();
            foreach (char c in value)
            {
                // If the previous character was a backslash trigger the escape logic
                if (escaped)
                {
                    // Reset the escape flag
                    escaped = false;
                    switch(c)
                    {
                        case 't':
                            sb.Append("\t");
                            break;
                        case 'n':
                            sb.Append("\n");
                            break;
                        case 'r':
                            sb.Append("\r");
                            break;
                        case '\\':
                            sb.Append("\\");
                            break;
                        case '|':
                            sb.Append("|");
                            break;
                        default:
                            throw new FormatException("Illegal escape character.");
                    }

                    // Continue looping through the characters
                    continue;
                }

                // If we detect a backslash, trigger the escape flag
                if(c == '\\')
                {
                    escaped = true;
                    continue;
                }

                sb.Append(c);

            }

            if(escaped)
            {
                throw new FormatException("Dangling backslash found with no corresponding character. EG: Not \\t, \\n, or \\r");
            }

            return sb.ToString();
        }
        
    }
}
