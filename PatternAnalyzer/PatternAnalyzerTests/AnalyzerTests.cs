using Emgu.CV.Structure;
using PatternAnalyzer;
using PatternAnalyzer.Structures;
using PatternAnalyzerTests.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using NUnit.Framework;

namespace PatternAnalyzerTests
{
    public class AnalyzerTests
    {
        [Test]
        public void FindBlobsTestTwentyBlobs( )
        {
            var image = ( Bitmap )Resources.ResourceManager.GetObject( "GoodImageTwentyDots" );
            const uint minArea = 2000;
            const uint maxArea = 5000;
            const int expectedBlobsFound = 20;

            var analyzer = new Analyzer( );

            var actual = analyzer.FindBlobs( image, minArea, maxArea );
            Assert.AreEqual( expectedBlobsFound, actual.Count( ) );
        }

        [Test]
        public void FindBlobsTestNoBlobs( )
        {
            var image = ( Bitmap )Resources.ResourceManager.GetObject( "BadImageNoDots" );
            const uint minArea = 2000;
            const uint maxArea = 5000;
            const int expectedBlobsFound = 0;

            var analyzer = new Analyzer( );

            var actual = analyzer.FindBlobs( image, minArea, maxArea );
            Assert.AreEqual( expectedBlobsFound, actual.Count( ) );
        }

        [Test]
        public void FindBlobsTestNull( )
        {
            Bitmap image = null;
            const uint minArea = 2000;
            const uint maxArea = 5000;

            var analyzer = new Analyzer( );

            Assert.Throws<ArgumentNullException>( ( ) => { analyzer.FindBlobs( image, minArea, maxArea ); } );
        }

        [Test]
        public void FindEdgeTestEdge( )
        {
            var image = ( Bitmap )Resources.ResourceManager.GetObject( "GoodImageTwentyDots" );
            var notExpected = default( LineSegment2D );
            var analyzer = new Analyzer( );

            var actual = analyzer.FindEdge( image );

            Assert.AreNotEqual( notExpected, actual );
        }

        [Test]
        public void FindEdgeTestNoEdge( )
        {
            var image = ( Bitmap )Resources.ResourceManager.GetObject( "BadImageNoEdge" );
            var expected = default( LineSegment2D );
            var analyzer = new Analyzer( );

            var actual = analyzer.FindEdge( image );

            Assert.AreEqual( expected, actual );
        }

        [Test]
        public void FindEdgeTestNull( )
        {
            Bitmap image = null;
            var analyzer = new Analyzer( );

            Assert.Throws<ArgumentNullException>( ( ) => { analyzer.FindEdge( image ); } );
        }

        [Test]
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

            Assert.AreEqual( expected, actual );
        }

        [Test]
        public void FindAveragePointTestNoPoints( )
        {
            var testPoints = new List<PointF>( );
            var expected = default( PointF );

            var analyzer = new Analyzer( );
            var actual = analyzer.FindAveragePoint( testPoints );

            Assert.AreEqual( expected, actual );
        }

        [Test]
        public void FindAveragePointTestNull( )
        {
            List<PointF> testPoints = null;
            var analyzer = new Analyzer( );

            Assert.Throws<ArgumentNullException>( ( ) => { analyzer.FindAveragePoint( testPoints ); } );
        }

        [Test]
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

            Assert.AreEqual( expectedLength, actual.Length );
            Assert.AreEqual( expectedPointsCount, actual[ 0, 0 ].Points.Count );
            Assert.AreEqual( expectedPointsCount, actual[ 1, 1 ].Points.Count );
            Assert.AreEqual( expectedFirstRowFirstColumnMiddlePoint, actual[ 0, 0 ].Points[ 2 ] );
            Assert.AreEqual( expectedFirstRowSecondColumnMiddlePoint, actual[ 0, 1 ].Points[ 2 ] );
            Assert.AreEqual( expectedSecondRowFirstColumnFirstPoint, actual[ 1, 0 ].Points[ 0 ] );
            Assert.AreEqual( expectedSecondRowSecondColumnLastPoint, actual[ 1, 1 ].Points[ 4 ] );
        }

        [Test]
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

            Assert.AreEqual( expectedLength, actual.Length );
            Assert.AreEqual( expectedFirstColumnMiddlePoint, actual[ 0, 0 ].Points[ 2 ] );
            Assert.AreEqual( expectedSecondColumnSecondLastPoint, actual[ 0, 1 ].Points[ 3 ] );
            Assert.AreEqual( expectedPointsCount, actual[ 0, 0 ].Points.Count );
            Assert.AreEqual( expectedPointsCount, actual[ 0, 1 ].Points.Count );

        }

        [Test]
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

            Assert.AreEqual( expected, result );
        }

        [Test]
        public void SortPointsInToRowsAndColumnsTestNull( )
        {
            List<PointF> alignmentDotCoordinates = null;
            var analyzer = new Analyzer( );

            Assert.Throws<ArgumentNullException>(
                ( ) => { analyzer.SortPointsInToRowsAndColumns( alignmentDotCoordinates ); } );
        }

        [Test]
        public void CalculateDeltaXTestCorrectDistance( )
        {
            var point1 = new PointF( 10, 0 );
            var point2 = new PointF( 50, 0 );

            const double expected = 40;
            var analyzer = new Analyzer( );

            var actual = analyzer.CalculateDeltaX( point1, point2 );
            Assert.AreEqual( expected, actual );
        }
    }
}