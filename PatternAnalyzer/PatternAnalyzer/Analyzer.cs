using System;
using System.Drawing;
using System.Linq;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;

namespace PatternAnalyzer
{
    public class Analyzer
    {
        private Mat BinarizeImage( IInputArray frame, double threshold, double maxValue, ThresholdType thresholdType )
        {
            var binaryImage = new Mat( );
            
            // Binarize the image martix.         
            CvInvoke.Threshold( frame, binaryImage, threshold, maxValue, thresholdType );
            return binaryImage;
        }

        private void FindTopPaperEdge( IImage frame, out LineSegment2DF paperEdge, out float paperSlope )
        {
            // Threshold and invert the image in order
            // to detect the edge of the paper.
            using( var image = BinarizeImage( frame, 90, 255, ThresholdType.BinaryInv ).ToImage<Gray, byte>( ) )
            {
                // Detect the edge(s) of the paper.
                var lines = image.PyrDown( )
                    .PyrUp( )
                    .Canny( 200, 255 )
                    .Dilate( 3 )
                    .HoughLinesBinary( rhoResolution : 1,
                        thetaResolution : Math.PI / 45.0,
                        threshold : 50,
                        minLineWidth : 100,
                        gapBetweenLines : 1 )[ 0 ];

                // The HoughLinesBinary method was returning multiple lines
                // which represented the edge of the paper. This was causing 
                // an issue when trying to determine the equation and slope
                // of the line. To correct this the left most Point and the
                // right most Point were taken in order to create only 1 line
                // in which the line equation and slope could be derived from.
                using( var img = new Image<Bgr, byte>( frame.Size ) )
                {
                    // Find the smallest X coordinate.
                    var min = lines.Min( line => line.P1.X );

                    // Find the largest X coordinate.
                    var max = lines.Max( line => line.P2.X );

                    // Find the Point which equals the smallest
                    // X coordinate found above.
                    var startPoint = lines.FirstOrDefault( line => line.P1.X == min ).P1;

                    // Find the Point which equals the largest
                    // X coordinate found above.
                    var endPoint = lines.FirstOrDefault( line => line.P2.X == max ).P2;

                    // Create a new line segment from the Point from above
                    // which will represent the edge of the paper.
                    paperEdge = new LineSegment2DF( startPoint, endPoint );

                    // Determine the slope (if any) of the line representing
                    // the paper edge.
                    paperSlope = ( ( paperEdge.P2.Y - paperEdge.P1.Y ) / ( paperEdge.P2.X - paperEdge.P1.X ) );

                    // Draw the paper edge line on the image for
                    // presentation purposes only.
                    img.Draw( paperEdge, new Bgr( Color.Red ), 8 );
//                    img.Or( threshold.ToImage<Bgr, byte>( ) ).ToBitmap( ).Save( "" );
                }
            }

        }
    }
}