using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace acquizapi.Models
{
    // Position
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

            if (obj.Row != Row)
                return obj.Row - Row;

            return obj.Column - Column;
        }

        public bool Equals(ChineseChessPosition other)
        {
            return other.Row == Row && other.Column == Column;
        }
    }

    // Piece
    public class ChineseChessPiece: ICloneable
    {
        public string Name { get; set; }
        public ChineseChessPosition Position { get; set; }

        public ChineseChessPiece(String name = null, Int32? row = null, Int32? column = null)
        {
            Position = new ChineseChessPosition();

            if (!String.IsNullOrEmpty(name))
                Name = name;
            if (row != null)
                Position.Row = row.Value;
            if (column != null)
                Position.Column = column.Value;
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

    // Game
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

        // Has piece on rows
        public static Boolean HasPieceOnRows(Int32 col, Int32 minRow, Int32 maxRow, Dictionary<ChineseChessPosition, Boolean> boardState)
        {
            for (var i = minRow; i <= maxRow; i++)
            {
                if (boardState.ContainsKey(new ChineseChessPosition() { Row = i, Column = col }))
                    return true;
            }

            return false;
        }

        // Number of peices on rows
        public static Int32 NumberOfPiecesOnRows(Int32 col, Int32 minRow, Int32 maxRow, Dictionary<ChineseChessPosition, Boolean> boardState)
        {
            var r = 0;
            for (var i = minRow; i <= maxRow; i++)
            {
                if (boardState.ContainsKey(new ChineseChessPosition() { Row = i, Column = col }))
                    ++r;
            }

            return r;
        }

        public static IEnumerable<ChineseChessPosition> FilterBoundedMoves(Int32 curRow, Int32 curCol, 
            List<ChineseChessPosition> possMoves, 
            Dictionary<ChineseChessPosition, Boolean> boardState)
        {
            foreach (var pos in possMoves)
            {
                if ( (pos.Row != curRow) || (pos.Column != curCol)
                    && pos.Row >= ChineseChessPosition.MinimumRow
                    && pos.Row <= ChineseChessPosition.MaximumRow
                    && pos.Column >= ChineseChessPosition.MinimumColumn
                    && pos.Column <= ChineseChessPosition.MaximumColumn
                    && !(boardState.ContainsKey(pos) && boardState[pos]))
                {
                    yield return pos;
                }
            }
        }

        public static ChineseChessPosition FindFirstOpponentOnRow(Int32 curRow, Int32 startCol, 
            Dictionary<ChineseChessPosition, Boolean> boardStates, Boolean increase)
        {
            Int32 nCol = startCol;
            if (increase)
            {
                for (; nCol <= ChineseChessPosition.MaximumColumn; nCol++)
                {
                    ChineseChessPosition pos = new ChineseChessPosition() { Row = curRow, Column = nCol };
                    if (boardStates.ContainsKey(pos))
                    {
                        if (boardStates[pos])
                            return null;
                        else
                            return pos;
                    }
                }
            }
            else
            {
                for (; nCol >= ChineseChessPosition.MinimumColumn; nCol--)
                {
                    ChineseChessPosition pos = new ChineseChessPosition() { Row = curRow, Column = nCol };
                    if (boardStates.ContainsKey(pos))
                    {
                        if (boardStates[pos])
                            return null;
                        else
                            return pos;
                    }
                }
            }

            return null;
        }
        public static ChineseChessPosition FindFirstOpponentOnColumn(Int32 curCol, Int32 startRow, 
            Dictionary<ChineseChessPosition, Boolean> boardStates, Boolean increase)
        {
            Int32 nRow = startRow;
            if (increase)
            {
                for (; nRow <= ChineseChessPosition.MaximumRow; nRow++)
                {
                    ChineseChessPosition pos = new ChineseChessPosition() { Row = nRow, Column = curCol };
                    if (boardStates.ContainsKey(pos))
                    {
                        if (boardStates[pos])
                            return null;
                        else
                            return pos;
                    }
                }
            }
            else
            {
                for (; nRow >= ChineseChessPosition.MinimumRow; nRow--)
                {
                    ChineseChessPosition pos = new ChineseChessPosition() { Row = nRow, Column = curCol};
                    if (boardStates.ContainsKey(pos))
                    {
                        if (boardStates[pos])
                            return null;
                        else
                            return pos;
                    }
                }
            }

            return null;
        }

        public static List<ChineseChessPosition> MovesOnSameLine(Int32 curRow,
            Int32 curCol, Dictionary<ChineseChessPosition, Boolean> boardStates)
        {
            List<ChineseChessPosition> listMoves = new List<ChineseChessPosition>();

            for(Int32 i = curRow + 1; i <= ChineseChessPosition.MaximumRow; i++)
            {
                ChineseChessPosition pos = new ChineseChessPosition() { Row = i, Column = curRow };
                if (boardStates.ContainsKey(pos))
                {
                    if (!boardStates[pos])
                        listMoves.Add(pos);
                    break;
                }
                listMoves.Add(pos);
            }
            for(Int32 j = curRow - 1; j >= ChineseChessPosition.MinimumRow; j --)
            {
                ChineseChessPosition pos = new ChineseChessPosition() { Row = j, Column = curCol };
                if (boardStates.ContainsKey(pos))
                {
                    if (!boardStates[pos])
                        listMoves.Add(pos);
                    break;
                }
                listMoves.Add(pos);
            }
            for (Int32 i = curCol + 1; i <= ChineseChessPosition.MaximumColumn; i ++)
            {
                ChineseChessPosition pos = new ChineseChessPosition() { Row = curRow, Column = i };
                if (boardStates.ContainsKey(pos))
                {
                    if (!boardStates[pos])
                        listMoves.Add(pos);
                }
                listMoves.Add(pos);
            }
            for(Int32 j = curCol - 1; j >= ChineseChessPosition.MinimumColumn; j --)
            {
                ChineseChessPosition pos = new ChineseChessPosition() { Row = curRow, Column = j };
                if (boardStates.ContainsKey(pos))
                {
                    if (!boardStates[pos])
                        listMoves.Add(pos);
                    break;
                }
                listMoves.Add(pos);
            }


            return listMoves;
        }
        public static List<ChineseChessPosition> PossibleMovesForJu(Int32 curRow, Int32 curCol, 
            Dictionary<ChineseChessPosition, Boolean> boardStates)
        {
            return MovesOnSameLine(curRow, curCol, boardStates);
        }
        public static List<ChineseChessPosition> PossibleMovesForMa(Int32 curRow, Int32 curCol, 
            Dictionary<ChineseChessPosition, Boolean> boardStates)
        {
            List<ChineseChessPosition> listMoves = new List<ChineseChessPosition>();
            if (boardStates.ContainsKey(new ChineseChessPosition() {  Row = curRow + 1, Column = curCol}))
            {
                listMoves.Add(new ChineseChessPosition() { Row = curRow + 2, Column = curCol + 1 });
                listMoves.Add(new ChineseChessPosition() { Row = curRow + 2, Column = curCol - 1 });
            }
            if (boardStates.ContainsKey(new ChineseChessPosition() { Row = curRow - 1, Column = curCol }))
            {
                listMoves.Add(new ChineseChessPosition() { Row = curRow - 2, Column = curCol + 1 });
                listMoves.Add(new ChineseChessPosition() { Row = curRow - 2, Column = curCol - 1 });
            }
            if (boardStates.ContainsKey(new ChineseChessPosition() { Row = curRow, Column = curCol + 1 }))
            {
                listMoves.Add(new ChineseChessPosition() { Row = curRow + 1, Column = curCol + 2 });
                listMoves.Add(new ChineseChessPosition() { Row = curRow - 1, Column = curCol + 2 });
            }
            if (boardStates.ContainsKey(new ChineseChessPosition() { Row = curRow, Column = curCol - 1 }))
            {
                listMoves.Add(new ChineseChessPosition() { Row = curRow + 1, Column = curCol - 2 });
                listMoves.Add(new ChineseChessPosition() { Row = curRow - 1, Column = curCol - 2 });
            }

            return listMoves;
        }
        public static List<ChineseChessPosition> PossibleMovesForPao(Int32 curRow, Int32 curCol,
            Dictionary<ChineseChessPosition, Boolean> boardStates)
        {
            List<ChineseChessPosition> listMoves = new List<ChineseChessPosition>();
            for(Int32 i = curRow + 1; i <= ChineseChessPosition.MaximumRow; i++)
            {
                ChineseChessPosition pos = new ChineseChessPosition() { Row = i, Column = curCol };
                if (boardStates.ContainsKey(pos))
                {
                    ChineseChessPosition next = FindFirstOpponentOnColumn(curCol, i + 1, boardStates, true);
                    if (next != null)
                        listMoves.Add(next);
                    break;
                }
                listMoves.Add(pos);
            }
            for(Int32 j = curRow - 1; j >= ChineseChessPosition.MinimumRow; j --)
            {
                ChineseChessPosition pos = new ChineseChessPosition() { Row = j, Column = curCol };
                if (boardStates.ContainsKey(pos))
                {
                    ChineseChessPosition next = FindFirstOpponentOnColumn(curCol, j - 1, boardStates, false);
                    if (next != null)
                        listMoves.Add(next);
                    break;
                }
                listMoves.Add(pos);
            }
            for(Int32 i = curCol + 1; i <= ChineseChessPosition.MaximumColumn; i ++)
            {
                ChineseChessPosition pos = new ChineseChessPosition() { Row = curRow, Column = i};
                if (boardStates.ContainsKey(pos))
                {
                    ChineseChessPosition next = FindFirstOpponentOnColumn(curRow, i + 1, boardStates, true);
                    if (next != null)
                        listMoves.Add(next);
                    break;
                }
                listMoves.Add(pos);
            }
            for (Int32 j = curCol - 1; j >= ChineseChessPosition.MinimumColumn; j --)
            {
                ChineseChessPosition pos = new ChineseChessPosition() { Row = curRow, Column = j };
                if (boardStates.ContainsKey(pos))
                {
                    ChineseChessPosition next = FindFirstOpponentOnColumn(curRow, j - 1, boardStates, false);
                    if (next != null)
                        listMoves.Add(next);
                    break;
                }
                listMoves.Add(pos);
            }

            return listMoves;
        }
        public static List<ChineseChessPosition> PossibleMovesForShi(Int32 curRow, Int32 curCol, 
            Dictionary<ChineseChessPosition, Boolean> boardStates, Boolean isLowerTeam)
        {
            List<ChineseChessPosition> listMoves = new List<ChineseChessPosition>();
            if (curRow == 2 || curRow == 9)
            {
                listMoves.Add(new ChineseChessPosition() { Row = curRow - 1, Column = curCol + 1 });
                listMoves.Add(new ChineseChessPosition() { Row = curRow - 1, Column = curCol - 1 });
                listMoves.Add(new ChineseChessPosition() { Row = curRow + 1, Column = curCol + 1 });
                listMoves.Add(new ChineseChessPosition() { Row = curRow + 1, Column = curCol - 1 });
            }
            else
            {
                listMoves.Add(isLowerTeam ? new ChineseChessPosition() { Row = 2, Column = 5 } 
                    : new ChineseChessPosition() { Row = 9, Column = 5 });
            }

            return listMoves;
        }
        public static IEnumerable<ChineseChessPosition> PossibleMovesForKing(Int32 curRow, Int32 curCol,
            Dictionary<ChineseChessPosition, Boolean> boardStates)
        {
            List<ChineseChessPosition> listMoves = new List<ChineseChessPosition>();
            for (Int32 col = 4; col <= 6; col ++)
            {
                listMoves.Add(new ChineseChessPosition() { Row = curRow, Column = col });
            }
            if (curRow < 5)
            {
                for(Int32 row = 1; row <= 3; row ++)
                {
                    listMoves.Add(new ChineseChessPosition() { Row = row, Column = curCol });
                }
            }
            else
            {
                for (Int32 row = 8; row <= 10; row++)
                {
                    listMoves.Add(new ChineseChessPosition() { Row = row, Column = curCol });
                }
            }

            foreach(var mov in listMoves)
            {
                if ((mov.Row - curRow) * (mov.Row - curRow) + (mov.Column - curRow) * (mov.Column - curCol) < 2)
                    yield return mov;
            }
        }
        public static List<ChineseChessPosition> PossibleMovesForXiang(Int32 curRow, Int32 curCol,
            Dictionary<ChineseChessPosition, Boolean> boardStates, Boolean isLowerTeam)
        {
            List<ChineseChessPosition> listMoves = new List<ChineseChessPosition>();
            var canMoveDowward = (isLowerTeam || curRow >= 8);
            var canMoveUpward = (curRow <= 3 || !isLowerTeam);
            if (canMoveUpward && !(boardStates.ContainsKey(new ChineseChessPosition() { Row = curRow + 1, Column = curCol + 1 }))) 
                listMoves.Add(new ChineseChessPosition() { Row = curRow + 2, Column = curCol + 2 });
            if (canMoveUpward && !(boardStates.ContainsKey(new ChineseChessPosition() { Row = curRow + 1, Column = curCol - 1 })))
                listMoves.Add(new ChineseChessPosition() { Row = curRow + 2, Column = curCol - 2 });
            if (canMoveDowward && !(boardStates.ContainsKey(new ChineseChessPosition() { Row = curRow - 1, Column = curCol + 1 })))
                listMoves.Add(new ChineseChessPosition() { Row = curRow - 2, Column = curCol + 2 });
            if (canMoveDowward && !(boardStates.ContainsKey(new ChineseChessPosition() { Row = curRow - 1, Column = curCol - 1 })))
                listMoves.Add(new ChineseChessPosition() { Row = curRow - 2, Column = curCol - 2 });

            return listMoves;
        }
        public static List<ChineseChessPosition> PossibleMovesForZu(Int32 curRow, Int32 curCol,
            Dictionary<ChineseChessPosition, Boolean> boardStates, Boolean isLowerTeam)
        {
            List<ChineseChessPosition> listMoves = new List<ChineseChessPosition>();
            var beyond = isLowerTeam ? (curRow > 5) : (curRow <= 5); //beyond the river
            if (isLowerTeam)
            {
                listMoves.Add(new ChineseChessPosition() { Row = curRow + 1, Column = curCol});
            }
            else
            {
                listMoves.Add(new ChineseChessPosition() { Row = curRow - 1, Column = curCol });
            }
            if (beyond) {
                listMoves.Add(new ChineseChessPosition() { Row = curRow, Column = curCol - 1 });
                listMoves.Add(new ChineseChessPosition() { Row = curRow, Column = curCol + 1 });
            }

            return listMoves;
        }
        public static IEnumerable<ChineseChessPosition> PossibleMovesByPiece(ChineseChessPiece piece, 
            Dictionary<ChineseChessPosition, Boolean> boardStates,
            Boolean isLowerTeam)
        {
            var name = piece.Name;
            List<ChineseChessPosition> listMoves = new List<ChineseChessPosition>();
            var curRow = piece.Position.Row;
            var curCol = piece.Position.Column;
            switch(name)
            {
                case "j":
                    listMoves = PossibleMovesForJu(curRow, curCol, boardStates);
                    break;

                case "m":
                    listMoves = PossibleMovesForMa(curRow, curCol, boardStates);
                    break;

                case "x":
                    listMoves = PossibleMovesForXiang(curRow, curCol, boardStates, isLowerTeam);
                    break;

                case "s":
                    listMoves = PossibleMovesForShi(curRow, curCol, boardStates, isLowerTeam);
                    break;

                case "k":
                    listMoves = PossibleMovesForKing(curRow, curCol, boardStates).ToList<ChineseChessPosition>();
                    break;

                case "p":
                    listMoves = PossibleMovesForPao(curRow, curCol, boardStates);
                    break;

                case "z":
                    listMoves = PossibleMovesForZu(curRow, curCol, boardStates, isLowerTeam);
                    break;

                default:
                    throw new Exception("Unsupported name");
            }

            return FilterBoundedMoves(curRow, curCol, listMoves, boardStates);
        }
        public static Dictionary<String, IEnumerable<ChineseChessPosition>> AllPossibleMoves(List<ChineseChessPiece> listMyPieces, 
            Dictionary<ChineseChessPosition, bool> boardStates, Int32 team)
        {
            var isLowerTeam = (team == 1);
            Dictionary<String, IEnumerable<ChineseChessPosition>> dictMoves = new Dictionary<string, IEnumerable<ChineseChessPosition>>();
            foreach (var piece in listMyPieces)
            {
                var moves4Piece = PossibleMovesByPiece(piece, boardStates, isLowerTeam);
                dictMoves.Add(piece.Name, moves4Piece);
            }

            return dictMoves;
        }

        // Get game end status
        // Null: Not end!
        public static ChineseChessStatusEnum? GetGameEndStatus(ChineseChessAgent agent)
        {
            var myPieces = agent.MyPieces;
            var oppoPieces = agent.OppoPieces;
            var boardState = agent.BoardState;
            return GetGameEndStatusByState(myPieces, oppoPieces, boardState, agent.Team);
        }

        private static ChineseChessStatusEnum? GetGameEndStatusByState(List<ChineseChessPiece> myPieces, 
            List<ChineseChessPiece> oppoPieces, 
            Dictionary<ChineseChessPosition, Boolean> boardState, 
            short team)
        {
            var myKing = myPieces.Find(x => x.Name == "k");
            var oppoKing = oppoPieces.Find(x => x.Name == "K");
            if (myKing == null) return ChineseChessStatusEnum.Lose;
            if (oppoKing == null) return ChineseChessStatusEnum.Win;
            if (myKing.Position.Column != oppoKing.Position.Column) return null;

            Int32 minRow = -1, maxRow = -1;
            if (team == 1)
            {
                minRow = myKing.Position.Row + 1;
                maxRow = oppoKing.Position.Row - 1;
            }
            else
            {
                minRow = oppoKing.Position.Row + 1;
                maxRow = myKing.Position.Row - 1;
            }

            if (HasPieceOnRows(myKing.Position.Column, minRow, maxRow, boardState))
                return null;

            return ChineseChessStatusEnum.Win;
        }
    }

    public class ChineseChessAgent
    {
        public Int16 Team { get; set; }
        public Dictionary<String, IEnumerable<ChineseChessPosition>> LegalMoves { get; set; }
        public List<ChineseChessPiece> MyPieces { get; set; }
        public List<ChineseChessPiece> OppoPieces { get; set; }
        public ChineseChessAgent OppoAgent { get; set; }
        public Dictionary<String, ChineseChessPosition> MyPiecesDic { get; set; }
        public Dictionary<ChineseChessPosition, Boolean> BoardState { get; set; }

        public ChineseChessAgent(Int16 team, List<ChineseChessPiece> myPieces)
        {
            Team = team;
            if (myPieces != null)
            {
                MyPieces = myPieces;
            }
            else
            {
                MyPieces = (team == 1) ? ChineseChessGame.GetInitRedPieces() : ChineseChessGame.GetInitBlackPieces();
            }

            BoardState = new Dictionary<ChineseChessPosition, bool>();
            MyPiecesDic = new Dictionary<string, ChineseChessPosition>();
        }

        public void SetOppoAgent(ChineseChessAgent oppoAgent)
        {
            OppoAgent = oppoAgent;
            OppoPieces = oppoAgent.MyPieces;
        }

        public void UpdateState()
        {
            UpdateBoardState();
            UpdatePieceDict();
            ComputeLegalMoves();
        }

        public void UpdateBoardState()
        {
            BoardState.Clear();

            foreach (var piece in MyPieces)
            {
                BoardState.Add(piece.Position, true);
            }
            foreach (var piece in OppoPieces)
            {
                BoardState.Add(piece.Position, false);
            }
        }
        public void UpdatePieceDict()
        {
            foreach(var piece in MyPieces)
            {
                MyPiecesDic.Add(piece.Name, piece.Position);
            }
        }
        public void ComputeLegalMoves()
        {
            LegalMoves = ChineseChessGame.AllPossibleMoves(MyPieces, BoardState, Team);
        }

        public void MovePieceTo(ChineseChessPiece piece, ChineseChessPosition pos, Boolean? isCapture)
        {
            piece.MoveToPosition(pos);

            Boolean bCap = false;
            if (isCapture == null)
            {
                foreach(var piece2 in OppoPieces)
                {
                    if (piece2.Position == pos)
                    {
                        bCap = true;
                        break;
                    }
                }
            }

            if (bCap)
                CaptureOppoPiece(pos);
        }

        private void CaptureOppoPiece(ChineseChessPosition pos)
        {
            Int32 idx = -1;
            for(var i = 0; i < OppoPieces.Count; i ++)
            {
                if (OppoPieces[i].Position == pos)
                {
                    idx = i;
                    break;
                }
            }
            if (idx != -1)
            {
                OppoPieces.RemoveAt(idx);
            }
        }
    }

    public enum ChineseChessStatusEnum
    {
        Win = 1,
        Lose = -1,
        Draw = 0
    }

    public class ChineseChessState
    {
        public ChineseChessAgent RedAgent { get; set; }
        public ChineseChessAgent BlackAgent { get; set; }
        public Int16 PlayingTeam { get; set; }
        public Boolean EndFlag { get; set; }

        public void SwitchTurn()
        {
            PlayingTeam *= -1;
        }

        public ChineseChessStatusEnum? GetEndStatus()
        {
            var playing = PlayingTeam == 1 ? RedAgent : BlackAgent;
            return ChineseChessGame.GetGameEndStatus(playing);
        }
    }

    // Interface between webapp and webapi
    public class ChineseChessAIMove
    {
        public string Name { get; set; }
        public ChineseChessPosition Position { get; set; }
    }
    public class ChineseChessAIInputAgent
    {
        public Int16 Strategy { get; set; }
        public Int16 Depth { get; set; }
        public List<ChineseChessAIMove> PastMoves { get; private set; }
        public Int16 Team { get; set; }
        public List<ChineseChessPiece> MyPieces { get; private set; }
        public List<Int32> Weights { get; set; }
        public List<Int32> FeatureMatrix { get; set; }

        public ChineseChessAIInputAgent()
        {
            PastMoves = new List<ChineseChessAIMove>();
            MyPieces = new List<ChineseChessPiece>();
        }
    }

    public class ChineseChessAIInput
    {
        public Boolean? EndFlag { get; set; }
        public ChineseChessAIInputAgent BlackAgent { get; set; }
        public ChineseChessAIInputAgent RedAgent { get; set; }
        public Int32 PlayingTeam { get; set; }

        public ChineseChessAIInput()
        {
            this.BlackAgent = new ChineseChessAIInputAgent();
            this.RedAgent = new ChineseChessAIInputAgent();
        }
    }

    public class ChineseChessAIOutput
    {
        public ChineseChessPiece CurrentPiece { get; set; }
        public ChineseChessPosition NewPosition { get; set; }
        public Int32 TickCount { get; set; }
        public List<Int32> StateFeature { get; set; }
    }
}
