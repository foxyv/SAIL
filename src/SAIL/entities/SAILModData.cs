using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAIL.entities
{
    /// <summary>
    /// When Sailwind makes a savegame file, it includes a section for ModData. This is what we will put there (Encoded as something or rather)
    /// </summary>
    [ Serializable ]
    public class SAILModData
    {
        public readonly int version = 1;

        /// <summary>
        /// A list of indexes and what they map to in the currentItems table.
        /// </summary>
        public List<SaveEntityMapping> saveEntityMappings = new List<SaveEntityMapping>();

        public SAILModData()
        {
        }

        public override bool Equals(object obj)
        {
            var data = obj as SAILModData;
            return data != null &&
                   version == data.version &&
                   EqualityComparer<List<SaveEntityMapping>>.Default.Equals(saveEntityMappings, data.saveEntityMappings);
        }

        public override int GetHashCode()
        {
            var hashCode = -1372541541;
            hashCode = hashCode * -1521134295 + version.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<List<SaveEntityMapping>>.Default.GetHashCode(saveEntityMappings);
            return hashCode;
        }
    }
}
