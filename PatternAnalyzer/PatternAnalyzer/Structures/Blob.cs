using System.Collections.Generic;
using System.Drawing;

namespace PatternAnalyzer.Structures
{
    public class Blob
    {
        public Blob( PointF centroid, int area, Rectangle boundingBox, IEnumerable<Point> contour )
        {
            Centroid = centroid;
            Area = area;
            BoundingBox = boundingBox;
            Contour = new List<Point>( contour );
        }

        /// <summary>
        /// The centroid of the blob
        /// </summary>
        public PointF Centroid { get; }

        /// <summary>
        /// The number of pixels in this blob
        /// </summary>
        public int Area { get; }

        /// <summary>
        /// The minimum bounding box of the blob
        /// </summary>
        public Rectangle BoundingBox { get; }

        /// <summary>
        /// Get the contour that defines the blob
        /// </summary>
        public IList<Point> Contour { get; }
    }
}
