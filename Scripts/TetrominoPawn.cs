namespace Blockfall.Scripts;

using Godot;
using Blockfall.Scripts.Models;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// The tetrominos that include <see cref="Piece"/>s displayed on board.
/// </summary>
public partial class TetrominoPawn : Tetromino
{
    /// <summary>
    /// Handler of signal that is emitted when <see cref="LockTetromino"/> is processed and tetromino is locked.
    /// </summary>
    /// <param name="tetromino">The locked tetromino.</param>
    [Signal]
    public delegate void LockTetrominoEventHandler(TetrominoPawn tetromino);

    /// <summary>
    /// Scene that is used to instantiate new <see cref="GhostTetromino"/>s.
    /// </summary>
    public PackedScene GhostTetrominoScene;

    /// <summary>
    /// Store all other pieces under active pawn to avoid having to poll when situation changes.
    /// Data is fetched from parent each time we activate new Pawn.
    /// </summary>
    public List<Piece> OtherPieces { get; set; } = new();

    //TODO: check if we can get rid of this
    protected TetrominoGhost GhostTetromino;

    /// <summary>
    /// The current rotation index of the Tetromino.
    /// </summary>
    protected int RotationIndex = 0;
    /// <summary>
    /// Wallkicks for this Tetromino.
    /// </summary>
    protected Vector2[][] WallKicks;

    //TODO: Remove when proper deferred call
    private Callable CallUpdateGhostPosition;
    //TODO: Remove when proper deferred call
    private Callable CallAddGhostTetrominoToRoot;

    /// <summary>
    /// <see cref="Vector2"/> for location where drag gesture was started.
    /// </summary>
    private Vector2? _dragStart = null;
    /// <summary>
    /// State for if dragging gesture is ongoing.
    /// </summary>
    public bool Dragging => _dragStart != null;

    /// <summary>
    /// Threshold for when drag gesture should turn into a <see cref="Move(Vector2)"/>.
    /// </summary>
    private static readonly int movementThreshold = Board.PIECE_SIZE / 2;

    /// <summary>
    /// Handle for <see cref="Board"/> that the pawn belongs to.
    /// </summary>
    private Board _board;

    /// <summary>
    /// Called when the node enters the scene tree for the first time.
    /// </summary>
    public override void _Ready()
    {
        // First do all of the common initialization.
        base._Ready();

        _board = GetParent<Board>();

        // Init common stuff
        _board.Difficulty.OnReady(OnMoveTimerTimeout);

        GhostTetrominoScene = ResourceLoader.Load<PackedScene>(Resources.GhostTetromino);
        //TODO: Replace with proper deferred call
        CallAddGhostTetrominoToRoot = Callable.From(() => GetTree().Root.AddChild(GhostTetromino));
        CallUpdateGhostPosition = Callable.From(() => UpdateGhostPosition());

        // Init Tetromino
        Position = PieceData.SpawnOffsetLeft ? Board.SPAWN_POINT + Vector2.Left * Board.PIECE_SIZE : Board.SPAWN_POINT;
        WallKicks = PieceData.TetrominoType == TetrominoType.I ? Autoload.WallKicksI : Autoload.WallKicksOthers;
        OtherPieces = _board.GetAllPiecesInLines();

        // Create ghost for the new active piece
        GhostTetromino = Tetromino.Create<TetrominoGhost>(GhostTetrominoScene, PieceData);
        CallAddGhostTetrominoToRoot.CallDeferred();
        CallUpdateGhostPosition.CallDeferred(); // TODO: Fix deferred call
    }

    /// <summary>
    /// Handle input.
    /// </summary>
    /// <param name="event">Event with input details.</param>
    public override void _Input(InputEvent @event)
    {
        //ProcessMouseInput(@event);
        ProcessTouchInput(@event);

        if (Input.IsActionJustPressed(Autoload.PlayerInputs.Right))
        {
            Move(Vector2.Right);
        }
        else if (Input.IsActionJustPressed(Autoload.PlayerInputs.Left))
        {
            Move(Vector2.Left);
        }
        else if (Input.IsActionJustPressed(Autoload.PlayerInputs.Down))
        {
            Move(Vector2.Down);
            _board.Difficulty.MoveTimer.Start();
        }
        else if (Input.IsActionJustPressed(Autoload.PlayerInputs.Drop))
        {
            Drop();
        }
        else if (Input.IsActionJustPressed(Autoload.PlayerInputs.Rotate))
        {
            Rotate();
        }
    }

    /// <summary>
    /// Check all pieces of tetromino, if they are within game bounds after <paramref name="direction"/>.
    /// </summary>
    /// <param name="direction">Direction Vector of movement.</param>
    /// <returns><see cref="true"/> if new position is within bounds, <see cref="false"/> otherwise.</returns>
    public bool IsWithinGameBounds(Vector2 direction)
    {
        return !Pieces.Any(piece => _board.IsPositionOutOfBoard(piece.Position + direction));
    }

