using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace PatternAnalyzer.Structures
{
    public class RowPoints
    {
        public bool IsEmpty => !Points.Any( );

        public int RowIndex { get; }

        public int ColumnIndex { get; }
        public List<PointF> Points { get; } = new List<PointF>( );

        public RowPoints( int rowIndex, int columnIndex, IEnumerable<PointF> points  )
        {          
            RowIndex = rowIndex;
            ColumnIndex = columnIndex;

            if( points != null )
            {
                Points.AddRange( points );
            }
        }
    }
}
