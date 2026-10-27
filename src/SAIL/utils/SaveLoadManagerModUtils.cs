using System;
using System.Threading;

namespace SAIL.utils
{
    /// <summary>
    /// This class' intended purpose is to provide a set of methods that safely manipulate the SaveLoadManager's arrays.
    /// </summary>
    static class SaveLoadManagerModUtils
    {
        /// <summary>
        /// This semaphore gates the resize of the arrays and let's us prevent race conditions for any mods that use SAIL. (Later we'll guard the array from manipulation outside this method)
        /// </summary>
        private static readonly SemaphoreSlim Gate = new SemaphoreSlim(1, 1);

        /// <summary>
        /// <para>Ensure that the "currentObjects" array in the SaveLoadManager has at least N available indexes. EG: N = 512 then the array will have indexes [0, 511]</para>
        /// 
        /// <para>NOTE: The current objects array is an array of objects that are included in the savegame. They usually include ships, anchors, houses, and such.</para>
        /// 
        /// </summary>
        /// <param name="newMaximumSize">The new size to ensure that the load manager has.</param>
        /// <param name="currentObjects">A reference to the current objects array game.</param> 
        public static int EnsureSaveableObjectCapacity(int newMaximumSize, ref SaveableObject[] currentObjects)
        {
            if (currentObjects.Length >= newMaximumSize)
            {
                // The array is already big enough. Leave without doing anything or waiting for the lock.
                // The array size will only increase so we can assume that it won't get smaller than newMaximumSize
                return 0;
            }

            // Only allow one thread to call this method at any given moment.
            Gate.Wait();
            try {
                int previousMaxSize = currentObjects.Length;
                // Check again after acquiring the lock (Another thread may have already made it big enough)
                if (currentObjects.Length >= newMaximumSize)
                {
                    // The array is already big enough. Leave without doing anything.
                    return 0;
                }

                // Create a new array with sufficient space and copy the existing array to this bigger one.
                var bigger = new SaveableObject[newMaximumSize];
                Array.Copy(currentObjects, bigger, currentObjects.Length);

                // Replace the existing array with the bigger one
                currentObjects = bigger;

                // Log the increase.
                Plugin.Log.LogInfo($"currentObjects grown to {newMaximumSize} slots.");

                return newMaximumSize - previousMaxSize;
            } finally {
                Gate.Release();
            }
        }
        





    }
}
