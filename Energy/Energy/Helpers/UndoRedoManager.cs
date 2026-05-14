using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Energy.Helpers
{
    public class UndoRedoManager<T>
    {
        private Stack<T> _undoStack = new Stack<T>();
        private Stack<T> _redoStack = new Stack<T>();

        public bool CanUndo => _undoStack.Count > 0;
        public bool CanRedo => _redoStack.Count > 0;

        public void Execute(T action, Action<T> doAction)
        {
            doAction(action);
            _undoStack.Push(action);
            _redoStack.Clear();
        }

        public T Undo(Action<T> undoAction)
        {
            if (!CanUndo) return default;

            var action = _undoStack.Pop();
            undoAction(action);
            _redoStack.Push(action);
            return action;
        }

        public T Redo(Action<T> redoAction)
        {
            if (!CanRedo) return default;

            var action = _redoStack.Pop();
            redoAction(action);
            _undoStack.Push(action);
            return action;
        }

        public void Clear()
        {
            _undoStack.Clear();
            _redoStack.Clear();
        }
    }
}
