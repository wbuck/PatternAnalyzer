using Emgu.CV;
using Emgu.CV.Cvb;
using Emgu.CV.Structure;
using PatternAnalyzer.Structures;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace PatternAnalyzer
{
    public class Analyzer
    {
        /// <summary>
        /// Finds blobs with the specified min and max area.
        /// </summary>
        /// <param name="src">Source image</param>
        /// <param name="minArea">The minimum allowable area</param>
        /// <param name="maxArea">The maximum allowable area</param>
        /// <returns>List containing the X and Y coordinates of the found blobs</returns>
        public IEnumerable<PointF> FindBlobs( Bitmap src, uint minArea, uint maxArea )
        {
            if( src == null )
            {
                throw new ArgumentNullException( nameof( src ), @"Source image cannot be null" );
            }

            using( var sourceImage = new Image<Gray, byte>( src ) )
            using( var blobDetector = new CvBlobDetector( ) )
            using( var blobs = new CvBlobs( ) )
            {
                Image<Gray, byte> filteredSrc = null;

                try
                {
                    // Binarize and invert the image so that
                    // that blob detector can locate the dots.
                    filteredSrc = sourceImage.ThresholdBinaryInv( new Gray( 90 ), new Gray( 255 ) );

                    // Finds all blobs in the input image and
                    // stores them in to the CvBlobs structure.
                    blobDetector.Detect( filteredSrc, blobs );

                    // Filter the blobs by area. The alignment dots
                    // have an average area of roughly 3500 pixels.
                    blobs.FilterByArea( ( int )minArea, ( int )maxArea );

                    // Return the centroids of each blob.
                    return blobs.Values.Select( b => new PointF( b.Centroid.X, b.Centroid.Y ) );
                }
                finally
                {
                    filteredSrc?.Dispose( );
                }
            }
        }

        /// <summary>
        /// Find the strongest edge in the source image.
        /// </summary>
        /// <param name="src">Source image</param>
        /// <returns>Object which contains the start and end point of the line</returns>
        public LineSegment2D FindEdge( Bitmap src )
        {
            if( src == null )
            {
                throw new ArgumentNullException( nameof( src ), @"Source image cannot be null" );
            }

            using( var sourceImage = new Image<Gray, byte>( src ) )
            {
                var edge = default( LineSegment2D );
                Image<Gray, byte> filteredSrc = null;

                try
                {
                    // Filter the source image. First I down sample
                    // the image to smooth out any noise. I then
                    // apply canny to the image (essentially binarizing)
                    // the image before I apply I dilate filter to help
                    // accentuate any contours.
                    filteredSrc = sourceImage.PyrDown( )
                        .PyrUp( )
                        .Canny( 100, 200 )
                        .Dilate( 3 );

                    // Find all lines in the filtered image.
                    // Note that the treshold parameter is 
                    // actually refering to the number of 
                    // required points (length) for a line.
                    var edges = filteredSrc.HoughLinesBinary( rhoResolution : 1,
                        thetaResolution : Math.PI / 180,
                        threshold : 500,
                        minLineWidth : 30,
                        gapBetweenLines : 50 )[ 0 ];

                    if( edges.Length > 0 )
                    {
                        // Find the edge with the longest length
                        // from the list of found edges.
                        edge = edges.Aggregate( ( edg1, edg2 ) => edg1.Length > edg2.Length ? edg1 : edg2 );
                    }
                    else
                    {
                        // No edges were found so return a
                        // default edge structure.
                        return edge;
                    }
                }
                finally
                {
                    filteredSrc?.Dispose( );
                }
                return edge;
            }
        }

        public RowPoints[ , ] SortPointsInToRowsAndColumns( IEnumerable<PointF> points )
        {
            if( points == null )
            {
                throw new ArgumentNullException( nameof( points ), @"Points collection cannot be null." );
            }

            var pointFs = points as IList<PointF> ?? points.ToList( );

            // Ensure there is 10 points per row.
            if( pointFs.Count % 10 != 0 )
            {
                return new RowPoints[ 0, 0 ];
            }
            // There are 10 dots per full row
            // on the alignment pattern.
            const int pointsPerFullRow = 10;

            // Split each row in to columns of
            // 5 points (alignment dots).
            const int numberOfColumns = 2;

            // Sort the points based on their Y coordinate
            // to make it eaiser to split the rows apart.
            var sorted = pointFs.OrderBy( p => p.Y ).ToList( );

            // Determine the number of rows found in the
            // alignment image.
            var numberOfRows = sorted.Count / pointsPerFullRow;

            // Create the structure which will hold each
            // row and column of dots.
            var splitPoints = new RowPoints[ numberOfRows, 2 ];

            for( int row = 0; row < numberOfRows; row++ )
            {
                // Grab the next row of points (alignment dots).
                var nextRow = sorted.GetRange( row * pointsPerFullRow, pointsPerFullRow ).OrderBy( p => p.X );

                for( int column = 0; column < numberOfColumns; column++ )
                {
                    // Split the row in to columns consisting
                    // of 5 points (alignment dots) per column.
                    var nextColumn = nextRow
                        .ToList( )
                        .GetRange( column * 5, 5 );

                    splitPoints[ row, column ] = new RowPoints( row, column, nextColumn );
                }
            }
            return splitPoints;
        }

        /// <summary>
        /// Finds the average X and Y coordinate 
        /// in a list of points.
        /// </summary>
        /// <param name="points">List of points to average</param>
        /// <returns>The average point</returns>
        public PointF FindAveragePoint( IEnumerable<PointF> points )
        {
            if( points == null )
            {
                throw new ArgumentNullException( nameof( points ), "Collection cannot be null" );
            }

            var pointFs = points as IList<PointF> ?? points.ToList( );

            if( !pointFs.Any( ) )
            {
                return default( PointF );
            }

            return new PointF( pointFs.Average( p => p.X ), pointFs.Average( p => p.Y ) );
        }

        public double CalculateDistanceBetweenPointsX( PointF start, PointF end )
        {
           
            return 0;
        }
    }
}