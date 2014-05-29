using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FreeWheeling.UI.Infrastructure.Messages
{
    public enum MessageType
    {
        Success,
        Warning,
        Error
    }

    public enum MessagePosition
    {
        TopLeft,
        TopCentre,
        TopRight
    }
}