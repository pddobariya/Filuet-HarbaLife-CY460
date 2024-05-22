using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Validations;
using System;
using System.ComponentModel.DataAnnotations;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Dtos.UZ
{
    public class Passport
    {
        #region Properties

        [Required(ErrorMessage = "Для доверенности требуется ФИО")]
        public string FIO { get; set; }

        [Required(ErrorMessage = "Для доверенности требуется серия паспорта")]
        [StringLengthIs(2)]
        public string PassportSeries { get; set; }

        [Required(ErrorMessage = "Для доверенности требуется номер паспорта")]
        [StringLengthIs(7)]
        public string PassportNumber { get; set; }

        [Required(ErrorMessage = "Для доверенности требуется информация кем выдан паспорт")]
        public string PassportIssued { get; set; }

        [Required(ErrorMessage = "Для доверенности требуется дата выдачи паспорта")]
        public DateTime PassportIssuedDate { get; set; }

        public string GetPassport() => $"{PassportSeries} {PassportNumber}";

        #endregion
    }
}
