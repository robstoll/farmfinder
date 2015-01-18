using System;

namespace CH.Tutteli.FarmFinder.Dtos
{
    /// <summary>
    /// Represents some basic information about a farm.
    /// </summary>
    [Serializable]
    public class FarmLocationDto
    {
        /// <summary>
        /// Name of the farm as registered in farm finder.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Latitude of the farm using -90/90 convention.
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        /// Longitude of the farm using -180/180 convention.
        /// </summary>
        public double Longitude { get; set; }
    }
}