    /// <summary>
    /// Check all pieces of tetromino, if they are colliding with other Tetrominos after <paramref name="direction"/>.
    /// </summary>
    /// <param name="direction">Direction Vector of movement.</param>
    /// <returns><see cref="true"/> if new position collides, <see cref="false"/> otherwise.</returns>
    public bool IsCollidingWithOtherTetrominos(Vector2 direction)
    {
        foreach (var otherPiece in OtherPieces)
        {
            if (Pieces.Any(piece => piece.Position + direction == otherPiece.GlobalPosition))
                return true;
        }

        return false;
    }

    /// <summary>
    /// Move the tetromino on <see cref="Board"/>.
    /// </summary>
    /// <param name="direction">Direction Vector of movement.</param>
    /// <returns><see cref="true"/> if tetromino could move, <see cref="false"/> otherwise.</returns>
    protected bool Move(Vector2 direction)
    {
        var newPosition = CalculatePosition(direction, GlobalPosition);
        if (newPosition.HasValue)
        {
            GlobalPosition = newPosition.Value;
            if (direction != Vector2.Down)
                CallUpdateGhostPosition.CallDeferred(); // TODO: Fix deferred call
            return true;
        }
        return false;
    }

    /// <summary>
    /// Rotate the tetromino.
    /// </summary>
    protected void Rotate()
    {
        if (PieceData.TetrominoType == TetrominoType.O)
            return;

        var direction = 1;
        var originalRotationIndex = RotationIndex;

        ApplyRotation(direction);

        RotationIndex = Wrap((RotationIndex + direction), 4);

        if (!TestWallKicks(RotationIndex, direction))
        {
            RotationIndex = originalRotationIndex;
            ApplyRotation(-direction);
        }

        CallUpdateGhostPosition.CallDeferred(); // TODO: Fix deferred call
    }

    /// <summary>
    /// Calculate and update <see cref="GhostTetromino"/>.
    /// </summary>
    /// <returns><see cref="true"/> if ghost position could be calculated, <see cref="false"/> otherwise.</returns>
    protected bool UpdateGhostPosition()
    {
        Vector2? finalDropPosition = null;
        var ghostPositionUpdate = CalculatePosition(Vector2.Down, GlobalPosition);
        while (ghostPositionUpdate.HasValue)
        {
            finalDropPosition = ghostPositionUpdate;
            ghostPositionUpdate = CalculatePosition(Vector2.Down, ghostPositionUpdate.Value);
        }

        if (finalDropPosition.HasValue)
        {
            GhostTetromino.Show();
            var piecePositions = Pieces
                .Select(piece => piece.Position)
                .ToArray();

            GhostTetromino.SetGhostTetromino(finalDropPosition.Value, piecePositions);
            return true;
        }
        else
        {
            GhostTetromino.Hide();
            return false;
        }
    }

    /// <summary>
    /// Called when <see cref="MoveTimer"/> reaches timeout.
    /// </summary>
    protected void OnMoveTimerTimeout()
    {
        if (!Move(Vector2.Down))
            Lock();
    }

    /// <summary>
    /// Drop the tetromino on <see cref="Board"/>.
    /// </summary>
    private void Drop()
    {
        while (Move(Vector2.Down))
            continue;
        Lock();
    }

    /// <summary>
    /// Lock the tetromino to <see cref="Board"/>.
    /// </summary>
    private void Lock()
    {
        _board.Difficulty.MoveTimer.Stop();
        EmitSignal(SignalName.LockTetromino, this);
        SetProcessInput(false);

        GhostTetromino.QueueFree();
    }

    /// <summary>
    /// Calculate new position for attempted <see cref="Move(Vector2)"/>.
    /// </summary>
    /// <param name="direction">Direction Vector of movement.</param>
    /// <param name="startingGlobalPosition">Position where move is started from.</param>
    /// <returns>New position, or <see cref="null"/> if move is not allowed.</returns>
    private Vector2? CalculatePosition(Vector2 direction, Vector2 startingGlobalPosition)
    {
        var newPosition = startingGlobalPosition + direction * Board.PIECE_SIZE;

        if (!IsWithinGameBounds(newPosition))
            return null;

        if (IsCollidingWithOtherTetrominos(newPosition))
            return null;

        return newPosition;
    }

    /// <summary>
    /// Test if the tetromino can perform the rotation and move away from wall if possible and necessary for the rotation.
    /// </summary>
    /// <param name="rotationIndex">Index of rotation state.</param>
    /// <param name="direction">Direction of the rotation.</param>
    /// <returns><see cref="true"/> if rotation was possible, <see cref="false"/> otherwise.</returns>
    private bool TestWallKicks(int rotationIndex, int direction)
    {
        var wallKickIndex = GetWallKickIndex(rotationIndex, direction);
        for (int i = 0; i < WallKicks[0].Length; i++)
        {
            var translation = WallKicks[wallKickIndex][i];
            if (Move(translation))
                return true;
        }
        return false;
    }

