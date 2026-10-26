using System;
using System.Collections.Generic;

namespace SAIL.entities
{
    /// <summary>
    /// When the game saves ships, houses, anchors, and such to "The Save File" it uses the "SaveLoadManager.currentObjects[]" array indexes
    /// at the time that the save was generated. Sometimes those indexes will change out from under the save (New Mods, removing existing mods, game update, etc...)
    /// 
    /// There is no information in vanilla what is saved in those indexes!
    /// 
    /// The SaveEntityMapping keeps track of "which items are where" when the save was created so we can map them back to the correct indexes when those indexes change.
    /// 
    /// EG: If the Leopard is on index 244 when the game is saved, then moves to index 592 before it is loaded; we can change the index when we load the save by matching it.
    /// 
    /// </summary>
    public class SaveEntityMapping
    {

        /// <summary>
        /// This is the index of the item in the game when the save file was created.
        /// </summary>
        public readonly int savedIndex;

        /// <summary>
        /// This is the unique-to-mod identifier for the Ship, Anchor, Buoy, or whatever that was in the "savedIndex"
        /// </summary>
        public readonly String entityIdentifier;

        /// <summary>
        /// This is the mod GUID that created the saved item. EG: com.dizzy.petstore
        /// </summary>
        public readonly String modGUID;

        public SaveEntityMapping(int savedIndex, string entityIdentifier, string modGUID)
        {
            this.savedIndex = savedIndex;
            this.entityIdentifier = entityIdentifier ?? throw new ArgumentNullException(nameof(entityIdentifier));
            this.modGUID = modGUID ?? throw new ArgumentNullException(nameof(modGUID));
        }

        public override bool Equals(object obj)
        {
            var mapping = obj as SaveEntityMapping;
            return mapping != null &&
                   savedIndex == mapping.savedIndex &&
                   entityIdentifier == mapping.entityIdentifier &&
                   modGUID == mapping.modGUID;
        }

        public override int GetHashCode()
        {
            var hashCode = 1827568439;
            hashCode = hashCode * -1521134295 + savedIndex.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(entityIdentifier);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(modGUID);
            return hashCode;
        }
    }

}
