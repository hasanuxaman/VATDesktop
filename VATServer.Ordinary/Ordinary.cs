using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using VATViewModel.DTOs;

namespace VATServer.Ordinary
{
    public class OrdinaryVATDesktop
    {

        #region Property

        public static string CompanyID { get; set; }
        public static string CompanyNameLog { get; set; }
        public static string CompanyName { get; set; }
        public static string CompanyLegalName { get; set; }
        public static string Address1 { get; set; }
        public static string Address2 { get; set; }
        public static string Address3 { get; set; }
        public static string City { get; set; }
        public static string ZipCode { get; set; }
        public static string TelephoneNo { get; set; }
        public static string FaxNo { get; set; }
        public static string Email { get; set; }
        public static string ContactPerson { get; set; }
        public static string ContactPersonDesignation { get; set; }
        public static string ContactPersonTelephone { get; set; }
        public static string ContactPersonEmail { get; set; }
        public static string TINNo { get; set; }
        public static string VatRegistrationNo { get; set; }
        public static string Comments { get; set; }
        public static string ActiveStatus { get; set; }
        public static DateTime FMonthStart { get; set; }
        public static DateTime FMonthEnd { get; set; }
        public static decimal VATAmount { get; set; }
        public static string IsWCF { get; set; }
        public static string Section { get; set; }

        public static int BranchId = 0;
        public static string BranchCode = "";

        public static string CurrentUser { get; set; }
        public static string CurrentUserID { get; set; }
        public static bool IsLoading { get; set; }
        public static string R_F { get; set; }
        public static string fromOpen { get; set; }
        public static string SalesType { get; set; }
        public static string Trading { get; set; }

        public static string DatabaseName { get; set; }

        public static string[] PublicRollLines { get; set; }
        public static DateTime SessionDate { get; set; }
        public static DateTime SessionTime { get; set; }
        public static int ChangeTime { get; set; }
        public static DateTime ServerDateTime { get; set; }
        public static DateTime vMinDate = Convert.ToDateTime("1753/01/02");
        public static DateTime vMaxDate = Convert.ToDateTime("9998/12/30");
        public static bool successLogin = false;
        public static string FontSize = "8";

        public static string Access { get; set; }
        public static string Post { get; set; }
        public static DateTime LicenceDate { get; set; }
        public static DateTime serverDate { get; set; }

        public static bool IsTrial = false;
        public static string Trial = "";
        public static string TrialComments = "Unregister Version";

        public static string ImportFileName { get; set; }
        public static string ItemType = "Other";

        public static bool IsAlpha = false;
        public static string Alpha = "";
        public static string AlphaComments = "Alpha Version";

        public static bool IsBeta = false;
        public static string Beta = "";
        public static string BetaComments = "Beta Version";
        public static string AppPathForRootLocation = AppDomain.CurrentDomain.BaseDirectory;

        public static bool IsBureau = false;

        public static string Add { get; set; }
        public static string Edit { get; set; }

        public static bool IsDHLCrat { get; set; }



        #endregion

        public static bool IsPhoneNumberValid(string phoneNumber)
        {
            // Define a regular expression pattern for a valid 11-digit phone number
            string pattern = @"^\d{11}$";

            // Use Regex.IsMatch to check if the phone number matches the pattern
            return Regex.IsMatch(phoneNumber, pattern);
        }
        public static bool IsEmailValid(string emailAddress)
        {
            // Define a regular expression pattern for a valid email address
            string pattern = @"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$";

            // Use Regex.IsMatch to check if the email address matches the pattern
            return Regex.IsMatch(emailAddress, pattern);
        }

        public static string StringReplacingForHTML(string stringToReplace)
        {
            string newString = stringToReplace;

            newString = newString.Replace("'", "&#39;");
           
            return newString;
        }

        public static string FormatingNumeric(string txtBox, string FormNumeric, int DecPlace = 2)
        {
            string inputValue = txtBox;
            string outPutValue = "0";
            try
            {

                outPutValue = ParseDecimal(inputValue, FormNumeric);

            }

            #region Catch

            catch (Exception ex)
            {
                throw ex;
            }
            #endregion Catch

            return outPutValue;
        }

        public static string ParseDecimal(string numb, string FormNumeric)
        {

            String val = "0";
            try
            {
                numb = numb.Replace(",", "");

                if (string.IsNullOrWhiteSpace(numb))
                {
                    numb = "0";
                }

                string Pre = "";
                Pre = Pre.PadRight(Convert.ToInt32(FormNumeric), '#');

                val = decimal.Parse(numb.ToString(), System.Globalization.NumberStyles.Float).ToString("#,###0." + Pre);

            }

            #region Catch

            catch (Exception ex)
            {
                throw ex;
            }
            #endregion Catch

            return val;
        }


        public static DataTable EmptyRowCheckAndDelete(DataTable table, List<ErrorMessage> msgs)
        {
            bool emptyRow = false;
            bool dataRow = false;
            ErrorMessage msg = new ErrorMessage();
            for (int i = 0; i < table.Rows.Count; i++)
            {
                foreach (DataColumn column in table.Columns)
                {
                    if (table.Rows[i][column.ColumnName] == DBNull.Value
                        //||string.IsNullOrWhiteSpace( table.Rows[i][column.ColumnName].ToString())
                        )
                    {
                        msg = new ErrorMessage();
                        emptyRow = true;
                        msg.ColumnName = "Empty/Blank Row";
                        msg.Message = "Column Name: " + column.ColumnName + " and Row No: " + (i + 2).ToString() + " Has Empty/Blank Data";
                        msgs.Add(msg);
                    }
                    else
                    {
                        dataRow = true;
                    }
                }

                if (emptyRow == true && dataRow == false)
                {
                    msg = new ErrorMessage();
                    table.Rows[i].Delete();
                    msg.ColumnName = "Delete Row";
                    msg.Message = "Deleted Row Number : " + (i + 2).ToString();
                    msgs.Add(msg);

                }
                emptyRow = false;
                dataRow = false;
            }
            table.AcceptChanges();
            return table;
        }

