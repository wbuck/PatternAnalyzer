using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
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
            Assert.Equal( expectedBlobsFound, result.Count( ) );
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
            Assert.Equal( expectedBlobsFound, result.Count( ) );
        }

        [Fact]
        public void FindBlobsTestWithNull( )
        {
            Bitmap image = null;
            const uint minArea = 2000;
            const uint maxArea = 5000;

            var analyzer = new Analyzer( );

            var exception = Record.Exception( ( ) => { analyzer.FindBlobs( image, minArea, maxArea ); } );

            Assert.NotNull( exception );
            Assert.IsType<ArgumentNullException>( exception );
        }

        [Fact]
        public void FindEdgeTestWithEdge( )
        {
            var image = ( Bitmap )Resources.ResourceManager.GetObject( "GoodImageTwentyDots" );
            var notExpected = default( LineSegment2D );
            var analyzer = new Analyzer( );

            var result = analyzer.FindEdge( image );

            Assert.NotEqual( notExpected, result );
        }

        [Fact]
        public void FindEdgeTestWithNoEdge( )
        {
            var image = ( Bitmap )Resources.ResourceManager.GetObject( "BadImageNoEdge" );
            var expected = default( LineSegment2D );
            var analyzer = new Analyzer( );

            var result = analyzer.FindEdge( image );

            Assert.Equal( expected, result );
        }

        [Fact]
        public void FindEdgeTestWithNull( )
        {
            Bitmap image = null;
            var analyzer = new Analyzer( );

            var exception = Record.Exception( ( ) => { analyzer.FindEdge( image ); } );

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

            var expected = new PointF( 30, 30 );

            var analyzer = new Analyzer( );
            var result = analyzer.FindAveragePoint( testPoints );

            Assert.Equal( expected, result );
        }

        [Fact]
        public void FindAveragePointTestWithNoPoints( )
        {
            var testPoints = new List<PointF>( );
            var expected = default( PointF );

            var analyzer = new Analyzer( );
            var result = analyzer.FindAveragePoint( testPoints );

            Assert.Equal( expected, result );
        }

        [Fact]
        public void FindAveragePointTestWithNull( )
        {
            List<PointF> testPoints = null;
            var analyzer = new Analyzer( );

            var exception = Record.Exception( ( ) => { analyzer.FindAveragePoint( testPoints ); } );

            Assert.NotNull( exception );
            Assert.IsType<ArgumentNullException>( exception );
        }

        [Fact]
        public void SplitPointsTest( )
        {
            var firstRow = new List<PointF>
            {
                new PointF( 10, 10 ),
                new PointF( 20, 10 ),
                new PointF( 30, 10 ),
                new PointF( 40, 10 ),
                new PointF( 50, 10 ),

                new PointF( 60, 10 ),
                new PointF( 70, 10 ),
                new PointF( 80, 10 ),
                new PointF( 90, 10 ),
                new PointF( 100, 10 )
            };

            var secondRow = new List<PointF>
            {
                new PointF( 10, 20 ),
                new PointF( 20, 20 ),
                new PointF( 30, 20 ),
                new PointF( 40, 20 ),
                new PointF( 50, 20 ),

                new PointF( 60, 20 ),
                new PointF( 70, 20 ),
                new PointF( 80, 20 ),
                new PointF( 90, 20 ),
                new PointF( 100, 20 )
            };

            var merged = new List<PointF>( );
            merged.AddRange( firstRow );
            merged.AddRange( secondRow );

            const int expectedFirstRowFirstColumnLastX = 50;
            const int expectedFirstRowSecondColumnMiddleX = 80;
            const int expectedFirstRowY = 10;

            const int expectedSecondRowFirstColumnMiddleX = 30;
            const int expectedSecondRowSecondColumnSecondX = 70;
            const int expectedSecondRowY = 20;

            var analyzer = new Analyzer();
            var result = analyzer.SplitPoints( merged );

            Assert.Equal( expectedFirstRowFirstColumnLastX, result[ 0, 0 ].Points[ 4 ].X );
            Assert.Equal( expectedFirstRowSecondColumnMiddleX, result[ 0, 1 ].Points[ 2 ].X );
            Assert.Equal( expectedFirstRowY, result[ 0, 0 ].Points[ 4 ].Y );

            Assert.Equal( expectedSecondRowFirstColumnMiddleX, result[ 1, 0 ].Points[ 2 ].X );
            Assert.Equal( expectedSecondRowSecondColumnSecondX, result[ 1, 1 ].Points[ 1 ].X );
            Assert.Equal( expectedSecondRowY, result[ 1, 0 ].Points[ 4 ].Y );

        }
    }
}