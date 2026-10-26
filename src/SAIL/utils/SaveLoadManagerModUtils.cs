using System;
using System.Threading;

namespace SAIL.utils
{
    /// <summary>
    /// This class' intended purpose is to provide a set of methods that safely manipulate the SaveLoadManager's arrays.
    /// </summary>
    static class SaveLoadManagerModUtils
    {
        private static readonly SemaphoreSlim Gate = new SemaphoreSlim(1, 1);

        /// <summary>
        /// <para>Ensure that the "currentObjects" array in the SaveLoadManager has at least N available indexes. EG: N = 512 then the array will have indexes [0, 511]</para>
        /// 
        /// <para>NOTE: The current objects array is an array of objects that are included in the savegame. They usually include ships, anchors, houses, and such.</para>
        /// 
        /// </summary>
        /// <param name="newMaximumSize">The new size to ensure that the load manager has.</param>
        /// <param name="currentObjects">A reference to the current objects array game.</param> 
        public static void EnsureSaveableObjectCapacity(int newMaximumSize, ref SaveableObject[] currentObjects)
        {
            // Only allow one thread to call this method at any given moment.
            Gate.Wait();
            try { 
                if (currentObjects.Length >= newMaximumSize)
                {
                    // The array is already big enough.
                    return;
                }

                var bigger = new SaveableObject[newMaximumSize];
                Array.Copy(currentObjects, bigger, currentObjects.Length);
                currentObjects = bigger;
                Plugin.Log.LogInfo($"currentObjects grown to {newMaximumSize} slots.");
            } finally {
                Gate.Release();
            }
        }
        



    }
}