        public static DataTable ParseDecimal_Dt(DataTable dt, string[] ColumnNames)
        {

            DataColumnCollection columns = dt.Columns;

            try
            {

                foreach (string ColumeName in ColumnNames)
                {
                    if (columns.Contains(ColumeName))
                    {

                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            string cellValue = dt.Rows[i][ColumeName].ToString();

                            if (!string.IsNullOrWhiteSpace(cellValue))
                            {
                                decimal ParseDecimal = Decimal.Parse(cellValue, System.Globalization.NumberStyles.Float);

                                dt.Rows[i][ColumeName] = ParseDecimal;
                            }
                        }

                    }

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        public static bool IsACIFGReturnFromSales(string CompanyCode, string transactionType)
        {
            //bool value = false;

            //if (transactionType == "ReceiveReturn")
            //{
            //    if (CompanyCode == "ACI-1")
            //    {
            //        value = true;
            //    }
            //}

            return transactionType == "ReceiveReturn" && IsACICompany(CompanyCode);

        }

        public static bool IsACICompany(string value)
        {

            return value == "ACI-1" || value == "ACI" || value == "CEPL" || value.ToLower() == "core cb" ||
                   value == "ACI-TEST" || value == "YAMAHAFACTORY" || value == "ACI CB TRADING" ||
                   value == "MOTORCENTRAL" || value == "FORMULATION" || value == "MOTORSSERVICE" ||
                   value.ToLower() == "aci cb hygine" ||
                   value.ToLower() == "aci toiletries" || value.ToLower() == "ppl" ||
                   value.ToLower() == "aci edible oils limited" ||
                   value.ToLower() == "aci foods limited" || value.ToUpper() == "ACI FOODS LIMITED (RICE UNIT)" ||
                   value.ToUpper() == "ACI PURE FLOUR LIMITED" ||
                   value.ToUpper() == "ACI CB ELECTRICAL" || value.ToUpper() == "AH MODONPUR CENTRAL WH" ||
                   value.ToUpper() == "ACI SALT LIMITED";

        }

        public static T CopyObject<T>(T obj) where T : class, new()
        {
            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(obj));
        }

        public static string NourishCompanyId(string value)
        {
            string companyCode = "";

            if (value == "")
            {
                companyCode = "01";
            }
            else if (value == "")
            {
                companyCode = "03";
            }
            else if (value == "")
            {
                companyCode = "05";
            }


            return companyCode;

        }

        public static bool IsNourishCompany(string value)
        {
            return value == "01" || value == "03" || value == "05";

        }

        public static bool IsUnileverCompany(string value)
        {

            return value == "AA03" || value == "DH01" || value == "DH02" || value == "DH03" || value == "DH04" || value == "DH05" || value == "DH06" || value == "DH07" || value == "DH08" || value == "DH09" || value == "DH10" || value == "DH11" || value == "DH12"
                   || value == "DH13" || value == "DH14" || value == "DH15" || value == "DH16" || value == "DH17" || value == "DH18" || value == "DH19" || value == "DH20" || value == "DH21" || value == "DH22" || value == "DH23" || value == "DH24"
                   || value == "DH25" || value == "DH26" || value == "DH27" || value == "DH28" || value == "DH29" || value == "CH01" || value == "CH02" || value == "CH03" || value == "CH04" || value == "CH05" || value == "CH12" || value == "CH06" || value == "SY14"
                   || value == "CH07" || value == "CH08" || value == "CH09" || value == "CH10" || value == "CH11" || value == "DH30" || value == "SY01" || value == "SY02" || value == "SY03" || value == "SY04" || value == "SY05" || value == "SY06" || value == "CH15" || value == "CH16" || value == "CH17" || value == "CH18"
                   || value == "SY07" || value == "SY08" || value == "SY09" || value == "SY10" || value == "SY11" || value == "SY12" || value == "SY13" || value == "DH31" || value == "CH12" || value == "CH13" || value == "CH14" || value == "MY01" || value == "MY02" || value == "RN01" || value == "RJ01" || value == "RJ04"|| value == "RJ05"
                   || value == "RN02" || value == "RN03" || value == "BR01" || value == "DH32" || value == "KH01" || value == "KH02" || value == "KH03" || value == "DH35" || value == "DH36" || value == "DH37" || value == "DH38" || value == "DH40" || value == "RJ02" || value == "RJ03" || value == "DH42" || value == "DH43" || value == "DH45" || value == "DH46" || value == "DH47" || value == "DH48" || value == "DH49";

        }

        public static bool IsNBR(string value)
        {

            return value == "NBR" || value == "admin";

        }

        public static void DataTable_DateFormat(DataTable dt, string ColumnName)
        {
            int count = 0;
            foreach (DataRow dataRow in dt.Rows)
            {
                try
                {
                    dataRow[ColumnName] = OrdinaryVATDesktop.DateToDate(dataRow[ColumnName].ToString());
                    count++;
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }

        public static string ParseDecimal_DecimalPlace(object numb, int DecimalPlace)
        {

            String val = "0";
            string numbField = string.Empty;
            try
            {
                for (int i = 0; i < numb.ToString().Length; i++)
                {

                    if (numb.ToString()[i].ToString() == ".")
                    {
                        numbField += numb.ToString()[i];
                    }
                    else
                    {
                        if (Char.IsDigit(numb.ToString()[i]))
                            numbField += numb.ToString()[i];
                    }
                }

                if (string.IsNullOrWhiteSpace(numbField.ToString()))
                {
                    numbField = "0";
                }
                else
                {
                    numbField = numbField.ToString().Replace(",", "");
                }
                string Pre = "";
                Pre = Pre.PadRight(DecimalPlace, '#');
                val = decimal.Parse(numbField.ToString(), System.Globalization.NumberStyles.Float).ToString("#,###0." + Pre);

            }
            catch { }
            return val;
        }

        public static string DateDifferenceYMD(string fDate, string tDate, bool day)
        {
            string returnValue = "";
            try
            {
                DateTime fromDate = DateTime.ParseExact(fDate, "yyyyMMdd", CultureInfo.InvariantCulture);
                DateTime now = DateTime.ParseExact(tDate, "yyyyMMdd", CultureInfo.InvariantCulture);
                fromDate = fromDate.Date;
                now = now.Date.AddDays(1);

                var days = now.Day - fromDate.Day;
                if (days < 0)
                {
                    var newNow = now.AddMonths(-1);
                    days += (int)(now - newNow).TotalDays;
                    now = newNow;
                }
                var months = now.Month - fromDate.Month;
                if (months < 0)
                {
                    months += 12;
                    now = now.AddYears(-1);
                }
                var years = now.Year - fromDate.Year;

                if (!day)
                {
                    days = 0;
                }
                if (years != 0)
                {
                    returnValue = years.ToString() + " years ";
                }
                if (months != 0)
                {
                    returnValue += months.ToString() + " months ";
                }
                if (days != 0)
                {
                    returnValue += days.ToString() + " days";
                }
            }
            catch (Exception)
            {

            }

            if (returnValue == "")
                returnValue = "Less than a month";

            return returnValue;
        }
        public static string DateToString(string val)
        {
            if (string.IsNullOrEmpty(val)) return "";
            try
            {
                val = DateTime.Parse(val).ToString("yyyyMMdd");
            }
            catch (Exception)
            {
                val = "";
            }
            return val;
        }
        public static string ToTitle(string input)
        {
            string text = string.Empty;
            text = input;// "MD. KAMRUL HASAN";
            string output = string.Empty;
            int c = text.Length;
            for (int i = 0; i < c; i++)
            {
                if (i == 0)
                {
                    output = output + text[i].ToString().ToUpper();

                }
                else if (text[i].ToString() == " ")
                {
                    output = output + text[i].ToString();
                    output = output + text[i + 1].ToString().ToUpper();
                    i++;
                }
                else
                {
                    output = output + text[i].ToString().ToLower();
                }
            }
            return output;
        }
        public static string StringToDate(string val)
        {
            if (string.IsNullOrEmpty(val)) return "";
            try
            {
                val = DateTime.ParseExact(val, "yyyyMMdd", CultureInfo.InvariantCulture).ToString("dd-MMM-yyyy");
            }
            catch (Exception)
            {
                val = "";
            }
            return val;
        }
        public static string TimeToString(string val)
        {
            if (string.IsNullOrEmpty(val)) return "";
            try
            {
                val = DateTime.Parse(val).ToString("HHmm");
            }
            catch (Exception)
            {
                val = "";
            }
            return val;
        }
        public static string StringToTime(string val)
        {
            string returnVal = "";
            if (string.IsNullOrEmpty(val)) return returnVal;
            try
            {
                int hour = Convert.ToInt32(val.Substring(0, 2));
                string meridiem = "";
                meridiem = hour > 12 ? "PM" : "AM";
                if (hour > 12)
                {
                    hour = hour - 12;
                }
                returnVal = hour + ":" + val.Substring(2, 2) + " " + meridiem;
            }
            catch (Exception)
            {

                returnVal = "";
            }
            return returnVal;
        }
        public static DateTime StringToDateAsDate(string val)
        {

            try
            {
                val = DateTime.ParseExact(val, "yyyyMMdd", CultureInfo.InvariantCulture).ToString("dd-MMM-yyyy");
            }
            catch (Exception)
            {
                val = "";
            }
            return Convert.ToDateTime(val);
        }

        public static string RemoveStringExpresion(string vstr)
        {
            string str = vstr;

            str = str.Replace("\n", String.Empty);
            str = str.Replace("\r", String.Empty);
            str = str.Replace("\t", String.Empty);
            str = str.Replace("'", String.Empty);
            var demo = str.Split(' ').Where(s => !string.IsNullOrWhiteSpace(s));
            //str = str.Replace("  ", " ");
            str = string.Join(" ", demo);
            return str;
        }
        public static DateTime StringToTimeAsTime(string val)
        {
            var returnVal = string.Empty;
            try
            {
                int hour = Convert.ToInt32(val.Substring(0, 2));
                string meridiem = "";
                meridiem = hour > 12 ? "PM" : "AM";
                if (hour > 12)
                {
                    hour = hour - 12;
                }
                returnVal = hour + ":" + val.Substring(2, 2) + " " + meridiem;
            }
            catch (Exception)
            {

                returnVal = "";
            }
            return Convert.ToDateTime(returnVal);
        }

        public static string StringReplacing(string stringToReplace)
        {
            string newString = stringToReplace;
            if (stringToReplace.Contains("."))
            {
                newString = Regex.Replace(stringToReplace, @"^[^.]*.", "", RegexOptions.IgnorePatternWhitespace);
            }
            newString = newString.Replace(">", "From");
            newString = newString.Replace("<", "To");
            newString = newString.Replace("!", "");
            newString = newString.Replace("=", "");
            newString = newString.Replace("isnull", "");
            return newString;
        }
        public static string DateToDateNew(string val)
        {

            if (string.IsNullOrEmpty(val)) return "";
            try
            {
                if (val.Contains("/") || val.Contains("-"))
                {
                    val = Convert.ToDateTime(val).ToString("dd-MMM-yyyy");
                }
                else
                {
                    double d = double.Parse(val);
                    val = DateTime.FromOADate(d).ToString("dd-MMM-yyyy");
                }
            }
            catch (Exception)
            {
                val = "";
            }
            return val;
        }

        public static string DateToDate(string val)
        {
            if (string.IsNullOrEmpty(val)) return "";
            try
            {
                val = val.TrimStart('\'');
                if (val.Contains("/") || val.Contains("-") || val.Contains(".") || val.Trim().Contains(" "))
                {
                    val = Convert.ToDateTime(val).ToString("yyyy-MMM-dd HH:mm:ss");
                }
                else
                {
                    double d = double.Parse(val);
                    val = DateTime.FromOADate(d).ToString("yyyy-MMM-dd HH:mm:ss");
                }
            }
            catch (Exception e)
            {
                val = "";

            }
            return val;
        }

        public static string DateToDate_YMD(string val)
        {
            if (string.IsNullOrEmpty(val)) return "";
            try
            {
                if (val.Contains("/") || val.Contains("-") || val.Trim().Contains(" ") || val.Trim().Contains("."))
                {
                    if (val.Trim().Contains("."))
                    {
                        val = val.Replace(".", "/");
                    }

                    val = Convert.ToDateTime(val).ToString("yyyy-MMM-dd");
                }
                else
                {
                    double d = double.Parse(val);
                    val = DateTime.FromOADate(d).ToString("yyyy-MMM-dd");
                }
            }
            catch (Exception e)
            {
                val = "";

                //////try
                //////{
                //////    val = DateTime.ParseExact(val, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MMM-dd");
                //////}
                //////catch (Exception)
                //////{


                //////}

            }
            return val;
        }

        public static string DateToTime_HMS(string val)
        {
            if (string.IsNullOrEmpty(val)) return "";
            try
            {
                if (val.Contains("/") || val.Contains("-") || val.Trim().Contains(" ") || val.Contains(":"))
                {
                    val = Convert.ToDateTime(val).ToString("HH:mm:ss");
                }
                else
                {
                    double d = double.Parse(val);
                    val = DateTime.FromOADate(d).ToString("HH:mm:ss");
                }
            }
            catch (Exception e)
            {
                val = "";
            }
            return val;
        }

        public static string DateTimeToDate(string val)
        {
            string senderVal = val;
            try
            {
                if (string.IsNullOrEmpty(val.Trim())) return "";
                val = Convert.ToDateTime(val).ToString("dd-MMM-yyyy");

            }
            catch (Exception)
            {
                val = "";
            }

            return val;
        }

        public static string SanitizeInput(string input)
        {
            string sanitized = string.Empty;
            try
            {
                //string pattern = @"[^a-zA-Z0-9\s\-/_~*+@.#$%&()\[\]{}!]";
                string pattern = @"[^a-zA-Z0-9\s\-/_~@.]"; // Allow letters, numbers, spaces, hyphens, forward slashes, underscores, tilde, at and dot signs
                sanitized = Regex.Replace(input, pattern, "");
            }
            catch (Exception ex)
            {
                sanitized = ex.Message;
                throw ex;
            }

            return sanitized;
        }

        public static bool IsNumeric(string valueToCheck)
        {
            try
            {
                decimal tt = 0;
                bool res = Decimal.TryParse(valueToCheck, out tt);

                //Convert.ToDecimal(valueToCheck);
                if (res)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            #region Catch

            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                //FileLogger.Log("Program", "IsNumeric", exMessage);
            }
            #endregion Catch
            return false;

        }

        public static bool IsUnitPriceMandatory(string TransactionType)
        {
            bool IsUnitePrice = true;

            try
            {

                if (TransactionType == "ContractorRawIssue")
                {
                    IsUnitePrice = false;
                }


            }
            #region Catch

            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine + ex.StackTrace;
                }
            }
            #endregion Catch

            return IsUnitePrice;

        }

        public static bool IsNumericScientific(string valueToCheck)
        {
            try
            {
                decimal ParseDecimal = Decimal.Parse(valueToCheck, System.Globalization.NumberStyles.Float);
                return true;
            }
            #region Catch

            catch (Exception ex)
            {
                return false;
            }
            #endregion Catch

        }

        public static bool IsZeroCheck(string valueToCheck)
        {
            bool result = false;
            try
            {
                 decimal ParseDecimal = Decimal.Parse(valueToCheck, System.Globalization.NumberStyles.Float);
                if (ParseDecimal !=0) 
                {
                    result= true;
                }

                else
                {
                    result= false;
                }
                
            }
            #region Catch

            catch (Exception ex)
            {
                return true;
            }


            #endregion Catch

            return result;

        }

        public static bool IsVATRateCheck(decimal valueToCheck)
        {
            try
            {

                decimal[] names = { 15, 10, 7.5m, 5, 4.5m, 3, 2.4m, 2, 0 };

                bool result = names.Any(element => element == valueToCheck);

                return result;

            }
            #region Catch

            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                //FileLogger.Log("Program", "IsNumeric", exMessage);
            }
            #endregion Catch
            return false;

        }

