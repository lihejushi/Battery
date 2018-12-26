using System;

namespace XT.MVC.Core.Domain.Logging
{
    public partial class Log : BaseEntity
    {
        public int Id { get; set; }
        /// <summary>
        /// ��־�ȼ�
        /// </summary>
        public int LogLevelId { get; set; }
        /// <summary>
        /// ��־����
        /// </summary>
        public string ShortMessage { get; set; }
        /// <summary>
        /// ��־��������
        /// </summary>
        public string FullMessage { get; set; }
        /// <summary>
        /// ip��ַ
        /// </summary>
        public string IpAddress { get; set; }
        /// <summary>
        /// ��¼ҳ��
        /// </summary>
        public string PageUrl { get; set; }
        /// <summary>
        /// ��Դ��ַ
        /// </summary>
        public string ReferrerUrl { get; set; }
        /// <summary>
        /// ��¼ʱ��
        /// </summary>
        public DateTime CreatedOnUtc { get; set; }

        public LogLevel LogLevel
        {
            get
            {
                return (LogLevel)this.LogLevelId;
            }
            set
            {
                this.LogLevelId = (int)value;
            }
        }
    }
}
