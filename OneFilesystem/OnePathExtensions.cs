#region OneFilesystem
// OneFilesystem
// (to rule them all... Or at least some...)
// https://github.com/ArxOne/OneFilesystem
// Released under MIT license http://opensource.org/licenses/MIT
#endregion

namespace ArxOne.OneFilesystem
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Extensions to <see cref="OnePath"/>
    /// </summary>
    public static class OnePathExtensions
    {
        /// <summary>
        /// Gets the self and ancestors.
        /// </summary>
        /// <param name="onePath">The one path.</param>
        /// <returns></returns>
        public static IEnumerable<OnePath> GetSelfAndAncestors(this OnePath onePath)
        {
            for (var currentPath = onePath; currentPath != null; currentPath = currentPath.GetParent())
                yield return currentPath;
        }

        /// <summary>
        /// Determines whether this path is localhost.
        /// </summary>
        /// <param name="onePath">The one path.</param>
        /// <returns></returns>
        public static bool IsLocalhost(this OnePath onePath)
        {
            return onePath.Host != null && string.Equals(onePath.Host, "localhost", StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
