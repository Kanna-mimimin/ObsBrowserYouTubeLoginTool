using ObsBrowserYouTubeLoginTool.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace ObsBrowserYouTubeLoginTool.Converters
{
    internal class CommandParameterConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            string localState = (string)values[0];
            string srcCookieFilePath = (string)values[1];
            string hostKey = (string)values[2];
            string destCookieFilePath = (string)values[3];

            return new CommandParameter() {LocalStateFilePath = localState, SrcCookieFilePath = srcCookieFilePath, HostKey = hostKey, DestCookieFilePath = destCookieFilePath };
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
