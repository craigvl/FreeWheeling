using System.Web.Mvc;

namespace FreeWheeling.UI.Infrastructure.Messages
{
    public static class Message
    {
        public static void ShowMessage(this Controller controller, MessageType messageType, string message, bool showAfterRedirect = false, MessagePosition messagePos = MessagePosition.TopCentre, bool persistMessage = false)
        {
            var messageTypeKey = messageType.ToString();

            if (showAfterRedirect)
            {
                controller.TempData[messageTypeKey] = message;
                controller.TempData["MessagePosition"] = messagePos.ToString();
                controller.TempData["PersistMessage"] = persistMessage.ToString();
            }
            else
            {
                controller.ViewData[messageTypeKey] = message;
                controller.ViewData["MessagePosition"] = messagePos.ToString();
                controller.ViewData["PersistMessage"] = persistMessage.ToString();
            }
        }
    }
}