        #region Excell upload Validation

        public ResultVM ExcelReceiveValidation(DataTable data, string columnName)
        {

            ResultVM result = new ResultVM();
            List<ErrorMessage> errorList = new List<ErrorMessage>();
            #region try

            try
            {
                // process

                foreach (DataRow row in data.Rows)
                {
                    if (columnName.ToLower() == "quantity")
                    {
                        if (!OrdinaryVATDesktop.IsNumeric(row[columnName].ToString()))
                        {
                            errorList.Add(new ErrorMessage()
                            {
                                ColumnName = columnName,
                                Message = "Quantity Field is numeric of ID :" + row["ID"]
                            });
                        }
                    }

                    if (columnName.ToLower() == "receive_datetime")
                    {
                        if (!OrdinaryVATDesktop.IsDate(row[columnName].ToString()))
                        {
                            errorList.Add(new ErrorMessage()
                            {
                                ColumnName = columnName,
                                Message = "Receive_DateTime Field is datetime of ID :" + row["ID"]
                            });
                        }
                    }

                    if (columnName.ToLower() == "nbr_price")
                    {
                        if (!OrdinaryVATDesktop.IsNumeric(row[columnName].ToString()))
                        {
                            errorList.Add(new ErrorMessage()
                            {
                                ColumnName = columnName,
                                Message = "NBR_Price Field is numeric of ID :" + row["ID"]
                            });
                        }
                    }

                    if (columnName.ToLower() == "post")
                    {
                        if (!OrdinaryVATDesktop.IsYNCheck(row[columnName].ToString()))
                        {
                            errorList.Add(new ErrorMessage()
                            {
                                ColumnName = columnName,
                                Message = "Post Field is only Y/N of ID :" + row["ID"]
                            });
                        }
                    }
                    if (columnName.ToLower() == "with_toll")
                    {
                        if (!OrdinaryVATDesktop.IsYNCheck(row[columnName].ToString()))
                        {
                            errorList.Add(new ErrorMessage()
                            {
                                ColumnName = columnName,
                                Message = "With_Toll Filed is only Y/N of ID :" + row["ID"]
                            });
                        }
                    }


                }



                if (errorList.Count > 0)
                {
                    result.Status = "fail";
                    result.Message = "fail";
                    result.ErrorList = errorList;

                }
                else
                {

                    result.Status = "Success";
                    result.Message = "Success";
                }


                return result;
            }
            #endregion

            #region catch
            catch (Exception ex)
            {
                ////FileLogger.Log("ReceiveDAL", "ExcelReceiveValidation", ex.ToString());
                throw ex;


            }

            #endregion
        }


        public ResultVM ExcelValidationNumeric(DataTable data, string columnName, string ErrorColumnName)
        {

            ResultVM result = new ResultVM();
            List<ErrorMessage> errorList = new List<ErrorMessage>();
            #region try

            try
            {
                // process

                foreach (DataRow row in data.Rows)
                {
                    if (!OrdinaryVATDesktop.IsNumericScientific(row[columnName].ToString()))
                    {
                        errorList.Add(new ErrorMessage()
                        {
                            ColumnName = columnName,
                            Message = columnName + " Field is numeric of " + ErrorColumnName + " :" + row[ErrorColumnName]
                        });
                    }

                }

                if (errorList.Count > 0)
                {
                    result.Status = "fail";
                    result.Message = "fail";
                    result.ErrorList = errorList;

                }
                else
                {

                    result.Status = "Success";
                    result.Message = "Success";
                }


                return result;
            }
            #endregion

            #region catch
            catch (Exception ex)
            {
                //FileLogger.Log("ReceiveDAL", "ExcelValidationNumeric", ex.ToString());
                throw ex;

            }

            #endregion
        }

        public ResultVM ExcelValidationZeroCheck(DataTable data, string columnName, string ErrorColumnName)
        {

            ResultVM result = new ResultVM();
            List<ErrorMessage> errorList = new List<ErrorMessage>();
            #region try

            try
            {
                // process

                foreach (DataRow row in data.Rows)
                {
                    if (!OrdinaryVATDesktop.IsZeroCheck(row[columnName].ToString()))
                    {
                        errorList.Add(new ErrorMessage()
                        {
                            ColumnName = columnName,
                            Message = columnName + " Field is Not Allowed Zero of " + ErrorColumnName + " :" + row[ErrorColumnName]
                        });
                    }

                }

                if (errorList.Count > 0)
                {
                    result.Status = "fail";
                    result.Message = "fail";
                    result.ErrorList = errorList;

                }
                else
                {

                    result.Status = "Success";
                    result.Message = "Success";
                }


                return result;
            }
            #endregion

            #region catch
            catch (Exception ex)
            {
                //FileLogger.Log("ReceiveDAL", "ExcelValidationNumeric", ex.ToString());
                throw ex;

            }

            #endregion
        }

        public ResultVM ExcelValidationDateTime(DataTable data, string columnName, string ErrorColumnName)
        {

            ResultVM result = new ResultVM();
            List<ErrorMessage> errorList = new List<ErrorMessage>();
            #region try

            try
            {

                foreach (DataRow row in data.Rows)
                {

                    if (!OrdinaryVATDesktop.IsDate(row[columnName].ToString()))
                    {
                        errorList.Add(new ErrorMessage()
                        {
                            ColumnName = columnName,
                            Message = columnName + " Field is datetime of " + ErrorColumnName + " :" + row[ErrorColumnName]
                        });
                    }

                }

                if (errorList.Count > 0)
                {
                    result.Status = "fail";
                    result.Message = "fail";
                    result.ErrorList = errorList;

                }
                else
                {

                    result.Status = "Success";
                    result.Message = "Success";
                }


                return result;
            }
            #endregion

            #region catch
            catch (Exception ex)
            {
                // FileLogger.Log("ReceiveDAL", "ExcelValidationDateTime", ex.ToString());
                throw ex;
            }

            #endregion
        }

        public ResultVM ExcelValidationYNCheck(DataTable data, string columnName, string ErrorColumnName)
        {

            ResultVM result = new ResultVM();
            List<ErrorMessage> errorList = new List<ErrorMessage>();
            #region try

            try
            {

                foreach (DataRow row in data.Rows)
                {

                    if (!OrdinaryVATDesktop.IsYNCheck(row[columnName].ToString()))
                    {
                        errorList.Add(new ErrorMessage()
                        {
                            ColumnName = columnName,
                            Message = columnName + " Field is only Y/N of " + ErrorColumnName + " :" + row[ErrorColumnName]
                        });
                    }

                }

                if (errorList.Count > 0)
                {
                    result.Status = "fail";
                    result.Message = "fail";
                    result.ErrorList = errorList;

                }
                else
                {

                    result.Status = "Success";
                    result.Message = "Success";
                }


                return result;
            }
            #endregion

            #region catch
            catch (Exception ex)
            {
                // FileLogger.Log("ReceiveDAL", "ExcelValidationYNCheck", ex.ToString());
                throw ex;
            }

            #endregion
        }
        public ResultVM ExcelValidationVatnameColumnCheck(DataTable data, string columnName, string ErrorColumnName)
        {

            ResultVM result = new ResultVM();
            List<ErrorMessage> errorList = new List<ErrorMessage>();
            #region try

            try
            {

                foreach (DataRow row in data.Rows)
                {

                    if (!OrdinaryVATDesktop.IsPricedeclarationCheck(row[columnName].ToString()))
                    {
                        errorList.Add(new ErrorMessage()
                        {
                            ColumnName = columnName,
                            Message = columnName + " Field is only Price Declaration name of " + ErrorColumnName + " :" + row[ErrorColumnName]
                        });
                    }

                }

                if (errorList.Count > 0)
                {
                    result.Status = "fail";
                    result.Message = "fail";
                    result.ErrorList = errorList;

                }
                else
                {

                    result.Status = "Success";
                    result.Message = "Success";
                }


                return result;
            }
            #endregion

            #region catch
            catch (Exception ex)
            {
                // FileLogger.Log("ReceiveDAL", "ExcelValidationYNCheck", ex.ToString());
                throw ex;
            }

            #endregion
        }
        public ResultVM ExcelValidationPTypeCheck(DataTable data, string columnName, string ErrorColumnName)
        {

            ResultVM result = new ResultVM();
            List<ErrorMessage> errorList = new List<ErrorMessage>();
            #region try

            try
            {

                foreach (DataRow row in data.Rows)
                {

                    if (!OrdinaryVATDesktop.IsPTypeStringCheck(row[columnName].ToString()))
                    {
                        errorList.Add(new ErrorMessage()
                        {
                            ColumnName = columnName,
                            Message = columnName + " Field is only Type name of " + ErrorColumnName + " :" + row[ErrorColumnName]
                        });
                    }

                }

                if (errorList.Count > 0)
                {
                    result.Status = "fail";
                    result.Message = "fail";
                    result.ErrorList = errorList;

                }
                else
                {

                    result.Status = "Success";
                    result.Message = "Success";
                }


                return result;
            }
            #endregion

            #region catch
            catch (Exception ex)
            {
                // FileLogger.Log("ReceiveDAL", "ExcelValidationYNCheck", ex.ToString());
                throw ex;
            }

            #endregion
        }
        public ResultVM ExcelValidationSTypeCheck(DataTable data, string columnName, string ErrorColumnName)
        {

            ResultVM result = new ResultVM();
            List<ErrorMessage> errorList = new List<ErrorMessage>();
            #region try

            try
            {

                foreach (DataRow row in data.Rows)
                {

                    if (!OrdinaryVATDesktop.IsSTypeStringCheck(row[columnName].ToString()))
                    {
                        errorList.Add(new ErrorMessage()
                        {
                            ColumnName = columnName,
                            Message = columnName + " Field is only Type name of " + ErrorColumnName + " :" + row[ErrorColumnName]
                        });
                    }

                }

                if (errorList.Count > 0)
                {
                    result.Status = "fail";
                    result.Message = "fail";
                    result.ErrorList = errorList;

                }
                else
                {

                    result.Status = "Success";
                    result.Message = "Success";
                }


                return result;
            }
            #endregion

            #region catch
            catch (Exception ex)
            {
                // FileLogger.Log("ReceiveDAL", "ExcelValidationYNCheck", ex.ToString());
                throw ex;
            }

            #endregion
        }

