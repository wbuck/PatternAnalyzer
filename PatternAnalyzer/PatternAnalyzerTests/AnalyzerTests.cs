using System;
using System.Collections.Generic;
using System.Drawing;
using Emgu.CV.Structure;
using PatternAnalyzer;
using PatternAnalyzerTests.Properties;
using Xunit;

namespace PatternAnalyzerTests
{
    public class AnalyzerTests
    {
        [Fact]
        public void FindBlobsTestWithTwentyBlobs( )
        {
            var image = ( Bitmap )Resources.ResourceManager.GetObject( "GoodImageTwentyDots" );
            const uint minArea = 2000;
            const uint maxArea = 5000;
            const int expectedBlobsFound = 20;

            var analyzer = new Analyzer( );

            var result = analyzer.FindBlobs( image, minArea, maxArea );
            Assert.Equal( expectedBlobsFound, result.Count );
        }

        [Fact]
        public void FindBlobsTestWithNoBlobs( )
        {
            var image = ( Bitmap )Resources.ResourceManager.GetObject( "BadImageNoDots" );
            const uint minArea = 2000;
            const uint maxArea = 5000;
            const int expectedBlobsFound = 0;

            var analyzer = new Analyzer( );

            var result = analyzer.FindBlobs( image, minArea, maxArea );
            Assert.Equal( expectedBlobsFound, result.Count );
        }

        [Fact]
        public void FindBlobsTestWithNull( )
        {
            Bitmap image = null;
            const uint minArea = 2000;
            const uint maxArea = 5000;

            var analyzer = new Analyzer( );

            var exception = Record.Exception( ( ) => {
                analyzer.FindBlobs( image, minArea, maxArea );
            } );

            Assert.NotNull( exception );
            Assert.IsType<ArgumentNullException>( exception );

        }

        [Fact]
        public void FindEdgeTestWithEdge( )
        {
            var image = ( Bitmap )Resources.ResourceManager.GetObject( "GoodImageTwentyDots" );
            var analyzer = new Analyzer( );

            var result = analyzer.FindEdge( image );

            Assert.NotEqual( default( LineSegment2D ), result );
        }

        [Fact]
        public void FindEdgeTestWithNoEdge( )
        {
            var image = ( Bitmap )Resources.ResourceManager.GetObject( "BadImageNoEdge" );
            var analyzer = new Analyzer( );

            var result = analyzer.FindEdge( image );

            Assert.Equal( default( LineSegment2D ), result );
        }

        [Fact]
        public void FindEdgeTestWithNull( )
        {
            Bitmap image = null;
            var analyzer = new Analyzer( );

            var exception = Record.Exception( ( ) => {
                analyzer.FindEdge( image );
            } );

            Assert.NotNull( exception );
            Assert.IsType<ArgumentNullException>( exception );
        }

        [Fact]
        public void FindAveragePointTestWithFivePoints( )
        {
            var testPoints = new List<PointF>
            {
                new PointF( 10, 50 ),
                new PointF( 20, 40 ),
                new PointF( 30, 30 ),
                new PointF( 40, 20 ),
                new PointF( 50, 10 )
            };
            
            var expectedPoint = new PointF( 30, 30 );

            var analyzer = new Analyzer( );
            var result = analyzer.FindAveragePoint( testPoints );

            Assert.Equal( expectedPoint, result );
        }

        [Fact]
        public void FindAveragePointTestWithNull( )
        {
            List<PointF> testPoints = null;
            var analyzer = new Analyzer( );

            var exception = Record.Exception( ( ) => {
                analyzer.FindAveragePoint( testPoints );
            } );

            Assert.NotNull( exception );
            Assert.IsType<ArgumentNullException>( exception );
        }
    }
}
