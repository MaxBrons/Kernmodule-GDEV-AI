using UnityEngine;

namespace KMGDEVAI.BehaviourSystem.Nodes
{
    public sealed class UpdateStateHeaderNode : BaseNode
    {
        private StateHeaderUI _ui;
        private string _header;
        private Color _textColor;
        private Color _backgroundColor;

        public UpdateStateHeaderNode(StateHeaderUI ui, string header, Color textColor, Color backgroundColor)
        {
            _ui = ui;
            _header = header;
            _textColor = textColor;
            _backgroundColor = backgroundColor;
        }

        protected override Status OnUpdate()
        {
            _ui.SetHeader(_header, _textColor, _backgroundColor);
            return Status.Success;
        }
    }
}