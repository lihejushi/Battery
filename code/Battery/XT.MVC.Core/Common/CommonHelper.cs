using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Net;
using System.IO;
using System.Data;
using XT.MVC.Core.Infrastructure;
using XT.MVC.Core.Logging;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace XT.MVC.Core.Common
{
    public partial class CommonHelper
    {
        /// <summary>
        /// �������������
        /// </summary>
        /// <param name="length">����</param>
        /// <returns></returns>
        public static string GenerateRandomDigitCode(int length)
        {
            var random = new Random();
            string str = string.Empty;
            for (int i = 0; i < length; i++)
                str = String.Concat(str, random.Next(10).ToString());
            return str;
        }

        /// <summary>
        /// ����ָ����Χ�ڵ������
        /// </summary>
        /// <param name="min">��Сֵ</param>
        /// <param name="max">���ֵ</param>
        /// <returns></returns>
        public static int GenerateRandomInteger(int min = 0, int max = int.MaxValue)
        {
            var randomNumberBuffer = new byte[10];
            new RNGCryptoServiceProvider().GetBytes(randomNumberBuffer);
            return new Random(BitConverter.ToInt32(randomNumberBuffer, 0)).Next(min, max);
        }

        private static AspNetHostingPermissionLevel? _trustLevel = null;
        /// <summary>
        /// �鿴��ǰ���г�������εȼ�(http://blogs.msdn.com/dmitryr/archive/2007/01/23/finding-out-the-current-trust-level-in-asp-net.aspx)
        /// </summary>
        /// <returns></returns>
        public static AspNetHostingPermissionLevel GetTrustLevel()
        {
            if (!_trustLevel.HasValue)
            {
                //������С�ȼ�
                _trustLevel = AspNetHostingPermissionLevel.None;

                //ȷ�����ȼ�
                foreach (AspNetHostingPermissionLevel trustLevel in
                        new AspNetHostingPermissionLevel[] {
                                AspNetHostingPermissionLevel.Unrestricted,
                                AspNetHostingPermissionLevel.High,
                                AspNetHostingPermissionLevel.Medium,
                                AspNetHostingPermissionLevel.Low,
                                AspNetHostingPermissionLevel.Minimal 
                            })
                {
                    try
                    {
                        new AspNetHostingPermission(trustLevel).Demand();
                        _trustLevel = trustLevel;
                        break; //we've set the highest permission we can
                    }
                    catch (System.Security.SecurityException)
                    {
                        continue;
                    }
                }
            }
            return _trustLevel.Value;
        }

        /// <summary>
        /// ���������е�ֵ
        /// </summary>
        /// <param name="instance">��Ҫ���õ�ʵ��</param>
        /// <param name="propertyName">��������</param>
        /// <param name="value">����ֵ</param>
        public static void SetProperty(object instance, string propertyName, object value)
        {
            if (instance == null) throw new ArgumentNullException("instance");
            if (propertyName == null) throw new ArgumentNullException("propertyName");

            Type instanceType = instance.GetType();
            PropertyInfo pi = instanceType.GetProperty(propertyName);
            if (pi == null)
                throw new Exception(string.Format("No property '{0}' found on the instance of type '{1}'.", propertyName, instanceType));
            if (!pi.CanWrite)
                throw new Exception(string.Format("The property '{0}' on the instance of type '{1}' does not have a setter.", propertyName, instanceType));
            if (value != null && !value.GetType().IsAssignableFrom(pi.PropertyType))
                value = TypeHelper.To(value, pi.PropertyType);
            pi.SetValue(instance, value, new object[0]);
        }

        //��������
        private static string[] _weekdays = { "������", "����һ", "���ڶ�", "������", "������", "������", "������" };
        //�ո񡢻س������з����Ʊ��������ʽ
        private static Regex _tbbrRegex = new Regex(@"\s*|\t|\r|\n", RegexOptions.IgnoreCase);

        #region ʱ�����

        /// <summary>
        /// ��õ�ǰʱ���""yyyy-MM-dd HH:mm:ss:fffffff""��ʽ�ַ���
        /// </summary>
        public static string GetDateTimeMS()
        {
            return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fffffff");
        }

        /// <summary>
        /// ��õ�ǰʱ���""yyyy��MM��dd�� HH:mm:ss""��ʽ�ַ���
        /// </summary>
        public static string GetDateTimeU()
        {
            return string.Format("{0:U}", DateTime.Now);
        }

        /// <summary>
        /// ��õ�ǰʱ���""yyyy-MM-dd HH:mm:ss""��ʽ�ַ���
        /// </summary>
        public static string GetDateTime()
        {
            return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }

        /// <summary>
        /// ��õ�ǰ����
        /// </summary>
        public static string GetDate()
        {
            return DateTime.Now.ToString("yyyy-MM-dd");
        }

        /// <summary>
        /// ������ĵ�ǰ����
        /// </summary>
        public static string GetChineseDate()
        {
            return DateTime.Now.ToString("yyyy��MM��dd");
        }

        /// <summary>
        /// ��õ�ǰʱ��(�������ڲ���)
        /// </summary>
        public static string GetTime()
        {
            return DateTime.Now.ToString("HH:mm:ss");
        }

        /// <summary>
        /// ��õ�ǰСʱ
        /// </summary>
        public static string GetHour()
        {
            return DateTime.Now.Hour.ToString("00");
        }

        /// <summary>
        /// ��õ�ǰ��
        /// </summary>
        public static string GetDay()
        {
            return DateTime.Now.Day.ToString("00");
        }

        /// <summary>
        /// ��õ�ǰ��
        /// </summary>
        public static string GetMonth()
        {
            return DateTime.Now.Month.ToString("00");
        }

        /// <summary>
        /// ��õ�ǰ��
        /// </summary>
        public static string GetYear()
        {
            return DateTime.Now.Year.ToString();
        }

        /// <summary>
        /// ��õ�ǰ����(����)
        /// </summary>
        public static string GetDayOfWeek()
        {
            return ((int)DateTime.Now.DayOfWeek).ToString();
        }

        /// <summary>
        /// ��õ�ǰ����(����)
        /// </summary>
        public static string GetWeek()
        {
            return _weekdays[(int)DateTime.Now.DayOfWeek];
        }

        #endregion

        #region �������

        /// <summary>
        /// ����ַ������ַ��������е�λ��
        /// </summary>
        public static int GetIndexInArray(string s, string[] array, bool ignoreCase)
        {
            if (string.IsNullOrEmpty(s) || array == null || array.Length == 0)
                return -1;

            int index = 0;
            string temp = null;

            if (ignoreCase)
                s = s.ToLower();

            foreach (string item in array)
            {
                if (ignoreCase)
                    temp = item.ToLower();
                else
                    temp = item;

                if (s == temp)
                    return index;
                else
                    index++;
            }

            return -1;
        }

        /// <summary>
        /// ����ַ������ַ��������е�λ��
        /// </summary>
        public static int GetIndexInArray(string s, string[] array)
        {
            return GetIndexInArray(s, array, false);
        }

        /// <summary>
        /// �ж��ַ����Ƿ����ַ���������
        /// </summary>
        public static bool IsInArray(string s, string[] array, bool ignoreCase)
        {
            return GetIndexInArray(s, array, ignoreCase) > -1;
        }

        /// <summary>
        /// �ж��ַ����Ƿ����ַ���������
        /// </summary>
        public static bool IsInArray(string s, string[] array)
        {
            return IsInArray(s, array, false);
        }

        /// <summary>
        /// �ж��ַ����Ƿ����ַ�����
        /// </summary>
        public static bool IsInArray(string s, string array, string splitStr, bool ignoreCase)
        {
            return IsInArray(s, StringHelper.SplitString(array, splitStr), ignoreCase);
        }

        /// <summary>
        /// �ж��ַ����Ƿ����ַ�����
        /// </summary>
        public static bool IsInArray(string s, string array, string splitStr)
        {
            return IsInArray(s, StringHelper.SplitString(array, splitStr), false);
        }

        /// <summary>
        /// �ж��ַ����Ƿ����ַ�����
        /// </summary>
        public static bool IsInArray(string s, string array)
        {
            return IsInArray(s, StringHelper.SplitString(array, ","), false);
        }



        /// <summary>
        /// ����������ƴ�ӳ��ַ���
        /// </summary>
        public static string ObjectArrayToString(object[] array, string splitStr)
        {
            if (array == null || array.Length == 0)
                return "";

            StringBuilder result = new StringBuilder();
            for (int i = 0; i < array.Length; i++)
                result.AppendFormat("{0}{1}", array[i], splitStr);

            return result.Remove(result.Length - splitStr.Length, splitStr.Length).ToString();
        }

        /// <summary>
        /// ����������ƴ�ӳ��ַ���
        /// </summary>
        public static string ObjectArrayToString(object[] array)
        {
            return ObjectArrayToString(array, ",");
        }

        /// <summary>
        /// ���ַ�������ƴ�ӳ��ַ���
        /// </summary>
        public static string StringArrayToString(string[] array, string splitStr)
        {
            return ObjectArrayToString(array, splitStr);
        }

        /// <summary>
        /// ���ַ�������ƴ�ӳ��ַ���
        /// </summary>
        public static string StringArrayToString(string[] array)
        {
            return StringArrayToString(array, ",");
        }

        /// <summary>
        /// ����������ƴ�ӳ��ַ���
        /// </summary>
        public static string IntArrayToString(int[] array, string splitStr)
        {
            if (array == null || array.Length == 0)
                return "";

            StringBuilder result = new StringBuilder();
            for (int i = 0; i < array.Length; i++)
                result.AppendFormat("{0}{1}", array[i], splitStr);

            return result.Remove(result.Length - splitStr.Length, splitStr.Length).ToString();
        }

        /// <summary>
        /// ����������ƴ�ӳ��ַ���
        /// </summary>
        public static string IntArrayToString(int[] array)
        {
            return IntArrayToString(array, ",");
        }



        /// <summary>
        /// �Ƴ������е�ָ����
        /// </summary>
        /// <param name="array">Դ����</param>
        /// <param name="removeItem">Ҫ�Ƴ�����</param>
        /// <param name="removeBackspace">�Ƿ��Ƴ��ո�</param>
        /// <param name="ignoreCase">�Ƿ���Դ�Сд</param>
        /// <returns></returns>
        public static string[] RemoveArrayItem(string[] array, string removeItem, bool removeBackspace, bool ignoreCase)
        {
            if (array != null && array.Length > 0)
            {
                StringBuilder arrayStr = new StringBuilder();
                if (ignoreCase)
                    removeItem = removeItem.ToLower();
                string temp = "";
                foreach (string item in array)
                {
                    if (ignoreCase)
                        temp = item.ToLower();
                    else
                        temp = item;

                    if (temp != removeItem)
                        arrayStr.AppendFormat("{0}_", removeBackspace ? item.Trim() : item);
                }

                return StringHelper.SplitString(arrayStr.Remove(arrayStr.Length - 1, 1).ToString(), "_");
            }

            return array;
        }

        /// <summary>
        /// �Ƴ������е�ָ����
        /// </summary>
        /// <param name="array">Դ����</param>
        /// <returns></returns>
        public static string[] RemoveArrayItem(string[] array)
        {
            return RemoveArrayItem(array, "", true, false);
        }

        /// <summary>
        /// �Ƴ��ַ����е�ָ����
        /// </summary>
        /// <param name="s">Դ�ַ���</param>
        /// <param name="splitStr">�ָ��ַ���</param>
        /// <returns></returns>
        public static string[] RemoveStringItem(string s, string splitStr)
        {
            return RemoveArrayItem(StringHelper.SplitString(s, splitStr), "", true, false);
        }

        /// <summary>
        /// �Ƴ��ַ����е�ָ����
        /// </summary>
        /// <param name="s">Դ�ַ���</param>
        /// <returns></returns>
        public static string[] RemoveStringItem(string s)
        {
            return RemoveArrayItem(StringHelper.SplitString(s), "", true, false);
        }



        /// <summary>
        /// �Ƴ������е��ظ���
        /// </summary>
        /// <returns></returns>
        public static int[] RemoveRepeaterItem(int[] array)
        {
            if (array == null || array.Length < 2)
                return array;

            Array.Sort(array);

            int length = 1;
            for (int i = 1; i < array.Length; i++)
            {
                if (array[i] != array[i - 1])
                    length++;
            }

            int[] uniqueArray = new int[length];
            uniqueArray[0] = array[0];
            int j = 1;
            for (int i = 1; i < array.Length; i++)
                if (array[i] != array[i - 1])
                    uniqueArray[j++] = array[i];

            return uniqueArray;
        }

        /// <summary>
        /// �Ƴ������е��ظ���
        /// </summary>
        /// <returns></returns>
        public static string[] RemoveRepeaterItem(string[] array)
        {
            if (array == null || array.Length < 2)
                return array;

            Array.Sort(array);

            int length = 1;
            for (int i = 1; i < array.Length; i++)
            {
                if (array[i] != array[i - 1])
                    length++;
            }

            string[] uniqueArray = new string[length];
            uniqueArray[0] = array[0];
            int j = 1;
            for (int i = 1; i < array.Length; i++)
                if (array[i] != array[i - 1])
                    uniqueArray[j++] = array[i];

            return uniqueArray;
        }

        /// <summary>
        /// ȥ���ַ����е��ظ�Ԫ��
        /// </summary>
        /// <returns></returns>
        public static string GetUniqueString(string s)
        {
            return GetUniqueString(s, ",");
        }

        /// <summary>
        /// ȥ���ַ����е��ظ�Ԫ��
        /// </summary>
        /// <returns></returns>
        public static string GetUniqueString(string s, string splitStr)
        {
            return ObjectArrayToString(RemoveRepeaterItem(StringHelper.SplitString(s, splitStr)), splitStr);
        }

        #endregion

        /// <summary>
        /// ȥ���ַ�����β���Ŀո񡢻س������з����Ʊ��
        /// </summary>
        public static string TBBRTrim(string str)
        {
            if (!string.IsNullOrEmpty(str))
                return str.Trim().Trim('\r').Trim('\n').Trim('\t');
            return string.Empty;
        }

        /// <summary>
        /// ȥ���ַ����еĿո񡢻س������з����Ʊ��
        /// </summary>
        public static string ClearTBBR(string str)
        {
            if (!string.IsNullOrEmpty(str))
                return _tbbrRegex.Replace(str, "");

            return string.Empty;
        }

        /// <summary>
        /// ɾ���ַ����еĿ���
        /// </summary>
        /// <returns></returns>
        public static string DeleteNullOrSpaceRow(string s)
        {
            if (string.IsNullOrEmpty(s))
                return "";

            string[] tempArray = StringHelper.SplitString(s, "\r\n");
            StringBuilder result = new StringBuilder();
            foreach (string item in tempArray)
            {
                if (!string.IsNullOrWhiteSpace(item))
                    result.AppendFormat("{0}\r\n", item);
            }
            if (result.Length > 0)
                result.Remove(result.Length - 2, 2);
            return result.ToString();
        }

        /// <summary>
        /// ���ָ��������html�ո�
        /// </summary>
        /// <returns></returns>
        public static string GetHtmlBS(int count)
        {
            if (count == 1)
                return "&nbsp;&nbsp;&nbsp;&nbsp;";
            else if (count == 2)
                return "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";
            else if (count == 3)
                return "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";
            else
            {
                StringBuilder result = new StringBuilder();

                for (int i = 0; i < count; i++)
                    result.Append("&nbsp;&nbsp;&nbsp;&nbsp;");

                return result.ToString();
            }
        }

        /// <summary>
        /// ���ָ��������htmlSpanԪ��
        /// </summary>
        /// <returns></returns>
        public static string GetHtmlSpan(int count)
        {
            if (count <= 0)
                return "";

            if (count == 1)
                return "<span></span>";
            else if (count == 2)
                return "<span></span><span></span>";
            else if (count == 3)
                return "<span></span><span></span><span></span>";
            else
            {
                StringBuilder result = new StringBuilder();

                for (int i = 0; i < count; i++)
                    result.Append("<span></span>");

                return result.ToString();
            }
        }

        /// <summary>
        ///��������ṩ��
        /// </summary>
        /// <param name="email">����</param>
        /// <returns></returns>
        public static string GetEmailProvider(string email)
        {
            int index = email.LastIndexOf('@');
            if (index > 0)
                return email.Substring(index + 1);
            return string.Empty;
        }

        /// <summary>
        /// ת��������ʽ
        /// </summary>
        public static string EscapeRegex(string s)
        {
            string[] oList = { "\\", ".", "+", "*", "?", "{", "}", "[", "^", "]", "$", "(", ")", "=", "!", "<", ">", "|", ":" };
            string[] eList = { "\\\\", "\\.", "\\+", "\\*", "\\?", "\\{", "\\}", "\\[", "\\^", "\\]", "\\$", "\\(", "\\)", "\\=", "\\!", "\\<", "\\>", "\\|", "\\:" };
            for (int i = 0; i < oList.Length; i++)
                s = s.Replace(oList[i], eList[i]);
            return s;
        }

        /// <summary>
        /// ��ip��ַת����long����
        /// </summary>
        /// <param name="ip">ip</param>
        /// <returns></returns>
        public static long ConvertIPToLong(string ip)
        {
            string[] ips = ip.Split('.');
            long number = 16777216L * long.Parse(ips[0]) + 65536L * long.Parse(ips[1]) + 256 * long.Parse(ips[2]) + long.Parse(ips[3]);
            return number;
        }

        /// <summary>
        /// ��������
        /// </summary>
        public static string HideEmail(string email)
        {
            int index = email.LastIndexOf('@');

            if (index == 1)
                return "*" + email.Substring(index);
            if (index == 2)
                return email[0] + "*" + email.Substring(index);

            StringBuilder sb = new StringBuilder();
            sb.Append(email.Substring(0, 2));
            int count = index - 2;
            while (count > 0)
            {
                sb.Append("*");
                count--;
            }
            sb.Append(email.Substring(index));
            return sb.ToString();
        }

        /// <summary>
        /// �����ֻ�
        /// </summary>
        public static string HideMobile(string mobile)
        {
            return mobile.Substring(0, 3) + "*****" + mobile.Substring(8);
        }

        /// <summary> 
        /// DataTableת��ΪList
        /// </summary> 
        /// <param name="dt">DataTable</param> 
        /// <returns></returns> 
        public static List<Dictionary<string, object>> DataTableToList(DataTable dt)
        {
            int columnCount = dt.Columns.Count;
            List<Dictionary<string, object>> list = new List<Dictionary<string, object>>(dt.Rows.Count);
            foreach (DataRow dr in dt.Rows)
            {
                Dictionary<string, object> item = new Dictionary<string, object>(columnCount);
                for (int i = 0; i < columnCount; i++)
                {
                    item.Add(dt.Columns[i].ColumnName, dr[i]);
                }
                list.Add(item);
            }
            return list;
        }

        
        public static string ToBase64(string source, Encoding encoding = null) {
            if (encoding == null) {
                encoding = Encoding.UTF8;
            }

            try {
                byte[] bytes = encoding.GetBytes(source);
                return Convert.ToBase64String(bytes);
            }
            catch {
                return string.Empty;
            }
        }
    }
}
