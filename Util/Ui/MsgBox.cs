using MessageBox.Avalonia.Enums;
using System.Threading.Tasks;
using MessageBox.Avalonia;

namespace Sikor.Util.Ui
{
    /**
     * <summary>
     * Message box util: allows to display a prompt or message using Avalonia UI
     * styled view components without the need to actually define their views in
     * the project.
     * </summary>
     */
    public class MsgBox
    {
        /**
        * <summary>
        * Creates a new message box which can only inform or prompt for action.
        * </summary>
        * <param name="title">Contents to display in the title bar.</param>
        * <param name="content">Text, contents</param>
        * <param name="type">Type of the icon: warning, success etc.</param>
        * <param name="buttons">Buttons, defaults to only `Okay`.</param>
        * <returns>Result of the usage: informs which button was clicked.</returns>
        */
        async public static Task<ButtonResult> Show(string title, string content, Icon type, ButtonEnum buttons = ButtonEnum.Ok)
        {
            var msgboxParams = new MessageBox.Avalonia.DTO.MessageBoxStandardParams()
            {
                CanResize = false, //we do not allow resizing yet we are still limited to the development of AvaloniaUI
                ContentMessage = content,
                ContentTitle = title,
                ShowInCenter = true,
                Icon = type,
                ButtonDefinitions = buttons
            };

            msgboxParams.Window.MaxWidth = 400;
            msgboxParams.Window.MaxHeight = 400;
            msgboxParams.Window.CanResize = false;
            msgboxParams.Window.Title = title;

            return await MessageBoxManager.GetMessageBoxStandardWindow(msgboxParams).Show();
        }
    }
}
