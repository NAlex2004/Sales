﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Sales.SaleSource.Validation
{

    public class FileNameValidator
    {
        private const string DATE_FORMAT = "ddMMyyyy";
        private const int INITIALS_LENGTH = 5;
        private const string FILENAME_PATTERN = @"[A-Z]{1}[a-z]{1}[A-Z]{1}[a-z]{2}_\d{8}\.[a-zA-Z]{1,}";

        /// <summary>
        /// Valid file name like this: IvIva_19112012.json
        /// [prefix]_DDMMYYYY.[extension]
        /// </summary>        
        public static bool Validate(string fileName)
        {
            bool isValid = false;

            Regex fileNameRegex = new Regex(FILENAME_PATTERN);
            try
            {
                if (fileNameRegex.IsMatch(fileName))
                {                    
                    string dateString = fileName.Substring(INITIALS_LENGTH + 1, 8);
                    var date = DateTime.ParseExact(dateString, DATE_FORMAT, CultureInfo.InvariantCulture);
                    isValid = true;
                }
            }
            catch (Exception)
            {
            }

            return isValid;
        }
    }
}
