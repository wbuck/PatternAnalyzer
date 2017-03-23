using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Emgu.CV;
using Emgu.CV.Cvb;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using PatternAnalyzer.Structures;

namespace PatternAnalyzer
{
    public class Analyzer
    {
        public IEnumerable<Blob> FindBlobs( Bitmap src, uint minArea, uint maxArea )
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

                    return blobs.Values.Select( b => new Blob( b.Centroid, b.Area, b.BoundingBox, b.GetContour( ) ) );
                }
                finally
                {
                    filteredSrc?.Dispose( );
                }
            }
        }
    }


}