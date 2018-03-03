using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MapGen.View.GUI.Windows;

namespace MapGen.View.Source.Interfaces
{
    public interface IMessage
    {
        event Action<bool> ReturnResult;

        void ShowMessage(string title, string text, MessageButton butonType, MessageType messageType);
    }
}
