using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Emgu.CV;
using Emgu.CV.Cvb;
using Emgu.CV.Structure;
using PatternAnalyzer.Structures;

namespace PatternAnalyzer
{
    public class Analyzer
    {
        public IEnumerable<PointF> FindBlobs( Bitmap src, uint minArea, uint maxArea )
        {
            if( src == null )
            {
                throw new ArgumentNullException( nameof( src ), @"Source image cannot be null" );
            }

            using( var sourceImage = new Image<Gray, byte>( src ) )
            using( var blobDetector = new CvBlobDetector( ) )
            using( var blobs = new CvBlobs( ))
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

        public RowPoints[ , ] SplitPoints( IEnumerable<PointF> points )
        {
            // There are 10 dots per full row
            // on the alignment pattern.
            const int pointsPerFullRow = 10;

            const int numberOfColumns = 2;

            // Sort the points based on their Y coordinate
            // to make it eaiser to split the rows apart.
            var sorted = points.OrderBy( p => p.Y ).ToList( );

            // Determine the number of rows found in the
            // alignment image.
            var numberOfRows = sorted.Count / pointsPerFullRow;

            // Create the structure which will hold each
            // row and column of dots.
            var splitPoints = new RowPoints[ numberOfRows, 2 ];

            for( int row = 0; row < numberOfRows; row++ )
            for( int column = 0; column < numberOfColumns; column++ )          
            {
                var next = row * pointsPerFullRow;
                var r =
                    sorted.GetRange( next, pointsPerFullRow )
                        .OrderBy( p => p.X )
                        .ToList( )
                        .GetRange( column * 5, 5 );

                splitPoints[ row, column ] = new RowPoints( row, column, r );
            }



            return splitPoints;
        }

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
    }
}