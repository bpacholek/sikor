using System;
using System.Collections.Generic;
using System.Text;
using MessageBox.Avalonia;
using MessageBox.Avalonia.Enums;

namespace Sikor.Util
{
    public class MsgBox
    {
        async public static System.Threading.Tasks.Task<ButtonResult> Show(string title, string content, MessageBox.Avalonia.Enums.Icon type, MessageBox.Avalonia.Enums.ButtonEnum buttons = ButtonEnum.Ok)
        {
            var msgboxParams = new MessageBox.Avalonia.DTO.MessageBoxStandardParams();
            msgboxParams.CanResize = false;
            msgboxParams.ContentMessage = content;
            msgboxParams.ContentTitle = title;
            msgboxParams.ShowInCenter = true;
            msgboxParams.Icon = type;
            msgboxParams.ButtonDefinitions = buttons;
            return await MessageBox.Avalonia.MessageBoxManager.GetMessageBoxStandardWindow(msgboxParams).Show();
        }
    }
}