        public ResultVM ExcelQuantityZeroCheck(DataTable data, string columnName)
        {

            ResultVM result = new ResultVM();
            List<ErrorMessage> errorList = new List<ErrorMessage>();
            #region try

            try
            {
                // process

                foreach (DataRow row in data.Rows)
                {
                    string quantity = row[columnName].ToString();
                    decimal qty = Convert.ToDecimal(quantity);

                    if (qty < 0)
                    {
                        errorList.Add(new ErrorMessage()
                        {
                            ColumnName = columnName,
                            Message = columnName + " Field Value can't be zero of ID :" + row["ID"]
                        });

                    }


                }

                if (errorList.Count > 0)
                {
                    result.Status = "fail";
                    result.Message = "fail";
                    result.ErrorList = errorList;

                }
                else
                {

                    result.Status = "Success";
                    result.Message = "Success";
                }


                return result;
            }
            #endregion

            #region catch
            catch (Exception ex)
            {
                // FileLogger.Log("ReceiveDAL", "ExcelValidationNumeric", ex.ToString());
                throw ex;
            }

            #endregion
        }

        #endregion

        public static bool IsYNCheck(string value)
        {
            bool result = false;
            try
            {
                if (value.ToLower() == "y" || value.ToLower() == "n")
                {
                    result = true;
                }
                else
                {
                    result = false;
                }


            }
            #region Catch

            catch (Exception ex)
            {
                result = false;
            }
            #endregion Catch

            return result;
        }
        public static bool IsPricedeclarationCheck(string value)
        {
            bool result = false;
            try
            {
                if (value.ToLower() == "vat 4.3" || value.ToLower() == "vat 4.3 (toll issue)" || value.ToLower() == "vat 4.3 (toll receive)" || value.ToLower() == "vat 4.3 (tender)" || value.ToLower() == "vat 4.3 (package)" || value.ToLower() == "vat 4.3 (wastage)" || value.ToLower() == "vat 4.3 (internal issue)" || value.ToLower() == "vat porisisto ka")
                {
                    result = true;
                }
                else
                {
                    result = false;
                }


            }
            #region Catch

            catch (Exception ex)
            {
                result = false;
            }
            #endregion Catch

            return result;
        }
        public static bool IsPTypeStringCheck(string value)
        {
            bool result = false;
            try
            {
                if (value.ToLower() == "vat" || value.ToLower() == "nonvat" || value.ToLower() == "exempted" || value.ToLower() == "fixedvat" || value.ToLower() == "otherrate"
                     || value.ToLower() == "truncated" || value.ToLower() == "ternover" || value.ToLower() == "unregister" || value.ToLower() == "nonrebate")
                {
                    result = true;
                }
                else
                {
                    result = false;
                }


            }
            #region Catch

            catch (Exception ex)
            {
                result = false;
            }
            #endregion Catch

            return result;
        }
        public static bool IsSTypeStringCheck(string value)
        {
            bool result = false;
            try
            {
                if (value.ToLower() == "vat" || value.ToLower() == "nonvat" || value.ToLower() == "mrprate(sc)" || value.ToLower() == "mrprate"
                   || value.ToLower() == "fixedvat" || value.ToLower() == "otherrate" || value.ToLower() == "retail" || value.ToLower() == "export" || value.ToLower() == "deemexport")
                {
                    result = true;
                }
                else
                {
                    result = false;
                }


            }
            #region Catch

            catch (Exception ex)
            {
                result = false;
            }
            #endregion Catch

            return result;
        }
        public static bool IsDate(string valueToCheck)
        {
            bool result = false;
            try
            {
                Convert.ToDateTime(valueToCheck);
                result = true;
            }
            #region Catch

            catch (Exception ex)
            {
                result = false;
            }
            #endregion Catch

            return result;

        }

        public static string[] Alphabet = new string[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "AA", "AB", "AC", "AD", "AE", "AF", "AG", "AH", "AI", "AJ", "AK", "AL", "AM", "AN", "AO", "AP", "AQ", "AR", "AS", "AT", "AU", "AV", "AW", "AX", "AY", "AZ", "BA", "BB", "BC", "BD", "BE", "BF", "BG", "BH", "BI", "BJ", "BK", "BL", "BM", "BN", "BO", "BP", "BQ", "BR", "BS", "BT", "BU", "BV", "BW", "BX", "BY", "BZ", "CA", "CB", "CC", "CD", "CE", "CF", "CG", "CH", "CI", "CJ", "CK", "CL", "CM", "CN", "CO", "CP", "CQ", "CR", "CS", "CT", "CU", "CV", "CW", "CX", "CY", "CZ", "DA", "DB", "DC", "DD", "DE", "DF", "DG", "DH", "DI", "DJ", "DK", "DL", "DM", "DN", "DO", "DP", "DQ", "DR", "DS", "DT", "DU", "DV", "DW", "DX", "DY", "DZ", "EA", "EB", "EC", "ED", "EE", "EF", "EG", "EH", "EI", "EJ", "EK", "EL", "EM", "EN", "EO", "EP", "EQ", "ER", "ES", "ET", "EU", "EV", "EW", "EX", "EY", "EZ", "FA", "FB", "FC", "FD", "FE", "FF", "FG", "FH", "FI", "FJ", "FK", "FL", "FM", "FN", "FO", "FP", "FQ", "FR", "FS", "FT", "FU", "FV", "FW", "FX", "FY", "FZ" };

        public static string[] MonthNames = new string[] { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };

        public static DataTable DtColumnNameChange(DataTable dt, string oldColumnName, string newColumnName)
        {
            DataTable resultDt = new DataTable();
            resultDt = dt;

            if (dt.Columns.Contains(oldColumnName))
            {
                resultDt.Columns[oldColumnName].ColumnName = newColumnName;
            }

            return resultDt;
        }

        public static DataTable DtSetColumnsOrder(DataTable table, string[] columnNames, int StartIndex = 0)
        {
            int columnIndex = StartIndex;
            foreach (var columnName in columnNames)
            {
                table.Columns[columnName].SetOrdinal(columnIndex);
                columnIndex++;
            }
            return table;
        }

        public static DataTable DtEmptyRowReplace(DataTable dt, string replaceValue = "0")
        {
            int dtrows = dt.Rows.Count;
            int dtcols = dt.Columns.Count;

            for (int m = 0; m < dtrows; m++)
            {
                for (int n = 0; n < dtcols; n++)
                {
                    var tt = dt.Rows[m][n].ToString();
                    if (string.IsNullOrEmpty(tt))
                    {

                    }
                    if (dt.Rows[m][n] != null && string.IsNullOrEmpty(dt.Rows[m][n].ToString().Trim()))
                    {
                        dt.Rows[m][n] = replaceValue;
                    }
                }
            }
            return dt;
        }

        // If Specific Columnns Value Empty the Delete Full Row
        public static DataTable DtEmptyRowDelete(DataTable dt, string columnNames = "")
        {

            for (int i = dt.Rows.Count - 1; i >= 0; i--)
            {
                if (dt.Rows[i][columnNames] == DBNull.Value)
                {
                    dt.Rows[i].Delete();
                }
            }
            dt.AcceptChanges();
            return dt;
        }

        public static void DtDeleteColumn(DataTable table, string columnName)
        {
            if (table.Columns.Contains(columnName))
            {
                table.Columns.Remove(columnName);
            }
        }

        public static void DtDeleteColumns(DataTable table, List<string> columnNames)
        {
            DataColumnCollection columns = table.Columns;

            foreach (string columnName in columnNames)
            {
                if (columns.Contains(columnName))
                {
                    table.Columns.Remove(columnName);
                }
            }
        }

        public static DataTable DtDeleteColumns(DataTable table, string[] columnNames)
        {
            DataColumnCollection columns = table.Columns;

            foreach (var columnName in columnNames)
            {
                if (columns.Contains(columnName))
                {
                    table.Columns.Remove(columnName);
                }
            }
            return table;
        }

        public static DataTable DtSlColumnAdd(DataTable dt, string replaceValue = "0")
        {
            int dtrows = dt.Rows.Count;
            int dtcols = dt.Columns.Count;

            if (!dt.Columns.Contains("SL"))
            {
                dt.Columns.Add("Sl", typeof(int)).SetOrdinal(0);
            }

            for (int m = 0; m < dtrows; m++)
            {
                dt.Rows[m]["Sl"] = m + 1;

            }
            return dt;
        }

