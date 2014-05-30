using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FreeWheeling.UI.Infrastructure.Messages;

namespace FreeWheeling.UI.Infrastructure.Messages
{
    public static class MessageHtmlHelper
    {
        // Render all messages that have been set during execution of the controller action.
        public static HtmlString RenderMessages(this HtmlHelper htmlHelper)
        {
            var messages = String.Empty;
            foreach (var messageType in Enum.GetNames(typeof(MessageType)))
            {
                //Retrieve the message from the ViewData or TempData, depending on which has been used
                var message = htmlHelper.ViewContext.ViewData.ContainsKey(messageType)
                                ? htmlHelper.ViewContext.ViewData[messageType]
                                : htmlHelper.ViewContext.TempData.ContainsKey(messageType)
                                    ? htmlHelper.ViewContext.TempData[messageType]
                                    : null;

                //Retrieve the message position 
                var messagePos = htmlHelper.ViewContext.ViewData.ContainsKey("MessagePosition")
                                    ? htmlHelper.ViewContext.ViewData["MessagePosition"] 
                                    : htmlHelper.ViewContext.TempData.ContainsKey("MessagePosition") 
                                        ? htmlHelper.ViewContext.TempData["MessagePosition"] 
                                        : null;

                //Retrieve the persist boolean value
                bool persistMessage = htmlHelper.ViewContext.ViewData.ContainsKey("PersistMessage")
                                        ? Convert.ToBoolean(htmlHelper.ViewContext.ViewData["PersistMessage"].ToString())
                                        : htmlHelper.ViewContext.TempData.ContainsKey("PersistMessage")
                                            ? Convert.ToBoolean(htmlHelper.ViewContext.TempData["PersistMessage"].ToString())
                                            : false;

                if (message != null)
                {
                    //Set css classes based on the message type
                    //Note: The Bootstrap css classes are being used below (eg. alert alert-success)
                    string messageTypeCssClass = "";
                    string messageHeading = "";
                    string persistCssClass = "tempoarayMessage";

                    switch (messageType)
                    {
                        case "Success":
                            messageTypeCssClass = "alert alert-success";
                            //messageHeading = "<strong>Success! </strong>";
                            break;
                        case "Warning":
                            messageTypeCssClass = "alert alert-warning";
                            //messageHeading = "<strong>Warning! </strong>";
                            break;
                        case "Error":
                            messageTypeCssClass = "alert alert-danger";
                            //messageHeading = "<strong>Error! </strong>";
                            break;
                        default:
                            messageTypeCssClass = "alert alert-info";
                            //messageHeading = "<strong>Warning! </strong>";
                            break;
                    }

                    //Determine if the message should be temporary or persistent
                    if (persistMessage)
                    {
                        persistCssClass = "persistMessage";
                    }

                    //The div with the actual message being displayed
                    var messageBoxBuilder = new TagBuilder("div");
                    messageBoxBuilder.AddCssClass("messagebox");
                    messageBoxBuilder.InnerHtml = messageHeading + message.ToString();
                    
                    //Inner button (x) to the right of the message (from Bootstrap)
                    //var closeButton = new TagBuilder("button");
                    //closeButton.MergeAttribute("type", "button");
                    //closeButton.MergeAttribute("data-dismiss", "alert");
                    //closeButton.AddCssClass("close");
                    //closeButton.InnerHtml = "&times;";
                    
                    //Parent div that holds all the elements for display
                    var messageBoxBuilderOuter = new TagBuilder("div");
                    messageBoxBuilderOuter.MergeAttribute("id", "messagewrapper");
                    messageBoxBuilderOuter.MergeAttribute("style", "display: none");
                    messageBoxBuilderOuter.AddCssClass(messageTypeCssClass + " " + messagePos + " " + persistCssClass);

                    //Append close button and message div to the outer div
                    //messageBoxBuilderOuter.InnerHtml = closeButton.ToString() + messageBoxBuilder.ToString();
                    messageBoxBuilderOuter.InnerHtml = messageBoxBuilder.ToString();
                                                            
                    //Return the whole string
                    messages += messageBoxBuilderOuter.ToString();
                }
            }
            return MvcHtmlString.Create(messages);
        }
    }
}