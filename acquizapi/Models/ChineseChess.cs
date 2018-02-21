using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace acquizapi.Models
{
    public sealed class ChineseChessPosition: ICloneable, IComparable<ChineseChessPosition>, IEquatable<ChineseChessPosition>
    {
        public Int32 Row { get; set; }
        public Int32 Column { get; set; }
        public const Int32 MinimumRow = 1;
        public const Int32 MaximumRow = 10;
        public const Int32 MinimumColumn = 1;
        public const Int32 MaximumColumn = 9;

        public ChineseChessPosition()
        {
            Row = -1;
            Column = -1;
        }

        public Boolean IsValid()
        {
            if (Row > MaximumRow || Row < MinimumRow || Column > MaximumColumn || Column < MinimumColumn )
                return false;

            return true;
        }

        public object Clone()
        {
            return new ChineseChessPosition()
            {
                Row = Row,
                Column = Column
            };
        }

        public int CompareTo(ChineseChessPosition obj)
        {
            if (obj.Row == Row && obj.Column == Column)
                return 0;

            if (obj.Row != this.Row)
                return obj.Row - Row;

            return obj.Column - this.Column;
        }

        public bool Equals(ChineseChessPosition other)
        {
            return other.Row == this.Row && other.Column == this.Column;
        }
    }

    public class ChineseChessPiece: ICloneable
    {
        public string Name { get; set; }
        public ChineseChessPosition Position { get; set; }

        public ChineseChessPiece(String name = null, Int32? row = null, Int32? column = null)
        {
            Position = new ChineseChessPosition();

            if (!String.IsNullOrEmpty(name))
                this.Name = name;
            if (row != null)
                this.Position.Row = row.Value;
            if (column != null)
                this.Position.Column = column.Value;
        }

        public void MoveToPosition(ChineseChessPosition pos)
        {
            Position.Row = pos.Row;
            Position.Column = pos.Column;
        }

        public object Clone()
        {
            ChineseChessPiece piece = new ChineseChessPiece()
            {
                Name = Name,
                Position = (ChineseChessPosition)Position.Clone()
            };

            return piece;
        }
    }

    public class ChineseChessGame
    {
        public static List<ChineseChessPiece> GetInitRedPieces()
        {
            List<ChineseChessPiece> listPieces = new List<ChineseChessPiece>();
            listPieces.Add(new ChineseChessPiece("j1", 1, 1));
            listPieces.Add(new ChineseChessPiece("j2", 1, 9));
            listPieces.Add(new ChineseChessPiece("p1", 3, 2));
            listPieces.Add(new ChineseChessPiece("p2", 3, 8));
            listPieces.Add(new ChineseChessPiece("m1", 1, 2));
            listPieces.Add(new ChineseChessPiece("m2", 1, 8));
            listPieces.Add(new ChineseChessPiece("x1", 1, 3));
            listPieces.Add(new ChineseChessPiece("x2", 1, 7));
            listPieces.Add(new ChineseChessPiece("s1", 1, 4));
            listPieces.Add(new ChineseChessPiece("s2", 1, 6));
            listPieces.Add(new ChineseChessPiece("z1", 4, 1));
            listPieces.Add(new ChineseChessPiece("z2", 4, 3));
            listPieces.Add(new ChineseChessPiece("z3", 4, 5));
            listPieces.Add(new ChineseChessPiece("z4", 4, 7));
            listPieces.Add(new ChineseChessPiece("z5", 4, 9));
            listPieces.Add(new ChineseChessPiece("k", 1, 5));

            return listPieces;
        }

        public static List<ChineseChessPiece> GetInitBlackPieces()
        {
            List<ChineseChessPiece> listPieces = new List<ChineseChessPiece>();
            listPieces.Add(new ChineseChessPiece("j1", 10, 1));
            listPieces.Add(new ChineseChessPiece("j2", 10, 9));
            listPieces.Add(new ChineseChessPiece("p1", 8, 2));
            listPieces.Add(new ChineseChessPiece("p2", 8, 8));
            listPieces.Add(new ChineseChessPiece("m1", 10, 2));
            listPieces.Add(new ChineseChessPiece("m2", 10, 8));
            listPieces.Add(new ChineseChessPiece("x1", 10, 3));
            listPieces.Add(new ChineseChessPiece("x2", 10, 7));
            listPieces.Add(new ChineseChessPiece("s1", 10, 4));
            listPieces.Add(new ChineseChessPiece("s2", 10, 6));
            listPieces.Add(new ChineseChessPiece("z1", 7, 1));
            listPieces.Add(new ChineseChessPiece("z2", 7, 3));
            listPieces.Add(new ChineseChessPiece("z3", 7, 5));
            listPieces.Add(new ChineseChessPiece("z4", 7, 7));
            listPieces.Add(new ChineseChessPiece("z5", 7, 9));
            listPieces.Add(new ChineseChessPiece("k", 10, 5));

            return listPieces;
        }
    }


    public class ChineseChessAgent
    {

    }

    // Interface between webapp and webapi
    public class ChineseChessAIMove
    {
        public string Name { get; set; }
        public ChineseChessPosition Position { get; set; }
    }
    public class ChineseChessAIInputAgent
    {
        public Int16 AIStrategy { get; set; }
        public Int16 Depth { get; set; }
        public List<ChineseChessAIMove> PastMovements { get; private set; }
        public Int16 Team { get; set; }
        public List <ChineseChessPiece> MyPieces { get; private set; }
        public List<Int32> Weights { get; set; }
        public List<Int32> FeatureMatrix { get; set; }

        public ChineseChessAIInputAgent()
        {
            this.PastMovements = new List<ChineseChessAIMove>();
            this.MyPieces = new List<ChineseChessPiece>();
        }
    }

    public class ChineseChessAIInput
    {
        public Boolean EndFlag { get; set; }
        public ChineseChessAIInputAgent BlackAgent { get; set; }
        public ChineseChessAIInputAgent RedAgent { get; set; }
        public Int32 PlayingTeam { get; set; }
    }

    public class ChineseChessAIOutput
    {
        public ChineseChessPiece CurrentPiece { get; set; }
        public ChineseChessPosition NewPosition { get; set; }
        public Int32 TickCount { get; set; }
        public List<Int32> StateFeature { get; set; }
    }
}
