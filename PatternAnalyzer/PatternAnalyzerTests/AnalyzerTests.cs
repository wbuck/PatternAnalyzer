using Emgu.CV.Structure;
using PatternAnalyzer;
using PatternAnalyzer.Structures;
using PatternAnalyzerTests.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Xunit;

namespace PatternAnalyzerTests
{
    public class AnalyzerTests
    {
        [Fact]
        public void FindBlobsTestTwentyBlobs( )
        {
            var image = ( Bitmap )Resources.ResourceManager.GetObject( "GoodImageTwentyDots" );
            const uint minArea = 2000;
            const uint maxArea = 5000;
            const int expectedBlobsFound = 20;

            var analyzer = new Analyzer( );

            var actual = analyzer.FindBlobs( image, minArea, maxArea );
            Assert.Equal( expectedBlobsFound, actual.Count( ) );
        }

        [Fact]
        public void FindBlobsTestNoBlobs( )
        {
            var image = ( Bitmap )Resources.ResourceManager.GetObject( "BadImageNoDots" );
            const uint minArea = 2000;
            const uint maxArea = 5000;
            const int expectedBlobsFound = 0;

            var analyzer = new Analyzer( );

            var actual = analyzer.FindBlobs( image, minArea, maxArea );
            Assert.Equal( expectedBlobsFound, actual.Count( ) );
        }

        [Fact]
        public void FindBlobsTestNull( )
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
        public void FindEdgeTestEdge( )
        {
            var image = ( Bitmap )Resources.ResourceManager.GetObject( "GoodImageTwentyDots" );
            var notExpected = default( LineSegment2D );
            var analyzer = new Analyzer( );

            var actual = analyzer.FindEdge( image );

            Assert.NotEqual( notExpected, actual );
        }

        [Fact]
        public void FindEdgeTestNoEdge( )
        {
            var image = ( Bitmap )Resources.ResourceManager.GetObject( "BadImageNoEdge" );
            var expected = default( LineSegment2D );
            var analyzer = new Analyzer( );

            var actual = analyzer.FindEdge( image );

            Assert.Equal( expected, actual );
        }

        [Fact]
        public void FindEdgeTestNull( )
        {
            Bitmap image = null;
            var analyzer = new Analyzer( );

            var exception = Record.Exception( ( ) => { analyzer.FindEdge( image ); } );

            Assert.NotNull( exception );
            Assert.IsType<ArgumentNullException>( exception );
        }

        [Fact]
        public void FindAveragePointTestFivePoints( )
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
            var actual = analyzer.FindAveragePoint( testPoints );

            Assert.Equal( expected, actual );
        }

        [Fact]
        public void FindAveragePointTestNoPoints( )
        {
            var testPoints = new List<PointF>( );
            var expected = default( PointF );

            var analyzer = new Analyzer( );
            var actual = analyzer.FindAveragePoint( testPoints );

            Assert.Equal( expected, actual );
        }

        [Fact]
        public void FindAveragePointTestNull( )
        {
            List<PointF> testPoints = null;
            var analyzer = new Analyzer( );

            var exception = Record.Exception( ( ) => { analyzer.FindAveragePoint( testPoints ); } );

            Assert.NotNull( exception );
            Assert.IsType<ArgumentNullException>( exception );
        }

