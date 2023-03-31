﻿using System;
using System.ComponentModel.DataAnnotations;

namespace QTComputer.ModelViews
{
    public class MuaHangVM
    {

        public int CustomerId { get; set; }

        public string FullName { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập Họ và Tên")]
        public string? Email { get; set; }
        public string Phone { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        public string Address { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập Địa chỉ nhận hàng")]
        public int TinhThanh { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn Quận/Huyện")]
        public int QuanHuyen { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn Phường/Xã")]
        public int PhuongXa { get; set; }
        public int PaymentID { get; set; }
        public string? Note { get; set; }
    }
}