        public static DataTable DtDateCheck(DataTable dt, string[] ColumnNames)
        {
            DataColumnCollection columns = dt.Columns;

            try
            {

                foreach (string ColumeName in ColumnNames)
                {
                    if (columns.Contains(ColumeName))
                    {
                        dt.Columns.Add(new DataColumn()
                        {
                            DefaultValue = "-",
                            ColumnName = "Temp",
                            DataType = typeof(string)
                        });

                        for (int i = 0; i < dt.Rows.Count; i++)
                            dt.Rows[i]["Temp"] = dt.Rows[i][ColumeName].ToString();

                        dt.Columns.Remove(ColumeName);
                        dt.Columns["Temp"].ColumnName = ColumeName;

                    }

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        public static DataTable DtDateFormat(DataTable dt, string[] ColumnNames)
        {
            DataColumnCollection columns = dt.Columns;
            string datetime = "";
            try
            {

                foreach (string ColumeName in ColumnNames)
                {
                    if (columns.Contains(ColumeName))
                    {
                        dt.Columns.Add(new DataColumn()
                        {
                            DefaultValue = "-",
                            ColumnName = "Temp",
                            DataType = typeof(string)
                        });

                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            try
                            {
                                datetime = dt.Rows[i][ColumeName].ToString(); ;

                                dt.Rows[i]["Temp"] = Convert.ToDateTime(datetime).ToString("yyyy-MM-dd HH:mm:ss");

                                //DateTime.ParseExact(datetime, "yyyy-MM-dd HH:mm:ss",
                                // CultureInfo.InvariantCulture).ToString("yyyy/MM/dd HH:mm:ss"); 
                            }
                            catch (Exception e)
                            {
                                throw e;
                            }
                        }

                        dt.Columns.Remove(ColumeName);
                        dt.Columns["Temp"].ColumnName = ColumeName;

                    }

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        public static DataTable DtNullCheck(DataTable dt, string[] ColumnNames, string NullValue)
        {


            DataColumnCollection columns = dt.Columns;



            try
            {


                foreach (string ColumeName in ColumnNames)
                {
                    if (columns.Contains(ColumeName))
                    {

                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            string cellValue = dt.Rows[i][ColumeName].ToString();
                            if (string.IsNullOrWhiteSpace(cellValue))
                            {
                                dt.Rows[i][ColumeName] = NullValue;
                            }
                        }


                    }



                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        public static DataTable DtDateExport(DataTable dt, string[] ColumnNames)
        {
            DataColumnCollection columns = dt.Columns;
            try
            {
                foreach (string ColumeName in ColumnNames)
                {
                    if (columns.Contains(ColumeName))
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            string cellValue = dt.Rows[i][ColumeName].ToString();
                            if (string.IsNullOrWhiteSpace(cellValue))
                            {
                                //dt.Rows[i][ColumeName] = NullValue;
                            }
                        }


                    }



                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        public static DataTable DtColumnAdd(DataTable dt, string ColumnName, string DefaultValue = "0", string Type = "numeric")
        {
            int dtrows = dt.Rows.Count;
            int dtcols = dt.Columns.Count;
            if (Type == "numeric")
            {
                dt.Columns.Add(ColumnName, typeof(int));
            }
            else if (Type == "string")
            {
                dt.Columns.Add(ColumnName, typeof(string));
            }
            for (int m = 0; m < dtrows; m++)
            {
                dt.Rows[m][ColumnName] = DefaultValue;

            }
            return dt;
        }

        public static DataTable DtColumnNameChangeList(DataTable table, List<string> oldColumnNames, List<string> newColumnNames)
        {
            DataTable resultDt = new DataTable();
            resultDt = table;

            // Iterate through each old column name
            for (int i = 0; i < oldColumnNames.Count; i++)
            {
                // Get the corresponding column index
                int columnIndex = resultDt.Columns.IndexOf(oldColumnNames[i]);

                // If the column exists, change its name to the new column name
                if (columnIndex >= 0)
                {
                    resultDt.Columns[columnIndex].ColumnName = newColumnNames[i];
                }
            }
            return resultDt;
        }

        public static string ReplaceFirstOccurrence(string Source, string Find, string Replace)
        {
            int Place = Source.IndexOf(Find);
            string result = Source.Remove(Place, Find.Length).Insert(Place, Replace);
            return result;
        }

        public static string AddSpacesToSentence(string text)
        {
            String PreString = text;
            StringBuilder SB = new StringBuilder();
            int past = 0, pre = 0;
            for (int i = 0; i < PreString.Length; i++)
            {
                var a = PreString.Substring(i, 1);
                Char b = Convert.ToChar(a);

                if (Char.IsUpper(b))
                {
                    if (pre != i - 1)
                    {
                        SB.Append(' ');
                    }
                    SB.Append(b);
                    pre = i;
                }
                else if (Char.IsNumber(b))
                {
                    if (past != i - 1)
                    {
                        SB.Append(' ');
                    }
                    SB.Append(b);
                    past = i;
                }
                else if (Char.IsLetterOrDigit(b) == false)
                {
                    SB.Append(' ');
                    SB.Append(b);
                }
                //else if (b.ToString() == ")" || b.ToString() == "(")
                //{
                //    SB.Append(' ');
                //    SB.Append(b);
                //}
                else
                {
                    SB.Append(b);
                    pre = 0;
                    past = 0;
                }
            }
            return SB.ToString();
        }

        public static void SaveExcel(DataTable data, string name, string sheetName)
        {
            string pathRoot = AppDomain.CurrentDomain.BaseDirectory;
            string fileDirectory = pathRoot + "//Excel Files";
            if (!Directory.Exists(fileDirectory))
            {
                Directory.CreateDirectory(fileDirectory);
            }

            fileDirectory += "\\" + name + "-" + DateTime.Now.ToString("yyyy-MM-dd-HHmmss") + ".xlsx";
            FileStream objFileStrm = File.Create(fileDirectory);

            using (ExcelPackage package = new ExcelPackage(objFileStrm))
            {
                ExcelWorksheet ws = package.Workbook.Worksheets.Add(sheetName);

                ws.Cells["A1"].LoadFromDataTable(data, true);
                package.Save();
                objFileStrm.Close();
            }

            //MessageBox.Show("Successfully Exported data in Excel files of root directory");
            ProcessStartInfo psi = new ProcessStartInfo(fileDirectory);
            psi.UseShellExecute = true;
            Process.Start(psi);
        }

        public static void SaveExcel_Audit(string sourceFileLocation)
        {
            string pathRoot = AppDomain.CurrentDomain.BaseDirectory;
            string fileDirectory = pathRoot + "Excel Files(Audit)";

            Directory.CreateDirectory(fileDirectory);

            string sourceFileName = Path.GetFileNameWithoutExtension(sourceFileLocation);
            string sourcefileExtension = Path.GetExtension(sourceFileLocation);

            string NewFileName = sourceFileName + "-" + DateTime.Now.ToString("yyyy-MM-dd-HHmmss") + sourcefileExtension;

            fileDirectory += "\\" + NewFileName;

            File.Copy(sourceFileLocation, fileDirectory);

        }

        public static VATReturnSubFormVM DownloadExcel(DataTable data, string name, string sheetName)
        {
            string pathRoot = AppDomain.CurrentDomain.BaseDirectory;
            string fileDirectory = pathRoot + "\\Excel Files";
            if (!Directory.Exists(fileDirectory))
            {
                Directory.CreateDirectory(fileDirectory);
            }


            string fileName = name + "-" + DateTime.Now.ToString("yyyy-MM-dd-HHmmss");

            fileDirectory += "\\" + fileName + ".xlsx";
            FileStream objFileStrm = File.Create(fileDirectory);

            ExcelPackage package = new ExcelPackage(objFileStrm);
            ExcelWorksheet ws = package.Workbook.Worksheets.Add(sheetName);

            ws.Cells["A1"].LoadFromDataTable(data, true);


            //using (ExcelPackage package = new ExcelPackage(objFileStrm))
            //{
            //    ExcelWorksheet ws = package.Workbook.Worksheets.Add(sheetName);

            //    ws.Cells["A1"].LoadFromDataTable(data, true);
            //    package.Save();
            //    objFileStrm.Close();
            //}

            //MessageBox.Show("Successfully Exported data in Excel files of root directory");
            //ProcessStartInfo psi = new ProcessStartInfo(fileDirectory);
            //psi.UseShellExecute = true;
            //Process.Start(psi);
            return new VATReturnSubFormVM { varExcelPackage = package, FileName = fileName };
        }


        public static VATReturnSubFormVM DownloadExcelMultiple(DataSet data, string name, string[] sheetNames)
        {
            string pathRoot = AppDomain.CurrentDomain.BaseDirectory;
            string fileDirectory = pathRoot + "\\Excel Files";
            if (!Directory.Exists(fileDirectory))
            {
                Directory.CreateDirectory(fileDirectory);
            }


            string fileName = name + "-" + DateTime.Now.ToString("yyyy-MM-dd-HHmmss");

            fileDirectory += "\\" + fileName + ".xlsx";
            FileStream objFileStrm = File.Create(fileDirectory);

            ExcelPackage package = new ExcelPackage(objFileStrm);

            int i = 0;
            foreach (string sheetName in sheetNames)
            {
                ExcelWorksheet ws = package.Workbook.Worksheets.Add(sheetName);
                ws.Cells["A1"].LoadFromDataTable(data.Tables[i], true);

                i++;
            }




            //using (ExcelPackage package = new ExcelPackage(objFileStrm))
            //{
            //    ExcelWorksheet ws = package.Workbook.Worksheets.Add(sheetName);

            //    ws.Cells["A1"].LoadFromDataTable(data, true);
            //    package.Save();
            //    objFileStrm.Close();
            //}

            //MessageBox.Show("Successfully Exported data in Excel files of root directory");
            //ProcessStartInfo psi = new ProcessStartInfo(fileDirectory);
            //psi.UseShellExecute = true;
            //Process.Start(psi);
            return new VATReturnSubFormVM { varExcelPackage = package, FileName = fileName };
        }

        public static void SaveXML(DataSet data, string name)
        {
            string pathRoot = AppDomain.CurrentDomain.BaseDirectory;
            string fileDirectory = pathRoot + "//XML Files";
            if (!Directory.Exists(fileDirectory))
            {
                Directory.CreateDirectory(fileDirectory);
            }

            fileDirectory += "\\" + name + "-" + DateTime.Now.ToString("yyyy-MM-dd-HHmmss") + ".xml";

            //var xml = data.GetXml();

            using (StreamWriter fs = new StreamWriter(fileDirectory)) // XML File Path
            {
                data.WriteXml(fs);
            }

            ProcessStartInfo psi = new ProcessStartInfo(fileDirectory);
            psi.UseShellExecute = true;
            Process.Start(psi);
        }

        public static void SaveJson(DataSet data, string name)
        {
            string pathRoot = AppDomain.CurrentDomain.BaseDirectory;
            string fileDirectory = pathRoot + "//Json Files";
            if (!Directory.Exists(fileDirectory))
            {
                Directory.CreateDirectory(fileDirectory);
            }

            fileDirectory += "\\" + name + "-" + DateTime.Now.ToString("yyyy-MM-dd-HHmmss") + ".json";

            string json = JsonConvert.SerializeObject(data, Newtonsoft.Json.Formatting.Indented);

            string filePath = fileDirectory;

            File.WriteAllText(filePath, json);

            ProcessStartInfo psi = new ProcessStartInfo(fileDirectory);
            psi.UseShellExecute = true;
            Process.Start(psi);
        }

        public static void SaveTextFile(string xml, string name, string extention = ".txt")
        {
            string pathRoot = AppDomain.CurrentDomain.BaseDirectory;
            string fileDirectory = pathRoot + "//Files";
            if (!Directory.Exists(fileDirectory))
            {
                Directory.CreateDirectory(fileDirectory);
            }

            fileDirectory += "\\" + name + "-" + DateTime.Now.ToString("yyyy-MM-dd-HHmmss") + extention;

            using (StreamWriter fs = new StreamWriter(fileDirectory)) // XML File Path
            {
                fs.WriteLine(xml);
            }

            ProcessStartInfo psi = new ProcessStartInfo(fileDirectory);
            psi.UseShellExecute = true;
            Process.Start(psi);
        }

        public static void SaveIVASJsonTextFile(string xml, string name, string extention = ".txt")
        {
            string pathRoot = AppDomain.CurrentDomain.BaseDirectory;
            string fileDirectory = pathRoot + "//Files//IVASJsonFile";
            if (!Directory.Exists(fileDirectory))
            {
                Directory.CreateDirectory(fileDirectory);
            }

            fileDirectory += "\\" + name + "-" + DateTime.Now.ToString("yyyy-MM-dd-HHmmss") + extention;

            using (StreamWriter fs = new StreamWriter(fileDirectory)) // XML File Path
            {
                fs.WriteLine(xml);
            }

            ProcessStartInfo psi = new ProcessStartInfo(fileDirectory);
            psi.UseShellExecute = true;
            //////Process.Start(psi);
        }

        public static DataSet GetDataSetFromXML(string xml)
        {
            var dataSet = new DataSet();
            StringReader reader = new StringReader(xml);

            dataSet.ReadXml(reader);

            return dataSet;
        }
        public static DataTable ConvertCSVtoDataTable(string strFilePath)
        {
            try
            {


                StreamReader sr = new StreamReader(strFilePath);
                string[] headers = sr.ReadLine().Split(',');
                DataTable dt = new DataTable();
                foreach (string header in headers)
                {
                    dt.Columns.Add(header);
                }
                while (!sr.EndOfStream)
                {
                    string[] rows = Regex.Split(sr.ReadLine(), ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");
                    DataRow dr = dt.NewRow();
                    for (int i = 0; i < headers.Length; i++)
                    {
                        dr[i] = rows[i];
                    }
                    dt.Rows.Add(dr);
                }

                sr.Close();
                return dt;
            }
            catch (Exception e)
            {

                throw e;
            }
        }
        public static string GetXMLFromDataSet(DataSet dataSet)
        {
            var memoryStream = new MemoryStream();

            dataSet.WriteXml(memoryStream);

            memoryStream.Seek(0, SeekOrigin.Begin);

            var sr = new StreamReader(memoryStream);

            var resultXML = sr.ReadToEnd();

            return resultXML;
        }

        public static void SaveExcelMultiple(DataSet dataSet, string name, string[] sheetNames, string[] reportHeader = null)
        {
            string pathRoot = AppDomain.CurrentDomain.BaseDirectory;
            string fileDirectory = pathRoot + "//Excel Files";
            int ColumnCount = 0;
            int RowCount = 0;
            int reportBorder = reportHeader == null ? 0 : 1;


            var format = new OfficeOpenXml.ExcelTextFormat();
            format.Delimiter = '~';
            format.TextQualifier = '"';
            format.DataTypes = new[] { eDataTypes.String };
            int reportHeaderLength = 1;
            if (!Directory.Exists(fileDirectory))
            {
                Directory.CreateDirectory(fileDirectory);
            }

            fileDirectory += "\\" + name + "-" + DateTime.Now.ToString("yyyy-MM-dd-HHmmss") + ".xlsx";
            FileStream objFileStrm = File.Create(fileDirectory);
            if (reportHeader != null)
            {
                reportHeaderLength = reportHeader.Length + 2;
            }
            using (ExcelPackage package = new ExcelPackage(objFileStrm))
            {


                for (int j = 0; j < sheetNames.Length; j++)
                {
                    ExcelWorksheet ws = package.Workbook.Worksheets.Add(sheetNames[j]);
                    ColumnCount = dataSet.Tables[j].Columns.Count;
                    RowCount = dataSet.Tables[j].Rows.Count;
                    if (reportHeader != null)
                    {
                        for (int i = 0; i < reportHeader.Length; i++)
                        {
                            ws.Cells[i + 1, 1, (i + 1), ColumnCount].Merge = true;
                            ws.Cells[i + 1, 1, (i + 1), ColumnCount].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            ws.Cells[i + 1, 1, (i + 1), ColumnCount].Style.Font.Size = 14 - i;
                            ws.Cells[i + 1, 1].LoadFromText(reportHeader[i], format);

                        }
                    }
                    int colNumber = 0;
                    foreach (DataColumn col in dataSet.Tables[j].Columns)
                    {
                        colNumber++;
                        if (col.DataType == typeof(DateTime))
                        {
                            ws.Column(colNumber).Style.Numberformat.Format = "dd-MMM-yyyy";
                        }
                        else if (col.DataType == typeof(Decimal))
                        {
                            ws.Column(colNumber).Style.Numberformat.Format = "#,##0.00_);[Red](#,##0.00)";
                        }

                    }

                    ws.Cells["A" + reportHeaderLength].LoadFromDataTable(dataSet.Tables[j], true);

                    if (reportHeader != null)
                    {
                        var alp = OrdinaryVATDesktop.Alphabet[(ColumnCount - 1)];
                        ws.Cells["A" + (reportHeaderLength) + ":" + alp + (reportHeaderLength + RowCount + reportBorder)].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        ws.Cells["A" + (reportHeaderLength) + ":" + OrdinaryVATDesktop.Alphabet[(ColumnCount)] + (reportHeaderLength + RowCount)].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    }

                }


                package.Save();
                objFileStrm.Close();
            }

            //MessageBox.Show("Successfully Exported data in Excel files of root directory");
            ProcessStartInfo psi = new ProcessStartInfo(fileDirectory);
            psi.UseShellExecute = true;
            Process.Start(psi);
        }

        public static void WriteDataToFile(DataTable table, string name)
        {
            int i;
            StreamWriter sw;

            string pathRoot = AppDomain.CurrentDomain.BaseDirectory;
            string path = pathRoot + "//TEXT Files";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            path += "\\" + name + "-" + DateTime.Now.ToString("HHmmss") + ".txt";

            if (!File.Exists(path))
            {
                var stream = File.Create(path);
                stream.Close();
            }


            sw = new StreamWriter(path, false);

            //for (i = 0; i < table.Columns.Count - 1; i++)
            //{

            //    sw.Write(table.Columns[i].ColumnName + ";");

            //}
            //sw.Write(table.Columns[i].ColumnName);
            //sw.WriteLine();

            var len = table.Rows.Count;
            var count = 1;
            foreach (DataRow row in table.Rows)
            {
                object[] array = row.ItemArray;

                for (i = 0; i < array.Length - 1; i++)
                {
                    sw.Write(array[i].ToString().Replace("\n", "").Trim() + "|");

                }
                sw.Write(array[i].ToString().Trim());

                if (count != len)
                {
                    sw.WriteLine();
                }
                count++;
            }

            sw.Close();

            ProcessStartInfo psi = new ProcessStartInfo(path) { UseShellExecute = true };
            Process.Start(psi);
        }
        public static string NumberToWords(int number)
        {
            string words = string.Empty;
            try
            {

                if (number == 0)
                    return "zero";

                if (number < 0)
                    return "minus " + NumberToWords(Math.Abs(number));

                words = "";

                if ((number / 1000000) > 0)
                {
                    words += NumberToWords(number / 1000000) + " million ";
                    number %= 1000000;
                }

                if ((number / 1000) > 0)
                {
                    words += NumberToWords(number / 1000) + " thousand ";
                    number %= 1000;
                }

                if ((number / 100) > 0)
                {
                    words += NumberToWords(number / 100) + " hundred ";
                    number %= 100;
                }

                if (number > 0)
                {
                    if (words != "")
                        words += "and ";

                    var unitsMap = new[]
                                       {
                                           "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine",
                                           "ten", "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen",
                                           "seventeen", "eighteen", "nineteen"
                                       };
                    var tensMap = new[]
                                      {
                                          "zero", "ten", "twenty", "thirty", "forty", "fifty", "sixty", "seventy", "eighty"
                                          , "ninety"
                                      };

                    if (number < 20)
                        words += unitsMap[number];
                    else
                    {
                        words += tensMap[number / 10];
                        if ((number % 10) > 0)
                            words += "-" + unitsMap[number % 10];
                    }
                }
            }
            #region Catch

            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
            }
            #endregion Catch
            return words;
        }
        public static DataTable FormatSaleData(DataTable table)
        {
            #region Columns

            ////table.Columns.Remove("FKDAT");
            ////table.Columns.Remove("ERZET");
            ////table.Columns.Remove("LASNO");
            ////table.Columns.Remove("WAERK");

            Dictionary<string, string> SaleColumns = new Dictionary<string, string>()
            {
                {"ID".ToLower(),"ID"},
                {"Branch_Code".ToLower(),"Branch_Code"},
                {"BranchCode".ToLower(),"Branch_Code"},
                {"CustomerGroup".ToLower(),"CustomerGroup"},
                {"Customer_Name".ToLower(),"Customer_Name"},
                {"CustomerName".ToLower(),"Customer_Name"},
                {"Customer_Code".ToLower(),"Customer_Code"},
                {"CustomerCode".ToLower(),"Customer_Code"},
                {"Delivery_Address".ToLower(),"Delivery_Address"},
                {"DeliveryAddress".ToLower(),"Delivery_Address"},
                {"Invoice_Date_Time".ToLower(),"Invoice_Date_Time"},
                {"InvoiceDateTime".ToLower(),"Invoice_Date_Time"},
                {"Delivery_Date_Time".ToLower(),"Delivery_Date_Time"},
                {"DeliveryDateTime".ToLower(),"Delivery_Date_Time"},
                {"Reference_No".ToLower(),"Reference_No"},
                {"ReferenceNo".ToLower(),"Reference_No"},
                {"Comments".ToLower(),"Comments"},
                {"Sale_Type".ToLower(),"Sale_Type"},
                {"Is_Print".ToLower(),"Is_Print"},
                {"Tender_Id".ToLower(),"Tender_Id"},
                {"Post".ToLower(),"Post"},
                {"LC_Number".ToLower(),"LC_Number"},
                {"LCNumber".ToLower(),"LC_Number"},
                {"Currency_Code".ToLower(),"Currency_Code"},
                {"CurrencyCode".ToLower(),"Currency_Code"},
                {"CommentsD".ToLower(),"CommentsD"},
                {"LineComments".ToLower(),"CommentsD"},
                {"Item_Code".ToLower(),"Item_Code"},
                {"ProductCode".ToLower(),"Item_Code"},
                {"Quantity".ToLower(),"Quantity"},
                {"Item_Name".ToLower(),"Item_Name"},
                {"ProductName".ToLower(),"Item_Name"},
                {"NBR_Price".ToLower(),"NBR_Price"},
                {"UnitPrice".ToLower(),"NBR_Price"},
                {"UOM".ToLower(),"UOM"},
                {"VAT_Rate".ToLower(),"VAT_Rate"},
                {"VATRate".ToLower(),"VAT_Rate"},
                {"SD_Rate".ToLower(),"SD_Rate"},
                {"SDRate".ToLower(),"SD_Rate"},
                {"Non_Stock".ToLower(),"Non_Stock"},
                {"Type".ToLower(),"Type"},
                {"Discount_Amount".ToLower(),"Discount_Amount"},
                {"DiscountAmount".ToLower(),"Discount_Amount"},
                {"Promotional_Quantity".ToLower(),"Promotional_Quantity"},
                {"PromotionalQuantity".ToLower(),"Promotional_Quantity"},
                {"Promotion_Quantity".ToLower(),"Promotional_Quantity"},
                {"VAT_Name".ToLower(),"VAT_Name"},
                {"SubTotal".ToLower(),"SubTotal"},
                {"ExpDescription".ToLower(),"ExpDescription"},
                {"ExpQuantity".ToLower(),"ExpQuantity"},
                {"ExpGrossWeight".ToLower(),"ExpGrossWeight"},
                {"ExpNetWeight".ToLower(),"ExpNetWeight"},
                {"ExpNumberFrom".ToLower(),"ExpNumberFrom"},
                {"ExpNumberTo".ToLower(),"ExpNumberTo"},
                {"SDAmount".ToLower(),"SDAmount"},
                {"VAT_Amount".ToLower(),"VAT_Amount"},
                {"trading_markup".ToLower(),"Trading_MarkUp"},
                {"ReturnId".ToLower(),"ReturnId"},
                {"Second_Unit".ToLower(),"Weight"},
                {"transactiontype".ToLower(),"TransactionType"},
                {"Vehicle_No".ToLower(),"Vehicle_No"},
                {"VehicleNo".ToLower(),"Vehicle_No"},
                {"Vehicle_Type".ToLower(),"VehicleType"},

                {"PreviousInvoiceNo".ToLower(),"Previous_Invoice_No"},
                {"Previous_Invoice_No".ToLower(),"Previous_Invoice_No"},
                {"PreviousNBRPrice".ToLower(),"PreviousNBRPrice"},
                {"PreviousUnitPrice".ToLower(),"PreviousNBRPrice"},
                {"PreviousQuantity".ToLower(),"PreviousQuantity"},
                {"PreviousSubTotal".ToLower(),"PreviousSubTotal"},
                {"PreviousSD".ToLower(),"PreviousSD"},
                {"PreviousSuppDuty".ToLower(),"PreviousSD"},
                {"PreviousSDAmount".ToLower(),"PreviousSDAmount"},
                {"PreviousVATRate".ToLower(),"PreviousVATRate"},
                {"PreviousVATAmount".ToLower(),"PreviousVATAmount"},
                {"PreviousUOM".ToLower(),"PreviousUOM"},
                {"PreviousUnitOfMeasuremnet".ToLower(),"PreviousUOM"},
                {"ReasonOfReturn".ToLower(),"ReasonOfReturn"},
                {"PreviousInvoiceDateTime".ToLower(),"PreviousInvoiceDateTime"},
                {"CustomerBIN".ToLower(),"CustomerBIN"},
                {"CustomerAddress".ToLower(),"CustomerAddress"},
                {"UserName".ToLower(),"UserName"},
                {"FKDAT".ToLower(),"FKDAT"},
                {"ERZET".ToLower(),"ERZET"},
                {"LASNO".ToLower(),"LASNO"},
                {"WAERK".ToLower(),"WAERK"},
                {"PRODUCTDESCRIPTION".ToLower(),"PRODUCTDESCRIPTION"},
                {"OtherRef".ToLower(),"OtherRef"},
                {"vehicletype".ToLower(),"VehicleType"},
                {"Option1".ToLower(),"Option2"},
                {"Option2".ToLower(),"Option3"},

                {"FileID".ToLower(),"Option3"},
                {"IsProcessed".ToLower(),"IsProcessed"},
                {"CompanyCode".ToLower(),"CompanyCode"},
                
            };

            #endregion
            try
            {
                var finalTable = new DataTable();

                foreach (DataColumn col in table.Columns)
                {
                    var clName = col.ColumnName.ToLower();

                    if (!SaleColumns.Keys.Contains(clName))
                        continue;

                    clName = SaleColumns[clName];

                    //table.Columns[col.ColumnName].ColumnName = clName;

                    finalTable.Columns.Add(clName);

                }

                foreach (DataRow row in table.Rows)
                {
                    var data = row.ItemArray;
                    finalTable.Rows.Add(data);
                }


                if (!finalTable.Columns.Contains("Invoice_Date_Time"))
                {
                    var columnName = new DataColumn("Invoice_Date_Time") { DefaultValue = "" };
                    finalTable.Columns.Add(columnName);
                }
                if (!finalTable.Columns.Contains("PreviousInvoiceDateTime"))
                {
                    var columnName = new DataColumn("PreviousInvoiceDateTime") { DefaultValue = "" };
                    finalTable.Columns.Add(columnName);
                }
                foreach (DataRow rows in finalTable.Rows)
                {
                    if (finalTable.Columns.Contains("FKDAT"))
                    {
                        if (finalTable.Columns.Contains("PreviousInvoiceDateTime"))
                        {
                            string pattern = "dd.MM.yyyy HH:mm:ss";
                            DateTime parsedDate;
                            var a = rows["PreviousInvoiceDateTime"].ToString();
                            DateTime.TryParseExact(a, pattern, null, DateTimeStyles.None, out parsedDate);
                            rows["PreviousInvoiceDateTime"] = parsedDate.ToString("yyyy-MM-dd HH:mm:ss");
                        }
                        if (rows["PreviousInvoiceDateTime"].ToString() == "0001-01-01 00:00:00")
                        {
                            rows["PreviousInvoiceDateTime"] = "1900-01-01 00:00:00";
                        }
                    }

                    if (finalTable.Columns.Contains("PreviousInvoiceDateTime"))
                    {
                        if (rows["PreviousInvoiceDateTime"].ToString() == "")
                        {
                            rows["PreviousInvoiceDateTime"] = "1900-01-01 00:00:00";
                        }
                    }

                    if (finalTable.Columns.Contains("Sale_Type"))
                    {
                        if (rows["Sale_Type"].ToString() == "ZBFD")
                        {
                            rows["Sale_Type"] = "New";
                            rows["TransactionType"] = "Other";
                        }
                        else if (rows["Sale_Type"].ToString() == "ZBRE")
                        {
                            rows["Sale_Type"] = "Credit";
                            rows["TransactionType"] = "Credit";
                        }
                    }

                    if (finalTable.Columns.Contains("FKDAT") && finalTable.Columns.Contains("ERZET"))
                    {
                        string pattern = "dd.MM.yyyy HHmmss";
                        DateTime parsedDate;
                        var a = rows["FKDAT"].ToString() + " " + rows["ERZET"].ToString();
                        DateTime.TryParseExact(a, pattern, null, DateTimeStyles.None, out parsedDate);
                        rows["Invoice_Date_Time"] = parsedDate.ToString("yyyy-MM-dd HH:mm:ss");
                    }
                }

                return finalTable;
            }
            catch (Exception e)
            {
                throw e;
            }
        }


        public static DataSet ChangeName(DataSet set, string name)
        {
            try
            {
                set.DataSetName = name;


                int tablesCount = set.Tables.Count;

                for (var index = 0; index < tablesCount; index++)
                {
                    DataTable table = set.Tables[index];
                    table.TableName = name + "-" + index;
                }

                return set;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static TList GetListFromDataTable<TList>(DataTable table)
            where TList : class, new()
        {
            string json = JsonConvert.SerializeObject(table);

            var list = JsonConvert.DeserializeObject<TList>(json);

            return list;
        }


        public static TI GetObject<TDAL, TRepo, TI>(string flag)
            where TDAL : class, TI, new()
            where TRepo : class, TI, new()
        {
            if (flag.ToLower() == "y")
            {
                return new TRepo();
            }
            else
            {
                return new TDAL();

            }
        }



        public static bool IsNumber(string s)
        {
            bool value = true;
            if (string.IsNullOrEmpty(s))
            {
                value = false;
            }
            else
            {
                foreach (char c in s)
                {
                    value = value && char.IsDigit(c);
                }
            }
            return value;
        }


        public static DataTable CopyTableInString(DataTable table)
        {
            try
            {
                DataTable tempTable = new DataTable();

                foreach (DataColumn column in table.Columns)
                {
                    var ColumnName = column.ColumnName;
                    tempTable.Columns.Add(ColumnName, typeof(string));
                }

                int i = 0;
                foreach (DataRow row in table.Rows)
                {
                    tempTable.Rows.Add(tempTable.NewRow());

                    foreach (DataColumn cl in table.Columns)
                    {
                        tempTable.Rows[i][cl.ColumnName] = row[cl.ColumnName];
                    }

                    i++;
                }

                return tempTable;
            }
            catch (Exception e)
            {
                return new DataTable();
            }
        }


        public static string PrintXML(string xml)
        {
            string result = "";

            MemoryStream mStream = new MemoryStream();
            XmlTextWriter writer = new XmlTextWriter(mStream, Encoding.Unicode);
            XmlDocument document = new XmlDocument();

            try
            {
                // Load the XmlDocument with the XML.
                document.LoadXml(xml);

                writer.Formatting = System.Xml.Formatting.Indented;

                // Write the XML into a formatting XmlTextWriter
                document.WriteContentTo(writer);
                writer.Flush();
                mStream.Flush();

                // Have to rewind the MemoryStream in order to read
                // its contents.
                mStream.Position = 0;

                // Read MemoryStream contents into a StreamReader.
                StreamReader sReader = new StreamReader(mStream);

                // Extract the text from the StreamReader.
                string formattedXml = sReader.ReadToEnd();

                result = formattedXml;
            }
            catch (Exception e)
            {
                return "";
            }
            finally
            {
                mStream.Close();
                writer.Close();
            }



            return result;
        }

        public static MemoryStream CopyInputStreamToOutput(Stream fromStream, MemoryStream destination)
        {
            MemoryStream ms = new MemoryStream();
            fromStream.Position = 0;

            fromStream.CopyTo(ms);

            byte[] rv = new byte[destination.GetBuffer().Length + ms.GetBuffer().Length];
            System.Buffer.BlockCopy(destination.GetBuffer(), 0, rv, 0, destination.GetBuffer().Length);
            System.Buffer.BlockCopy(ms.GetBuffer(), 0, rv, destination.GetBuffer().Length, ms.GetBuffer().Length);
            //System.Buffer.BlockCopy(a3, 0, rv, a1.Length + a2.Length, a3.Length);



            return new MemoryStream(rv, 0, rv.Length, true, true);
        }

        public static IEnumerable<DateTime> EachDay(DateTime from, DateTime thru)
        {
            for (var day = from.Date; day.Date <= thru.Date; day = day.AddDays(1))
                yield return day;
        }


        public static string ApplyConditions(string sqlText, string[] conditionalFields, string[] conditionalValue, bool orOperator = false)
        {
            string cField = "";
            bool conditionFlag = true;

            var checkValueExist = conditionalValue != null && conditionalValue.ToList().All(x => !string.IsNullOrEmpty(x));
            var checkConditionalValue = conditionalValue != null && conditionalValue.ToList().All(x => !string.IsNullOrEmpty(x));



            if (checkValueExist && orOperator && checkConditionalValue)
            {
                sqlText += " and (";
            }


            if (conditionalFields != null && conditionalValue != null && conditionalFields.Length == conditionalValue.Length)
            {
                for (int i = 0; i < conditionalFields.Length; i++)
                {
                    if (string.IsNullOrWhiteSpace(conditionalFields[i]) || string.IsNullOrWhiteSpace(conditionalValue[i]))
                    {
                        continue;
                    }
                    cField = conditionalFields[i].ToString();
                    cField = StringReplacing(cField);
                    string operand = " AND ";

                    if (orOperator)
                    {
                        operand = " OR ";

                        if (conditionFlag)
                        {
                            operand = "  ";
                            conditionFlag = false;
                        }
                    }


                    if (conditionalFields[i].ToLower().Contains("like"))
                    {
                        sqlText += operand + conditionalFields[i] + " '%'+ " + " @" + cField.Replace("like", "").Trim() + " +'%'";
                    }
                    else if (conditionalFields[i].Contains(">") || conditionalFields[i].Contains("<"))
                    {
                        sqlText += operand + conditionalFields[i] + " @" + cField;

                    }
                    else if (conditionalFields[i].Contains("in"))
                    {

                        //  such as invoice then work it , to avoid this type 

                        var test = conditionalFields[i]
                            .Split(new string[] { " in" }, StringSplitOptions.RemoveEmptyEntries);

                        if (test.Length > 1)
                        {
                            sqlText += operand + conditionalFields[i] + "(" + conditionalValue[i] + ")";
                        }
                        else
                        {
                            sqlText += operand + conditionalFields[i] + "= '" + Convert.ToString(conditionalValue[i]) + "'";
                        }

                    }
                    else
                    {
                        sqlText += operand + conditionalFields[i] + "= @" + cField;
                    }
                }
            }

            if (checkValueExist && orOperator && checkConditionalValue)
            {
                sqlText += " )";
            }

            return sqlText;
        }


        public static SqlCommand ApplyParameters(SqlCommand objComm, string[] conditionalFields, string[] conditionalValue)
        {
            string cField = "";
            string tst = "";
            if (conditionalFields != null && conditionalValue != null && conditionalFields.Length == conditionalValue.Length)
            {
                for (int j = 0; j < conditionalFields.Length; j++)
                {
                    if (string.IsNullOrWhiteSpace(conditionalFields[j]) || string.IsNullOrWhiteSpace(conditionalValue[j]))
                    {
                        continue;
                    }
                    cField = conditionalFields[j].ToString();
                    cField = StringReplacing(cField);
                    var test = conditionalFields[j].ToLower().Contains("in");


                    if (conditionalFields[j].ToLower().Contains("like"))
                    {
                        objComm.Parameters.AddWithValue("@" + cField.Replace("like", "").Trim(), conditionalValue[j]);
                    }
                    else if (conditionalFields[j].ToLower().Contains("in") || conditionalFields[j].ToLower().Contains("IN"))
                    {
                    }
                    else
                    {
                        objComm.Parameters.AddWithValue("@" + cField, conditionalValue[j]);
                    }
                }
            }

            return objComm;
        }
        public static object TransferData(object source, object destination)
        {
            var sourceProperties = source.GetType().GetProperties();
            var destinationProperties = destination.GetType().GetProperties();

            foreach (var sourceProperty in sourceProperties)
            {
                var destinationProperty = destinationProperties.FirstOrDefault(p => p.Name.ToLower() == sourceProperty.Name.ToLower());
                if (destinationProperty != null && destinationProperty.PropertyType == sourceProperty.PropertyType && destinationProperty.CanWrite)
                {
                    var value = sourceProperty.GetValue(source);
                    destinationProperty.SetValue(destination, value);
                }
            }
            return destination;

        }



    }


    //public static class Converter
    //{
    //    public static string DESDecrypt(string passPhrase, string IV, string dataToDeccrypt);
    //    public static string DESEncrypt(string passPhrase, string IV, string dataToEncrypt);
    //}

    public static class DBConstant
    {
        public static string PassPhrase = "20120220";
        public static string EnKey = "12345678";
        public const string LineDelimeter = "^";
        public const string FieldDelimeter = "~";
        public const string DBName = "VAT";

        public const string CompanyName = "Symphony Softtech Ltd.";
        public const string CompanyAddress = "Concord Archidea, Block B, 4th Floor, Road 4, Dhanmondi R/A, Dhaka-1205.";
        public const string CompanyContactNumber = "Tel: +88(02)9119812, +88(02)8151714, Fax +88(02)9104352";
        public static bool IsProjectVersion2012 = true;

    }


    public static class ListExtension
    {
        public static List<TModel> ToList<TModel>(this DataTable table) where TModel : class, new()
        {
            var modelList = new List<TModel>();


            foreach (DataRow row in table.Rows)
            {
                var model = new TModel();
                var type = model.GetType();

                foreach (DataColumn column in table.Columns)
                {
                    if (type.GetProperty(column.ColumnName) != null)
                    {
                        type.GetProperty(column.ColumnName).SetValue(model, row[column.ColumnName].ToString());

                    }

                }

                modelList.Add(model);
            }



            return modelList;
        }

        public static DataTable ToDataTable<T>(this List<T> source)
        {
            return JsonConvert.DeserializeObject<DataTable>(JsonConvert.SerializeObject(source));
        }
    }

    public class StreamReadWriteToString
    {
        private Stream ioStream;
        private UnicodeEncoding streamEncoding;

        public StreamReadWriteToString(Stream ioStream)
        {
            this.ioStream = ioStream;
            this.streamEncoding = new UnicodeEncoding();
        }

        public string ReadString()
        {
            try
            {
                int len = 0;
                len = ioStream.ReadByte() * 256;
                len += ioStream.ReadByte();
                byte[] inBuffer = new byte[len];
                ioStream.Read(inBuffer, 0, len);
                return streamEncoding.GetString(inBuffer);
            }
            catch (Exception ex)
            {
                // throw ex;
                ;
            }

            return "";
        }

        public int WriteString(string outString)
        {
            byte[] outBuffer = streamEncoding.GetBytes(outString);
            int len = outBuffer.Length;
            if (len > UInt16.MaxValue)
            {
                len = (int)UInt16.MaxValue;
            }

            ioStream.WriteByte((byte)(len / 256));
            ioStream.WriteByte((byte)(len & 255));
            ioStream.Write(outBuffer, 0, len);
            ioStream.Flush();

            return outBuffer.Length + 2;
        }
    }

    

}