        [Fact]
        public void SortPointsInToRowsAndColumnsTestTentyPoints( )
        {
            var alignmentDotCoordinates = new List<PointF>
            {
                new PointF( 100, 20 ),
                new PointF( 10, 10 ),
                new PointF( 20, 10 ),
                new PointF( 90, 20 ),
                new PointF( 30, 10 ),
                new PointF( 60, 20 ),
                new PointF( 50, 10 ),
                new PointF( 50, 20 ),
                new PointF( 80, 10 ),
                new PointF( 90, 10 ),
                new PointF( 100, 10 ),
                new PointF( 10, 20 ),
                new PointF( 20, 20 ),
                new PointF( 60, 10 ),
                new PointF( 70, 10 ),
                new PointF( 30, 20 ),
                new PointF( 40, 20 ),
                new PointF( 70, 20 ),
                new PointF( 80, 20 ),
                new PointF( 40, 10 ),
            };

            var expectedFirstRowFirstColumnMiddlePoint = new PointF( 30, 10 );
            var expectedFirstRowSecondColumnMiddlePoint = new PointF( 80, 10 );
            var expectedSecondRowFirstColumnFirstPoint = new PointF( 10, 20 );
            var expectedSecondRowSecondColumnLastPoint = new PointF( 100, 20 );
            const int expectedPointsCount = 5;
            const int expectedLength = 4;

            var analyzer = new Analyzer( );
            var actual = analyzer.SortPointsInToRowsAndColumns( alignmentDotCoordinates );

            Assert.Equal( expectedLength, actual.Length );
            Assert.Equal( expectedPointsCount, actual[ 0, 0 ].Points.Count );
            Assert.Equal( expectedPointsCount, actual[ 1, 1 ].Points.Count );
            Assert.Equal( expectedFirstRowFirstColumnMiddlePoint, actual[ 0, 0 ].Points[ 2 ] );
            Assert.Equal( expectedFirstRowSecondColumnMiddlePoint, actual[ 0, 1 ].Points[ 2 ] );
            Assert.Equal( expectedSecondRowFirstColumnFirstPoint, actual[ 1, 0 ].Points[ 0 ] );
            Assert.Equal( expectedSecondRowSecondColumnLastPoint, actual[ 1, 1 ].Points[ 4 ] );
        }

        [Fact]
        public void SortPointsInToRowsAndColumnsTestTenPoints( )
        {
            var alignmentDotCoordinates = new List<PointF>
            {              
                new PointF( 20, 10 ),
                new PointF( 90, 10 ),
                new PointF( 30, 10 ),
                new PointF( 40, 10 ),
                new PointF( 100, 10 ),
                new PointF( 50, 10 ),
                new PointF( 10, 10 ),
                new PointF( 60, 10 ),
                new PointF( 70, 10 ),
                new PointF( 80, 10 ),             
            };

            const int expectedLength = 2;
            var expectedSecondColumnSecondLastPoint = new PointF( 90, 10 );
            var expectedFirstColumnMiddlePoint = new PointF( 30, 10 );
            const int expectedPointsCount = 5;

            var analyzer = new Analyzer( );
            var actual = analyzer.SortPointsInToRowsAndColumns( alignmentDotCoordinates );

            Assert.Equal( expectedLength, actual.Length );
            Assert.Equal( expectedFirstColumnMiddlePoint, actual[ 0, 0 ].Points[ 2 ] );
            Assert.Equal( expectedSecondColumnSecondLastPoint, actual[ 0, 1 ].Points[ 3 ] );
            Assert.Equal( expectedPointsCount, actual[ 0, 0 ].Points.Count );
            Assert.Equal( expectedPointsCount, actual[ 0, 1 ].Points.Count );

        }

        [Fact]
        public void SortPointsInToRowsAndColumnsTestNotEnoughPointsToSort( )
        {
            var alignmentDotCoordinates = new List<PointF>
            {
                new PointF( 100, 20 ),
                new PointF( 10, 10 ),
                new PointF( 20, 10 ),
                new PointF( 90, 20 ),
                new PointF( 30, 10 ),
                new PointF( 60, 20 ),
            };

            var expected = new RowPoints[ 0, 0 ];
            var analyzer = new Analyzer( );

            var result = analyzer.SortPointsInToRowsAndColumns( alignmentDotCoordinates );

            Assert.Equal( expected, result );
        }

        [Fact]
        public void SortPointsInToRowsAndColumnsTestNull( )
        {
            List<PointF> alignmentDotCoordinates = null;
            var analyzer = new Analyzer( );

            var exception = Record.Exception(
                ( ) => { analyzer.SortPointsInToRowsAndColumns( alignmentDotCoordinates ); } );

            Assert.NotNull( exception );
            Assert.IsType<ArgumentNullException>( exception );
        }
    }
}