    /// <summary>
    /// Get correct wallkick index that matches <paramref name="rotationIndex"/> and <paramref name="direction"/>.
    /// </summary>
    /// <param name="rotationIndex">Index of rotation state.</param>
    /// <param name="direction">Direction of the rotation.</param>
    /// <returns>Wallkick index for <see cref="Autoload.WallKicksI"/> or <see cref="Autoload.WallKicksOthers"/>, depending on <see cref="TetrominoType"/>.</returns>
    private int GetWallKickIndex(int rotationIndex, int direction)
    {
        var wallKickIndex = rotationIndex * 2;
        if (direction < 0)
            wallKickIndex -= 1;

        return Wrap(wallKickIndex, WallKicks.Length);
    }

    /// <summary>
    /// Apply rotation to the tetromino.
    /// </summary>
    /// <param name="direction">Direction of the rotation.</param>
    private void ApplyRotation(int direction)
    {
        var rotationMatrix = direction == 1 ? Autoload.ClockwiseRotationMatrix : Autoload.CounterClockwiseRotationMatrix;

        // Calculate cells based on rotation matrix
        for (int i = 0; i < TetrominoCells.Length; i++)
        {
            var cell = TetrominoCells[i];
            var coordinates = rotationMatrix[0] * cell.X + rotationMatrix[1] * cell.Y;
            TetrominoCells[i] = coordinates;
        }

        // Move pieces
        for (int i = 0; i < Pieces.Count; i++)
        {
            var piece = Pieces[i];
            piece.Position = TetrominoCells[i] * Piece.Size;
        }
    }

    /// <summary>
    /// Process input for mouse movement.
    /// </summary>
    /// <param name="event">Event with input details.</param>
    private void ProcessMouseInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseEvent)
        {
            if (mouseEvent.ButtonIndex == MouseButton.Left)
            {
                // Start dragging any click
                if (!Dragging && mouseEvent.Pressed)
                {
                    _dragStart = mouseEvent.Position;
                }

                // Stop dragging if the button is released.
                if (Dragging && !mouseEvent.Pressed)
                {
                    _dragStart = null;
                }
            }
            else if (mouseEvent.Pressed && mouseEvent.ButtonIndex == MouseButton.Right)
            {
                Rotate();
            }


        }
        else if (@event is InputEventMouseMotion motionEvent && Dragging)
        {
            // While dragging, do stuff
            var movement = motionEvent.Position - _dragStart ?? Vector2.Zero;

            if (movement.Y <= -movementThreshold * 2)
            {
                Drop();
                _dragStart = motionEvent.Position;
            }
            else if (movement.Y >= movementThreshold)
            {
                Move(Vector2.Down);
                _dragStart = motionEvent.Position;
            }
            else if (movement.X <= -movementThreshold)
            {
                Move(Vector2.Left);
                _dragStart = motionEvent.Position;
            }
            else if (movement.X >= movementThreshold)
            {
                Move(Vector2.Right);
                _dragStart = motionEvent.Position;
            }
        }
    }

    /// <summary>
    /// Process input for touch gesture movement.
    /// </summary>
    /// <param name="event">Event with input details.</param>
    private void ProcessTouchInput(InputEvent @event)
    {
        if (@event is InputEventScreenTouch touchEvent)
        {
            if (touchEvent.DoubleTap)
            {
                Rotate();
            }
            else if (!Dragging && touchEvent.Pressed)
            {
                _dragStart = touchEvent.Position;
            }
            else if (Dragging && !touchEvent.Pressed)
            {
                // Stop dragging if the touch is released.
                _dragStart = null;
            }
        }
        else if (@event is InputEventScreenDrag motionEvent && Dragging)
        {
            // While dragging, do stuff
            var movement = motionEvent.Position - _dragStart ?? Vector2.Zero;

            if (movement.Y <= -movementThreshold * 2)
            {
                Drop();
                _dragStart = motionEvent.Position;
            }
            else if (movement.Y >= movementThreshold)
            {
                Move(Vector2.Down);
                _dragStart = motionEvent.Position;
            }
            else if (movement.X <= -movementThreshold)
            {
                Move(Vector2.Left);
                _dragStart = motionEvent.Position;
            }
            else if (movement.X >= movementThreshold)
            {
                Move(Vector2.Right);
                _dragStart = motionEvent.Position;
            }
        }
    }

    //Helper for wrapping
    private static int Wrap(int k, int n)
    {
        int remainder = k % n;
        return (remainder < 0) ? remainder + n : remainder;
    }
}
